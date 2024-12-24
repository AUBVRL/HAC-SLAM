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
        VoxelsDict = new Dictionary<Vector3, Voxel>();
        VoxelsDict.Add(voxelPosition, new Voxel(voxelPosition, prefab));
    }

    public void AddVoxel(Vector3 voxelPosition, bool humanEdited = false)
    {
        VoxelsDict.Add(voxelPosition, new Voxel(voxelPosition, prefab, humanEdited));
    }




}
