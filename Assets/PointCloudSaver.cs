using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using TMPro;
using UnityEngine;

[Serializable]
public class PointData
{
    public float x;
    public float y;
    public float z;
    public bool added; // true if voxel was added (vs original). set to false if unknown.

    public PointData(float x, float y, float z, bool added)
    {
        this.x = x; this.y = y; this.z = z; this.added = added;
    }
}

[Serializable]
public class SavedPointCloudMeta
{
    //public string original_id;
    //public string user_id;
    public string platform;
    public string timestamp_utc;
    public float voxel_size;
    public int num_points;
    public string notes;
}

public class PointCloudSaver : MonoBehaviour
{
    [Header("Save options")]
    [Tooltip("Number of points to write before yielding a frame. Reduce for HoloLens/iOS.")]
    public int yieldEvery = 1000;

    [Tooltip("Subfolder under platform base path where files will be saved")]
    public string exportsSubfolder = "HAC-SLAM/exports";

    [Tooltip("Optional note to include in metadata")]
    public string notes = "";

    public TextMeshPro TextMeshPro;

    /// <summary>
    /// Public wrapper to start saving. Call this from your UI after editing finishes.
    /// originalId: the id of the original scan (e.g., faro_scan_001)
    /// userId: experiment participant id
    /// </summary>
    public void StartSaveCoroutine() //(string originalId, string userId)
    {
        StartCoroutine(SaveEditedPointCloudCoroutine());  //(originalId, userId));
    }

    IEnumerator SaveEditedPointCloudCoroutine() //(string originalId, string userId)
    {
        // Validate VoxelManager
        if (VoxelManager.ChunksDict == null)
        {
            Debug.LogError("[PointCloudSaver] VoxelManager.ChunksDict is null. Nothing to save.");
            yield break;
        }

        // Choose base folder based on platform (makes it easy to find files on desktop)
        string baseFolder = GetPlatformBaseSaveFolder();
        string fullFolder = Path.Combine(baseFolder, exportsSubfolder);
        try
        {
            if (!Directory.Exists(fullFolder))
                Directory.CreateDirectory(fullFolder);
        }
        catch (Exception ex)
        {
            Debug.LogError("[PointCloudSaver] Failed to create folder: " + fullFolder + " -> " + ex.Message);
            yield break;
        }

        string platformName = GetPlatformString();
        string timestamp = DateTime.UtcNow.ToString("yyyyMMdd'T'HHmmss", CultureInfo.InvariantCulture);
        string fileName = $"{platformName}_{timestamp}.json";//$"{originalId}_edited_{userId}_{platformName}_{timestamp}.json";
        string outPath = Path.Combine(fullFolder, fileName);

        // Prepare metadata
        SavedPointCloudMeta meta = new SavedPointCloudMeta
        {
            //original_id = originalId,
            //user_id = userId,
            platform = platformName,
            timestamp_utc = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            voxel_size = PrefabsManager.voxelSize,
            num_points = CountVoxels(),
            notes = notes
        };

        // Start streaming write
        Debug.Log($"[PointCloudSaver] Saving {meta.num_points:N0} points to: {outPath}");
        using (var fs = new FileStream(outPath, FileMode.Create, FileAccess.Write, FileShare.None))
        using (var sw = new StreamWriter(fs, new UTF8Encoding(false)))
        {
            // Write opening and metadata
            sw.Write("{\n");
            sw.Write($"  \"meta\": {JsonUtility.ToJson(meta, true)},\n");
            sw.Write("  \"points\": [\n");
            sw.Flush();

            int written = 0;
            int innerCounter = 0;
            bool firstPoint = true;

            // Iterate chunks => voxels. This mirrors VoxelManager internal structure
            // We do not allocate a large list; we write each point as we see it.
            foreach (var chunkPair in VoxelManager.ChunksDict)
            {
                var chunk = chunkPair.Value;
                // Each chunk has VoxelsDict: Dictionary<Vector3, Voxel>
                foreach (var voxelPair in chunk.VoxelsDict)
                {
                    Vector3 pos = voxelPair.Key;
                    Voxel voxelObj = voxelPair.Value;
                    //bool addedFlag = voxelObj != null && voxelObj.IsAdded ? true : false; // you may need to expose this property in Voxel class

                    // write comma separation
                    if (!firstPoint)
                        sw.Write(",\n");
                    else
                        firstPoint = false;

                    // write point JSON object (compact)
                    // Use invariant culture for floats to ensure '.' decimal separator
                    string pointJson = string.Format(CultureInfo.InvariantCulture,
                        "    {{\"x\":{0},\"y\":{1},\"z\":{2}}}",
                        pos.x, pos.z, pos.y); //, addedFlag ? "true" : "false");

                    sw.Write(pointJson);

                    written++;
                    innerCounter++;

                    if (innerCounter >= yieldEvery)
                    {
                        innerCounter = 0;
                        sw.Flush();
                        // Yield so we don't block main thread
                        yield return null;
                    }
                }
            }

            // close array and object
            sw.Write("\n  ]\n}\n");
            sw.Flush();
        }

        Debug.Log($"[PointCloudSaver] Save complete. Wrote {meta.num_points:N0} points to: {outPath}");
        TextMeshPro.text = $"Saved {meta.num_points:N0} points.";

        // Optionally: write an accompanying metadata-only JSON for quick inspection
        string metaPath = Path.Combine(fullFolder, Path.GetFileNameWithoutExtension(fileName) + ".meta.json");
        try
        {
            File.WriteAllText(metaPath, JsonUtility.ToJson(meta, true), new UTF8Encoding(false));
        }
        catch (Exception ex)
        {
            Debug.LogWarning("[PointCloudSaver] Failed to write metadata file: " + ex.Message);
        }

        yield break;
    }

    int CountVoxels()
    {
        int count = 0;
        foreach (var chunkPair in VoxelManager.ChunksDict)
        {
            var chunk = chunkPair.Value;
            count += chunk.VoxelsDict.Count;
        }
        return count;
    }

    string GetPlatformBaseSaveFolder()
    {
        // Desktop: write to user's Documents/HAC-SLAM/exports for easy retrieval
        // Mobile/UWP: use Application.persistentDataPath (sandboxed)
        // Editor: write to Application.dataPath/../SavedPointclouds for convenience
#if UNITY_EDITOR
        // place under project root /SavedPointclouds so it's easy to find
        string editorPath = Path.Combine(Application.dataPath, "..", "SavedPointclouds");
        return Path.GetFullPath(editorPath);
#elif UNITY_STANDALONE_WIN
        string doc = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        return Path.Combine(doc); // we'll append exportsSubfolder later
#elif UNITY_STANDALONE_OSX
        string docMac = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        return Path.Combine(docMac);
#elif UNITY_STANDALONE_LINUX
        return Application.persistentDataPath;
#elif UNITY_WSA || WINDOWS_UWP
        // HoloLens / UWP - app sandbox; persistentDataPath is the right choice
        return Application.persistentDataPath;
#elif UNITY_IOS
        return Application.persistentDataPath;
#elif UNITY_ANDROID
        return Application.persistentDataPath;
#else
        return Application.persistentDataPath;
#endif
    }

    string GetPlatformString()
    {
#if UNITY_EDITOR
        return "Editor";
#elif UNITY_STANDALONE_WIN
        return "Windows";
#elif UNITY_STANDALONE_OSX
        return "macOS";
#elif UNITY_STANDALONE_LINUX
        return "Linux";
#elif UNITY_WSA || WINDOWS_UWP
        return "UWP";
#elif UNITY_IOS
        return "iOS";
#elif UNITY_ANDROID
        return "Android";
#else
        return Application.platform.ToString();
#endif
    }
}
