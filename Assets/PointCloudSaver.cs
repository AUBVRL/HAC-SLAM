using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

#if UNITY_WSA && ENABLE_WINMD_SUPPORT
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Storage.Pickers;
using Windows.Foundation;
#endif

[Serializable]
public class PointData
{
    public float x;
    public float y;
    public float z;
    public bool added; // retained but not used for now

    public PointData(float x, float y, float z, bool added)
    {
        this.x = x; this.y = y; this.z = z; this.added = added;
    }
}

[Serializable]
public class SavedPointCloudMeta
{
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

    /// <summary>
    /// Call this to start saving
    /// </summary>
    public void StartSaveCoroutine()
    {
        StartCoroutine(SaveEditedPointCloudCoroutine());
    }

    IEnumerator SaveEditedPointCloudCoroutine()
    {
        if (VoxelManager.ChunksDict == null)
        {
            Debug.LogError("[PointCloudSaver] VoxelManager.ChunksDict is null. Nothing to save.");
            yield break;
        }

        // Choose local base folder and ensure it exists
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
        string fileName = $"{platformName}_{timestamp}.json";
        string localOutPath = Path.Combine(fullFolder, fileName);

        SavedPointCloudMeta meta = new SavedPointCloudMeta
        {
            platform = platformName,
            timestamp_utc = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            voxel_size = PrefabsManager.voxelSize,
            num_points = CountVoxels(),
            notes = notes
        };

        Debug.Log($"[PointCloudSaver] Saving {meta.num_points:N0} points to local path: {localOutPath}");

        // Write local file (streaming)
        
        using (var fs = new FileStream(localOutPath, FileMode.Create, FileAccess.Write, FileShare.None))
        using (var sw = new StreamWriter(fs, new UTF8Encoding(false)))
        {
            sw.Write("{\n");
            sw.Write($"  \"meta\": {JsonUtility.ToJson(meta, true)},\n");
            sw.Write("  \"points\": [\n");
            sw.Flush();

            int written = 0;
            int innerCounter = 0;
            bool firstPoint = true;

            foreach (var chunkPair in VoxelManager.ChunksDict)
            {
                var chunk = chunkPair.Value;
                foreach (var voxelPair in chunk.VoxelsDict)
                {
                    Vector3 pos = voxelPair.Key;
                    // Note: original code used pos.x, pos.z, pos.y ordering — preserve that if needed
                    string pointJson = string.Format(CultureInfo.InvariantCulture,
                        "    {{\"x\":{0},\"y\":{1},\"z\":{2}}}",
                        pos.x, pos.z, pos.y);

                    if (!firstPoint)
                        sw.Write(",\n");
                    else
                        firstPoint = false;

                    sw.Write(pointJson);

                    written++;
                    innerCounter++;

                    if (innerCounter >= yieldEvery)
                    {
                        innerCounter = 0;
                        sw.Flush();
                        yield return null;
                    }
                }
            }

            sw.Write("\n  ]\n}\n");
            sw.Flush();
        }

        Debug.Log($"[PointCloudSaver] Local save complete. Wrote {meta.num_points:N0} points to: {localOutPath}");
        
        

#if UNITY_WSA && ENABLE_WINMD_SUPPORT
        // On UWP/HoloLens, attempt to copy the local file into KnownFolders.DocumentsLibrary
        // so that the file is visible in the Files app (and via device portal / file explorers).
        
        Debug.Log("[PointCloudSaver] Running UWP copy to DocumentsLibrary...");

        // Use WinRT Storage APIs via Task bridge
        Task copyTask = CopyFileToDocumentsAsync(localOutPath, fileName);
        // Wait until completed
        yield return new WaitUntil(() => copyTask.IsCompleted);

        if (copyTask.IsFaulted)
        {
            Debug.LogWarning("[PointCloudSaver] Copy to DocumentsLibrary faulted: " + copyTask.Exception?.ToString());
        }
        else if (copyTask.IsCanceled)
        {
            Debug.LogWarning("[PointCloudSaver] Copy to DocumentsLibrary canceled.");
        }
        else
        {
            Debug.Log("[PointCloudSaver] Copy to DocumentsLibrary completed successfully. File should be visible in Files app under Documents.");
        }
        
#endif

        // Write metadata sidecar locally as well
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

#if UNITY_WSA && ENABLE_WINMD_SUPPORT
    // UWP async copy: copy local file at fullLocalPath into KnownFolders.DocumentsLibrary with destFileName
    static async Task CopyFileToDocumentsAsync(string fullLocalPath, string destFileName)
    {
        // Get the StorageFile for the local file path
        StorageFile sourceFile = await StorageFile.GetFileFromPathAsync(fullLocalPath);

        // Destination: KnownFolders.DocumentsLibrary
        StorageFolder docs = KnownFolders.DocumentsLibrary;

        // Copy file into DocumentsLibrary, replacing if exists
        await sourceFile.CopyAsync(docs, destFileName, NameCollisionOption.ReplaceExisting);
    }
#endif

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
#if UNITY_EDITOR
        string editorPath = Path.Combine(Application.dataPath, "..", "SavedPointclouds");
        return Path.GetFullPath(editorPath);
#elif UNITY_STANDALONE_WIN
        string doc = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        return Path.Combine(doc);
#elif UNITY_STANDALONE_OSX
        string docMac = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        return Path.Combine(docMac);
#elif UNITY_STANDALONE_LINUX
        return Application.persistentDataPath;
#elif UNITY_WSA || WINDOWS_UWP
        // Save first to persistentDataPath (app-local)
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
