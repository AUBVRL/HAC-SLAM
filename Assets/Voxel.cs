using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Voxel
{
    public GameObject gameobject;
    public Vector3 position;
    public byte[] positionInBytes;


    public Voxel(GameObject prefab, Vector3 voxelPosition, GameObject chunkParent)
    {
        position = voxelPosition;
        gameobject = UnityEngine.Object.Instantiate(prefab, voxelPosition, Quaternion.identity, chunkParent.transform);
    }


    public List<byte> ToByteArray()
    {
        List<byte> byteList = new();
        // Convert position to bytes
        byteList.AddRange(BitConverter.GetBytes(position.x));
        byteList.AddRange(BitConverter.GetBytes(position.z));
        byteList.AddRange(BitConverter.GetBytes(position.y));
        return byteList;
    }


}