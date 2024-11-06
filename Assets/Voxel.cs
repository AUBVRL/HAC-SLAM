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

}
