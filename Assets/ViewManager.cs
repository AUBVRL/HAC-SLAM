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
        for (int i = Absdiff.x - 4; i <= 4 - Absdiff.x; i += 4)
        {
            for (int j = Absdiff.y; j <= 8 - Absdiff.y; j += 4)
            {
                for (int k = Absdiff.z - 4; k <= 4 - Absdiff.z; k += 4)
                {
                    Vector3 surroundingChunk = new Vector3(i, j, k);
                    Vector3 newChunkToEnable = v + surroundingChunk + diff;
                    Vector3 oldChunkToDisable = currentChunk + surroundingChunk - diff;
                    if (VoxelManager.ChunksDict.ContainsKey(newChunkToEnable))
                    {
                        Debug.Log("ACTIVATING CHUNK");
                        VoxelManager.ChunksDict[newChunkToEnable].prefab.SetActive(true);
                    }
                    if (VoxelManager.ChunksDict.ContainsKey(oldChunkToDisable))
                    {
                        Debug.Log("DISABLE CHUNK");
                        VoxelManager.ChunksDict[oldChunkToDisable].prefab.SetActive(false);
                    }
                }
            }
        }
    }

    public void ViewInitialChunks()
    {
        Vector3 cameraPosition = Camera.main.transform.position;
        cameraPosition = VoxelManager.RoundToChunk(cameraPosition);
        Vector3 increment = new Vector3();
        for (float i = -PrefabsManager.chunkSize; i <= PrefabsManager.chunkSize; i += PrefabsManager.chunkSize)
        {
            for (float j = -PrefabsManager.chunkSize; j <= PrefabsManager.chunkSize; j += PrefabsManager.chunkSize)
            {
                for (float k = -PrefabsManager.chunkSize; k <= PrefabsManager.chunkSize; k += PrefabsManager.chunkSize)
                {
                    increment.Set(i, j, k);
                    if (VoxelManager.ChunksDict.ContainsKey(cameraPosition + increment))
                    {
                        Chunk chunk = VoxelManager.ChunksDict[cameraPosition + increment];
                        if (!chunk.prefab.activeInHierarchy) chunk.prefab.SetActive(true);
                    }
                }
            }
        }
    }

}