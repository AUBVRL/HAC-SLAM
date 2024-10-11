using UnityEngine;
using System;
using System.Collections.Generic;

public class Voxel
{
    public Vector3 Position;
    private float Proba;
    public bool State;
    public byte[] PoseInBytes;

    public GameObject prefab;

    private void Converter() 
    {
        PoseInBytes = BitConverter.GetBytes(Position.x);
    }
    
    public Voxel(Vector3 vecto)
    {
        Position = vecto;
        Proba = 0.15f;
        State = false;
    }

    public void IncreaseProba()
    {
        if (Proba <= 0.75 && Proba >= 0) Proba += 0.25f;
        if (!State && Proba >= 0.6f) create();
    }

    public void DecrementProba()
    {
        int layerMask = 1 << 31;
        bool checkBoxOverlap = Physics.CheckBox(Position, prefab.transform.localScale, Quaternion.identity, layerMask);
        if(!checkBoxOverlap)
        {
            Debug.Log("Removing");
            if (Proba <= 1 && Proba >= 0.3) Proba -= 0.3f;
            if (State && Proba < 0.6f) destroy();
        }
        
    }

    private void create()
    {
        prefab = UnityEngine.Object.Instantiate(VoxelManager.staticPrefab, Position, Quaternion.identity,VoxelManager.staticPrefabParent.transform);
        State = true;
    }

    private void destroy()
    {
        UnityEngine.Object.Destroy(prefab);
        State = false;
    }

    public byte[] ToByteArray()
    {
        if(!State)
        {
            return null;
        }
        List<byte> byteList = new List<byte>();

        // Convert position to bytes
        byteList.AddRange(BitConverter.GetBytes(Position.x));
        byteList.AddRange(BitConverter.GetBytes(Position.y));
        byteList.AddRange(BitConverter.GetBytes(Position.z));

        return byteList.ToArray();
    }

    public void AddVoxel()
    {

    }

    public void DeleteVoxel()
    {

    }

    public void LabelVoxel()
    {

    }
}