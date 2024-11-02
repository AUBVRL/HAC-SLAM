using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class ViewManager : MonoBehaviour
{
    Vector3Int currentChunk;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("checkCameraPosition", 0, 0.1f);
    }

    // Update is called once per frame
    void checkCameraPosition()
    {
        Vector3 cameraPosition = Camera.main.transform.position;
        Vector3Int cameraChunk = Vector3Int.RoundToInt(VoxelManager.RoundToChunk(cameraPosition));
        
        if (cameraChunk != currentChunk)
        {
            Debug.Log(cameraChunk);
            ManageEnabledChunks(cameraChunk);
            currentChunk = cameraChunk;
        }
    }

    public void ManageEnabledChunks(Vector3Int v)
    {
        Vector3Int diff = v - currentChunk;
        Vector3Int Absdiff = new(Mathf.Abs(diff.x), Mathf.Abs(diff.y), Mathf.Abs(diff.z));
        
        for(int i = Absdiff.x - 3 ; i <= 3 - Absdiff.x; i+=3)
        {
            for(int j = Absdiff.y ; j <= 6 - Absdiff.y; j+=3)
            {
                for(int k = Absdiff.z - 3; k <= 3 - Absdiff.z; k+=3)
                {
                    Vector3 surroundingChunk = new Vector3(i, j, k);

                    Vector3 newChunkToEnable = v + surroundingChunk + diff;

                    Vector3 oldChunkToDisable = currentChunk + surroundingChunk - diff;

                    if (VoxelManager.ChunksDict.ContainsKey(newChunkToEnable))
                    {
                        print("Activating chunk");
                        VoxelManager.ChunksDict[newChunkToEnable].ChunkGameObject.SetActive(true);
                    }

                    if (VoxelManager.ChunksDict.ContainsKey(oldChunkToDisable))
                    {
                        print("Deactivating chunk");
                        VoxelManager.ChunksDict[oldChunkToDisable].ChunkGameObject.SetActive(false);
                    }
                }
            }
        }
        
    }
}
