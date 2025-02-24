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

    public static void AddVoxel(Vector3 point, bool humanEdited = false)
    {
        Vector3 voxelVector = RoundToVoxel(point);
        Vector3 chunkVector = RoundToChunk(voxelVector);

        if (!ChunksDict.ContainsKey(chunkVector))
        {
            ChunksDict.Add(chunkVector, new Chunk(voxelVector));
        }

        if (!ChunksDict[chunkVector].VoxelsDict.ContainsKey(voxelVector))
        {
            ChunksDict[chunkVector].AddVoxel(voxelVector, humanEdited);
        }
        else
        {
            ChunksDict[chunkVector].VoxelsDict[voxelVector].IncreaseProba(humanEdited);
        }
    }

    public static void RemoveVoxel(Vector3 point, bool humanEdited = false)
    {

        Vector3 chunkVector = RoundToChunk(point);
        if (!humanEdited)
        {
            ChunksDict[chunkVector].VoxelsDict[point].DecrementProba();
        }
        else
        {
            if (!ChunksDict.ContainsKey(chunkVector))
            {
                ChunksDict.Add(chunkVector, new Chunk(point));
            }

            Chunk tempChunk = ChunksDict[chunkVector];

            if (!tempChunk.VoxelsDict.ContainsKey(point))
            {
                tempChunk.AddVoxel(point);
            }

            Voxel tempVoxel = tempChunk.VoxelsDict[point];
            tempVoxel.DecrementProba(humanEdited);

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