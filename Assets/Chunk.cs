using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    public GameObject gameobject;
    public Vector3 position;
    public Dictionary<Vector3, Voxel> VoxelsDict;

    public Chunk(Vector3 chunkPosition, bool state)
    {
        position = chunkPosition;
        gameobject = UnityEngine.Object.Instantiate(PrefabsManager.chunkPrefab, Vector3.zero, Quaternion.identity, PrefabsManager.chunkParentPrefab.transform);
        gameobject.SetActive(state);
        VoxelsDict = new Dictionary<Vector3, Voxel>();
    }

    public List<byte> GetChunkByteData()
    {
        List<byte> byteList = new();

        // Iterate over the voxels in the chunk
        foreach (var voxelEntry in VoxelsDict.Values)
        {
            byteList.AddRange(voxelEntry.ToByteArray());  // Use Voxel's ToByteArray method
        }

        return byteList;  // Return byte array for the whole chunk
    }
}