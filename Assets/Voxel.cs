using UnityEngine;
using System;
using System.Collections.Generic;

public class Voxel
{
    public Vector3 Position;

    public byte[] PoseInBytes;
    public GameObject prefab;
    GameObject voxelParent;
    
    public Voxel(Vector3 vecto, GameObject parent,bool humanAdded = false)
    {
        Position = vecto;
        voxelParent = parent;
        create(humanAdded);
    }
    
    public void create(bool humanAddition = false)
    {
        if(!humanAddition)
        {
            prefab = UnityEngine.Object.Instantiate(PrefabsManager.voxelPrefab, 
                                                    Position, 
                                                    Quaternion.identity,
                                                    voxelParent.transform);
        }
        else
        {
            if (prefab != null) UnityEngine.Object.Destroy(prefab);
            prefab = UnityEngine.Object.Instantiate(PrefabsManager.addedVoxelPrefab, 
                                                    Position, 
                                                    Quaternion.identity,
                                                    voxelParent.transform);

        }
    }

    public void destroy()
    {
        if (prefab != null) UnityEngine.Object.Destroy(prefab);
        prefab = UnityEngine.Object.Instantiate(PrefabsManager.deletedVoxelPrefab, 
                                                Position, 
                                                Quaternion.identity,
                                                voxelParent.transform);
        prefab.SetActive(false);
    }

    public List<byte> ToByteArray()
    {
        List<byte> byteList = new();

        // Convert position to bytes
        byteList.AddRange(BitConverter.GetBytes(Position.x));
        byteList.AddRange(BitConverter.GetBytes(Position.z));
        byteList.AddRange(BitConverter.GetBytes(Position.y));

        return byteList;
    }

}