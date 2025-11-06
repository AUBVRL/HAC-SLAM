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
    
    public static Dictionary<Vector3, Chunk> ChunksDict = new();
    static Dictionary<Vector3, byte[]> VoxelByteDict = new();

    void Start()
    {
        chunkSize = ChunkSize;
    }

    public static void AddVoxel(Vector3 randomVector, bool humanAdded = false)
    {
        Vector3 voxelVector = RoundToVoxel(randomVector);
        Vector3 chunkVector = RoundToChunk(voxelVector);
        
        if (!ChunksDict.ContainsKey(chunkVector))
        {
            ChunksDict.Add(chunkVector, new Chunk(voxelVector)); // voxelVector is not a typo
        }
        
        Chunk tempChunk = ChunksDict[chunkVector];
        
        if(!tempChunk.VoxelsDict.ContainsKey(voxelVector))
        {
            tempChunk.AddVoxel(voxelVector, humanAdded);
            //Debug.Log("Added new voxel");
        }
        // else
        // {
        //     Voxel tempVoxel = tempChunk.VoxelsDict[voxelVector];
        //     tempVoxel.create(humanAdded);
        //     //Debug.Log("Increased");
        // }
    }
    public static void RemoveVoxel(Vector3 RandomVector)
    {
        Vector3 chunkVector = RoundToChunk(RandomVector);
        //Vector3 voxelVector = RoundToVoxel(RandomVector);
        if (ChunksDict.ContainsKey(chunkVector))
        {
            if(ChunksDict[chunkVector].VoxelsDict.ContainsKey(RandomVector))
            {
                ChunksDict[chunkVector].VoxelsDict[RandomVector].destroy();
            }
        }
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