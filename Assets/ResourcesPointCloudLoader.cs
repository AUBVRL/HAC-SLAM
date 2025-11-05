using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;

/// <summary>
/// Simple Resources-based pointcloud loader that expects a JSON file in Resources/pointclouds/<resourceName>
/// JSON shape expected: { "num_points": N, "points": [ [x,y,z], [x,y,z], ... ] }
/// Streams points into VoxelManager.AddVoxel(point, false) while yielding periodically to avoid frame hitches.
/// Works out-of-the-box in Editor, Desktop, HoloLens, iOS for small/medium files.
/// </summary>
public class ResourcesPointCloudLoader : MonoBehaviour
{
    [Tooltip("Path under Resources without extension. Example: pointclouds/testply")]
    public string resourcePath = "pointclouds/testply";

    [Tooltip("How many voxels to add between yielding a frame (tune: 100-200 for HoloLens, 500-2000 for desktop)")]
    public int yieldEvery = 500;

    [Tooltip("Auto-start loading on Awake")]
    public bool autoStart = true;

    [Tooltip("If true, swap Y/Z axis when calling VoxelManager (use if coordinates are in different ordering)")]
    public bool swapYandZ = false;

    void Awake()
    {
        if (autoStart) StartCoroutine(LoadAndFillCoroutine());
    }

    /// <summary>
    /// Public entry if you want to call from UI
    /// </summary>
    public void StartLoad() => StartCoroutine(LoadAndFillCoroutine());

    IEnumerator LoadAndFillCoroutine()
    {
        if (string.IsNullOrEmpty(resourcePath))
        {
            Debug.LogError("[ResourcesPointCloudLoader] resourcePath is empty.");
            yield break;
        }

        // 1) Load TextAsset from Resources
        TextAsset ta = Resources.Load<TextAsset>(resourcePath);
        if (ta == null)
        {
            Debug.LogError($"[ResourcesPointCloudLoader] Resource not found: Resources/{resourcePath}.json");
            yield break;
        }

        string jsonText = ta.text;
        if (string.IsNullOrEmpty(jsonText))
        {
            Debug.LogError("[ResourcesPointCloudLoader] Loaded JSON is empty.");
            yield break;
        }

        // 2) Stream-parse JSON and feed VoxelManager on the fly (no big List allocation)
        int added = 0;
        int counter = 0;

        // lightweight streaming parser for known shape: it finds "points" array and reads numeric tokens as triples
        int idx = jsonText.IndexOf("\"points\"", StringComparison.OrdinalIgnoreCase);
        if (idx < 0) idx = jsonText.IndexOf("'points'", StringComparison.OrdinalIgnoreCase);
        if (idx < 0)
        {
            Debug.LogError("[ResourcesPointCloudLoader] Could not find \"points\" in JSON.");
            yield break;
        }

        // move to the first '[' after "points"
        idx = jsonText.IndexOf('[', idx);
        if (idx < 0)
        {
            Debug.LogError("[ResourcesPointCloudLoader] Malformed JSON: no '[' after points.");
            yield break;
        }
        idx++; // step into array
        int len = jsonText.Length;

        StringBuilder token = new StringBuilder();
        bool inNumber = false;
        List<float> triple = new List<float>(3);

        while (idx < len)
        {
            char c = jsonText[idx];

            if ((c >= '0' && c <= '9') || c == '-' || c == '+' || c == '.' || c == 'e' || c == 'E')
            {
                token.Append(c);
                inNumber = true;
            }
            else
            {
                if (inNumber)
                {
                    string s = token.ToString();
                    token.Length = 0;
                    inNumber = false;
                    if (float.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out float val))
                    {
                        triple.Add(val);
                        if (triple.Count == 3)
                        {
                            // produce Vector3
                            Vector3 pt = new Vector3(triple[0], triple[1], triple[2]);
                            if (swapYandZ) pt = new Vector3(triple[0], triple[2], triple[1]);

                            pt = PrefabsManager.imageTarget.transform.TransformPoint(pt);
                            VoxelManager.AddVoxel(pt, false);

                            triple.Clear();
                            added++;
                            counter++;
                            if (counter >= yieldEvery)
                            {
                                counter = 0;
                                // yield back to engine to avoid stutter
                                yield return null;
                            }
                        }
                    }
                    else
                    {
                        // couldn't parse number: ignore and continue
                    }
                }

                // quick early-out if we've probably left the points array: detect the end of the main points array "]]" followed by maybe "}"
                if (c == ']')
                {
                    // check next non-whitespace; if next is ']' still inside inner arrays, continue; else we might be at end - but keep parsing until no numbers remain
                }
            }
            idx++;
        }

        Debug.Log($"[ResourcesPointCloudLoader] Done streaming. Voxels added: {added:N0}");
        // call your view manager same as before
        //try
        //{
        //    ViewManager.ViewInitialChunks();
        //}
        //catch (Exception ex)
        //{
        //    Debug.LogWarning("ViewInitialChunks threw: " + ex.Message);
        //}
    }
}