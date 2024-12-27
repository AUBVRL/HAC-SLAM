using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    public GameObject prefab;
    public Vector3 position;
    public Dictionary<Vector3, Voxel> VoxelsDict;

    public Chunk(Vector3 voxelPosition)
    {
        position = VoxelManager.RoundToChunk(voxelPosition);
        prefab = UnityEngine.Object.Instantiate(PrefabsManager.chunkPrefab,position, Quaternion.identity, PrefabsManager.chunkParentPrefab.transform);
        prefab.SetActive(false);
        VoxelsDict = new Dictionary<Vector3, Voxel>();
        VoxelsDict.Add(voxelPosition, new Voxel(voxelPosition, prefab));
    }

    public void AddVoxel(Vector3 voxelPosition, bool humanEdited = false)
    {
        VoxelsDict.Add(voxelPosition, new Voxel(voxelPosition, prefab, humanEdited));
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
