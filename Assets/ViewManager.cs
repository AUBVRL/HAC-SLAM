using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewManager : MonoBehaviour
{
    Vector3Int currentChunk;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("checkCameraPosition", 0, 0.1f);
    }

    void checkCameraPosition()
    {
        Vector3 cameraPosition = Camera.main.transform.position;
        Vector3Int cameraChunk = Vector3Int.RoundToInt(VoxelManager.RoundToChunk(cameraPosition));
        if (cameraChunk != currentChunk)
        {
            ManageEnabledChunks(cameraChunk);
            currentChunk = cameraChunk;
        }
    }

    public void ManageEnabledChunks(Vector3Int v)
    {
        Vector3Int diff = v - currentChunk;
        Vector3Int Absdiff = new(Mathf.Abs(diff.x), Mathf.Abs(diff.y), Mathf.Abs(diff.z));
        for (int i = Absdiff.x - 3; i <= 3 - Absdiff.x; i += 3)
        {
            for (int j = Absdiff.y; j <= 6 - Absdiff.y; j += 3)
            {
                for (int k = Absdiff.z - 3; k <= 3 - Absdiff.z; k += 3)
                {
                    Vector3 surroundingChunk = new Vector3(i, j, k);
                    Vector3 newChunkToEnable = v + surroundingChunk + diff;
                    Vector3 oldChunkToDisable = currentChunk + surroundingChunk - diff;
                    if (VoxelManager.ChunksDict.ContainsKey(newChunkToEnable))
                    {
                        VoxelManager.ChunksDict[newChunkToEnable].gameobject.SetActive(true);
                    }
                    if (VoxelManager.ChunksDict.ContainsKey(oldChunkToDisable))
                    {
                        VoxelManager.ChunksDict[oldChunkToDisable].gameobject.SetActive(false);
                    }
                }
            }
        }
    }

    public static void ViewInitialChunks()
    {
        Vector3 cameraPosition = VoxelManager.RoundToChunk(Camera.main.transform.position);
        Vector3 increment = new Vector3();
        for (float i = -PrefabsManager.chunkSize; i <= PrefabsManager.chunkSize; i += PrefabsManager.chunkSize)
        {
            for (float j = 0; j <= PrefabsManager.chunkSize; j += PrefabsManager.chunkSize)
            {
                for (float k = -PrefabsManager.chunkSize; k <= PrefabsManager.chunkSize; k += PrefabsManager.chunkSize)
                {
                    increment.Set(i, j, k);
                    if (VoxelManager.ChunksDict.ContainsKey(cameraPosition + increment))
                    {
                        Chunk chunk = VoxelManager.ChunksDict[cameraPosition + increment];
                        chunk.gameobject.SetActive(true);
                    }
                }
            }
        }
        Debug.Log("Done Showing Initial Chunks");
    }

}