using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using static UnityEngine.UIElements.UxmlAttributeDescription;

// If you have Newtonsoft.Json installed, it will be used automatically.
// Otherwise the script falls back to a small parser that reads the predictable JSON:
// { "num_points": N, "points": [ [x,y,z], [x,y,z], ... ] }

public class PointCloudLoader : MonoBehaviour
{
    [Header("StreamingAssets relative path (under Assets/StreamingAssets)")]
    public string fileName = "pointclouds/testply.json";

    [Header("Yield frequency (how many voxels before yielding)")]
    public int yieldEvery = 500;

    [Header("Auto start on Awake?")]
    public bool autoStart = true;

    void Start()
    {
        if (autoStart) StartLoad();
    }

    public void StartLoad()
    {
        StartCoroutine(LoadPointCloudAndFill());
    }

    IEnumerator LoadPointCloudAndFill()
    {
        string fullPath = "C:/Users/Dev-MohamadKY/Desktop/HAC-SLAM/Assets/StreamingAssets/pointclouds/testply.json";

        // 1) Load file text cross-platform
        string jsonText = null;
        if (Application.platform == RuntimePlatform.Android)
        {
            // On Android streamingAssets are packed inside the APK and must be read via UnityWebRequest
            string uri = Path.Combine(Application.streamingAssetsPath, fileName);
            using (UnityWebRequest uwr = UnityWebRequest.Get(uri))
            {
                yield return uwr.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
                if (uwr.result != UnityWebRequest.Result.Success)
#else
                if (uwr.isNetworkError || uwr.isHttpError)
#endif
                {
                    Debug.LogError($"Failed to load streaming asset (Android): {uwr.error} -- {uri}");
                    yield break;
                }
                jsonText = uwr.downloadHandler.text;
            }
        }
        else
        {
            // Editor / Standalone / iOS - can read directly from disk
            if (!File.Exists(fullPath))
            {
                Debug.LogError($"Pointcloud JSON not found: {fullPath}");
                yield break;
            }
            jsonText = File.ReadAllText(fullPath, Encoding.UTF8);
        }

        if (string.IsNullOrEmpty(jsonText))
        {
            Debug.LogError("Loaded JSON text was empty.");
            yield break;
        }

        // 2) Parse JSON -> List<Vector3>
        List<Vector3> points = null;

        // Try Newtonsoft if available
        try
        {
            // Attempt to use Newtonsoft (if available in project)
            // This code is in try/catch so if Newtonsoft is not present it will fallthrough to fallback parser.
            var newtonsoft = Type.GetType("Newtonsoft.Json.JsonConvert, Newtonsoft.Json");
            if (newtonsoft != null)
            {
                // dynamic usage to avoid hard compile dependency
                var jo = newtonsoft
                    .GetMethod("DeserializeObject", new Type[] { typeof(string) })
                    .Invoke(null, new object[] { jsonText });

                // But easier: directly use a small internal DTO with JsonUtility? Instead use Newtonsoft strongly if present:
                // We'll use a simple dynamic-like approach: parse into a known structure using Newtonsoft via reflection.

                // Build a temporary strongly-typed call using generic method via reflection:
                var jObjectType = Type.GetType("Newtonsoft.Json.Linq.JObject, Newtonsoft.Json");
                if (jObjectType != null)
                {
                    var parseMethod = Type.GetType("Newtonsoft.Json.Linq.JObject, Newtonsoft.Json")
                                          .GetMethod("Parse", new Type[] { typeof(string) });
                    var jObj = parseMethod.Invoke(null, new object[] { jsonText });

                    var jArrayType = Type.GetType("Newtonsoft.Json.Linq.JArray, Newtonsoft.Json");
                    var tokenType = Type.GetType("Newtonsoft.Json.Linq.JToken, Newtonsoft.Json");

                    var pointsToken = jObj.GetType().GetMethod("get_Item", new Type[] { typeof(string) }).Invoke(jObj, new object[] { "points" });

                    // enumerate the array of arrays
                    points = new List<Vector3>();
                    var enumerator = (System.Collections.IEnumerable)pointsToken;
                    foreach (var sub in enumerator)
                    {
                        // each sub is a JArray containing 3 numbers
                        var subEnum = (System.Collections.IEnumerable)sub;
                        float[] triple = new float[3];
                        int idx = 0;
                        foreach (var val in subEnum)
                        {
                            float f = Convert.ToSingle(val.GetType().GetMethod("ToObject", new Type[] { typeof(Type) })
                                .Invoke(val, new object[] { typeof(float) }));
                            triple[idx++] = f;
                        }
                        if (idx >= 3) points.Add(new Vector3(triple[0], triple[1], triple[2]));
                    }
                }
                else
                {
                    // As a fallback (rare), attempt a simple JsonConvert to a known DTO
                    // We'll create a simple DTO class inline and use JsonConvert to parse - but reflection complexity increases.
                    throw new Exception("Newtonsoft present but JObject type missing; falling back.");
                }
            }
            else
            {
                // Newtonsoft not found, fall back to manual parser below
                throw new Exception("Newtonsoft not available.");
            }
        }
        catch (Exception)
        {
            // Fallback parser: a small streaming parser that expects:
            // { "num_points": N, "points": [ [x,y,z], [x,y,z], ... ] }
            points = ParsePointsFromJsonSimple(jsonText);
        }

        if (points == null || points.Count == 0)
        {
            Debug.LogError("No points parsed from JSON.");
            yield break;
        }

        Debug.Log($"Parsed {points.Count:N0} points. Starting voxel fill...");

        // 3) Fill voxels using your VoxelManager.AddVoxel interface (respect the original yield logic)
        yield return StartCoroutine(FillIncomingFromList(points, yieldEvery));

        Debug.Log("Point cloud fill complete.");
        yield break;
    }

    // Coroutine that mirrors your ROS-driven FillIncoming but uses a List<Vector3>
    IEnumerator FillIncomingFromList(List<Vector3> points, int yieldEvery = 500)
    {
        Vector3 point = Vector3.zero;
        int countTillYield = 0;

        for (int i = 0; i < points.Count; i++)
        {
            point = points[i];
            // your VoxelManager API is used unchanged:
            VoxelManager.AddVoxel(point, false);

            countTillYield++;
            if (countTillYield % yieldEvery == 0)
            {
                // give the engine a frame
                yield return null;
            }
        }

        Debug.Log("Done");
        // keep your call to ViewManager.ViewInitialChunks() as before
        ViewManager.ViewInitialChunks();

        yield break;
    }


    // A very small parser that is tolerant and memory-friendly for the known JSON shape.
    // It scans for numeric tokens inside the "points" array and collects them into triples.
    static List<Vector3> ParsePointsFromJsonSimple(string json)
    {
        List<Vector3> pts = new List<Vector3>();

        int i = 0;
        int len = json.Length;
        // find "points"
        int idx = json.IndexOf("\"points\"", StringComparison.OrdinalIgnoreCase);
        if (idx < 0) idx = json.IndexOf("'points'", StringComparison.OrdinalIgnoreCase);
        if (idx < 0) return pts;

        // move to first '[' after "points"
        i = json.IndexOf('[', idx);
        if (i < 0) return pts;
        i++; // step into array

        List<float> currentTriple = new List<float>(3);
        StringBuilder token = new StringBuilder();
        bool inNumber = false;
        while (i < len)
        {
            char c = json[i];

            if ((c >= '0' && c <= '9') || c == '-' || c == '+' || c == '.' || c == 'e' || c == 'E')
            {
                token.Append(c);
                inNumber = true;
            }
            else
            {
                if (inNumber)
                {
                    // finish token
                    string s = token.ToString();
                    token.Length = 0;
                    inNumber = false;
                    if (float.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out float val))
                    {
                        currentTriple.Add(val);
                        if (currentTriple.Count == 3)
                        {
                            pts.Add(new Vector3(currentTriple[0], currentTriple[2], currentTriple[1]));
                            currentTriple.Clear();
                        }
                    }
                    else
                    {
                        // parse error - skip
                    }
                }

                // end of points array?
                if (c == ']')
                {
                    // could be end of inner array or the whole points array.
                    // Check if next non-whitespace char corresponds to end of outer points array
                    // We'll just continue until we hit the closing bracket for the outermost 'points' array.
                    // Detect double ']]' or nested closure: if we find ']' followed by '}' then stop maybe.
                    // But simpler: if we find "}" after skipping whitespace it likely ends object.
                    // We'll allow loop to naturally end when no more numbers found.
                }
            }
            i++;
        }

        return pts;
    }
}

