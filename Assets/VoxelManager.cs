using System.Collections;
using System;
using JetBrains.Annotations;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Examples.Demos;
using Microsoft.MixedReality.Toolkit.UI;
using Unity.VisualScripting;

public class VoxelManager : MonoBehaviour
{
    
    
    public float ChunkSize = 3f;
    public static float chunkSize;
    
    static Dictionary<Vector3, Chunk> ChunksDict = new Dictionary<Vector3, Chunk>();
    static Dictionary<Vector3, byte[]> VoxelByteDict = new();

    void Start()
    {
        chunkSize = ChunkSize;
    }

    public static void AddVoxel(Vector3 randomVector, bool humanEdited = false)
    {

        Vector3 voxelVector = RoundToVoxel(randomVector);
        Vector3 chunkVector = RoundToChunk(voxelVector);
        
        if (!ChunksDict.ContainsKey(chunkVector))
        {
            ChunksDict.Add(chunkVector, new Chunk(voxelVector));
        }
        
        Chunk tempChunk = ChunksDict[chunkVector];
        
        if(!tempChunk.VoxelsDict.ContainsKey(voxelVector))
        {
            tempChunk.VoxelsDict.Add(voxelVector, new Voxel(voxelVector, humanEdited));
        }
        else
        {
            Voxel tempVoxel = tempChunk.VoxelsDict[voxelVector];
            tempVoxel.IncreaseProba(humanEdited);
        }
    }
    public static void RemoveVoxel(Vector3 RandomVector, bool humanEdited = false)
    {

        Vector3 chunkVector = RoundToChunk(RandomVector);
        //Vector3 voxelVector = RoundToVoxel(RandomVector);
        ChunksDict[chunkVector].VoxelsDict[RandomVector].DecrementProba(humanEdited);
        
    }

    public static Vector3 RoundToChunk(Vector3 v)
    {
        Vector3 roundedVector = new Vector3();
        roundedVector.Set(Mathf.RoundToInt(v.x / chunkSize) * chunkSize,
                          Mathf.RoundToInt(v.y / chunkSize) * chunkSize,
                          Mathf.RoundToInt(v.z / chunkSize) * chunkSize);
        return roundedVector;

    }
    public static Vector3 RoundToVoxel(Vector3 v)
    {
        Vector3 roundedVector = new Vector3();
        roundedVector.Set(Mathf.RoundToInt(v.x / PrefabsManager.voxelSize) * PrefabsManager.voxelSize,
                          Mathf.RoundToInt(v.y / PrefabsManager.voxelSize) * PrefabsManager.voxelSize,
                          Mathf.RoundToInt(v.z / PrefabsManager.voxelSize) * PrefabsManager.voxelSize);
        return roundedVector;
    }

}