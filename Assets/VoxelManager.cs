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

    public static void AddVoxel(Vector3 randomVector, bool humanEdited = false)
    {

        Vector3 voxelVector = RoundToVoxel(randomVector);
        Vector3 chunkVector = RoundToChunk(voxelVector);
        
        if (!ChunksDict.ContainsKey(chunkVector))
        {
            ChunksDict.Add(chunkVector, new Chunk(voxelVector));
            Debug.Log("Added new chunk");
            Debug.Log("Added new voxel");
        }
        
        Chunk tempChunk = ChunksDict[chunkVector];
        
        if(!tempChunk.VoxelsDict.ContainsKey(voxelVector))
        {
            tempChunk.VoxelsDict.Add(voxelVector, new Voxel(voxelVector, humanEdited));
            Debug.Log("Added new voxel");
        }
        else
        {
            Voxel tempVoxel = tempChunk.VoxelsDict[voxelVector];
            tempVoxel.IncreaseProba(humanEdited);
            //Debug.Log("Increased");
        }
    }
    public static void RemoveVoxel(Vector3 RandomVector, bool humanEdited = false)
    {

        Vector3 chunkVector = RoundToChunk(RandomVector);
        //Vector3 voxelVector = RoundToVoxel(RandomVector);
        if (!humanEdited)
        {
            ChunksDict[chunkVector].VoxelsDict[RandomVector].DecrementProba();
        }
        else
        {
            if (!ChunksDict.ContainsKey(chunkVector))
            {
                ChunksDict.Add(chunkVector, new Chunk(RandomVector));
            }
        
            Chunk tempChunk = ChunksDict[chunkVector];
            
            if(!tempChunk.VoxelsDict.ContainsKey(RandomVector))
            {
                tempChunk.VoxelsDict.Add(RandomVector, new Voxel(RandomVector));
            }

            Voxel tempVoxel = tempChunk.VoxelsDict[RandomVector];
            tempVoxel.DecrementProba(humanEdited);
            
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