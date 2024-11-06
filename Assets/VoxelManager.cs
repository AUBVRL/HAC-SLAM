using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelManager : MonoBehaviour
{
    public static Dictionary<Vector3, Chunk> ChunksDict;
    void Start()
    {
        ChunksDict = new Dictionary<Vector3, Chunk>();
    }

    public static void AddVoxel(Vector3 point, bool state)
    {
        Vector3 voxelVector = RoundToVoxel(point);
        Vector3 chunkVector = RoundToChunk(voxelVector);
        if (!ChunksDict.ContainsKey(chunkVector))
        {
            ChunksDict.Add(chunkVector, new Chunk(chunkVector, state));
        }
        if (!ChunksDict[chunkVector].VoxelsDict.ContainsKey(voxelVector))
        {
            ChunksDict[chunkVector].VoxelsDict.Add(voxelVector, new Voxel(state ? PrefabsManager.addedVoxelPrefab : PrefabsManager.voxelPrefab, voxelVector, ChunksDict[chunkVector].gameobject));
        }
    }

    public static void DeleteVoxel(Vector3 point)
    {
        Vector3 voxelVector = RoundToVoxel(point);
        Vector3 boxSize = new Vector3(PrefabsManager.voxelSize - 0.001f, PrefabsManager.voxelSize - 0.001f, PrefabsManager.voxelSize - 0.001f);
        if (Physics.CheckBox(voxelVector, boxSize / 2, Quaternion.identity, 1 << 3))
        {
            Vector3 chunkVector = RoundToChunk(voxelVector);
            Chunk chunk = ChunksDict[chunkVector];
            Voxel voxel = chunk.VoxelsDict[voxelVector];
            Destroy(voxel.gameobject);
            Instantiate(PrefabsManager.deletedVoxelPrefab, voxel.position, Quaternion.identity, PrefabsManager.deletedVoxelPrefabParent.transform);
            chunk.VoxelsDict.Remove(voxelVector);
        }
    }

    public static Vector3 RoundToVoxel(Vector3 point)
    {
        Vector3 roundedVector = new Vector3();
        roundedVector.Set(Mathf.RoundToInt(point.x / PrefabsManager.voxelSize) * PrefabsManager.voxelSize,
            Mathf.RoundToInt(point.y / PrefabsManager.voxelSize) * PrefabsManager.voxelSize,
            Mathf.RoundToInt(point.z / PrefabsManager.voxelSize) * PrefabsManager.voxelSize);
        return roundedVector;
    }

    public static Vector3 RoundToChunk(Vector3 point)
    {
        Vector3 roundedVector = new Vector3();
        roundedVector.Set(Mathf.RoundToInt(point.x / PrefabsManager.chunkSize) * PrefabsManager.chunkSize,
            Mathf.RoundToInt(point.y / PrefabsManager.chunkSize) * PrefabsManager.chunkSize,
            Mathf.RoundToInt(point.z / PrefabsManager.chunkSize) * PrefabsManager.chunkSize);
        return roundedVector;
    }

    public static void HideDeletedVoxels(bool state)
    {
        PrefabsManager.deletedVoxelPrefabParent.SetActive(state);
    }

    public static void HideVoxels(bool state)
    {
        PrefabsManager.chunkParentPrefab.SetActive(state);
    }
}