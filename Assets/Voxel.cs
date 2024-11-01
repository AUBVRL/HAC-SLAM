using UnityEngine;
using System;
using System.Collections.Generic;

public class Voxel
{
    public Vector3 Position;
    float Proba;
    public bool State;
    public byte[] PoseInBytes;

    public GameObject prefab;
    GameObject voxelParent;
    private void Converter() 
    {
        PoseInBytes = BitConverter.GetBytes(Position.x);
    }
    
    public Voxel(Vector3 vecto, GameObject parent, bool humanEdited = false)
    {
        Position = vecto;
        voxelParent = parent;
        if(humanEdited)
        {
            IncreaseProba(humanEdited);
        } 
        else
        {
            Proba = 0.15f;
            State = false;
        }
        
    }

    public void IncreaseProba(bool humanEdited = false)
    {
        if (!humanEdited)
        {
            if (Proba < 0.75 && Proba >= 0) Proba += 0.25f;
            if (!State && Proba >= 0.6f ) create();
        }
        else
        {
            Proba = 2;
            create(humanEdited);
            //Debug.Log("Adding");
        }
        
    }

    public void DecrementProba(bool humanEdited = false)
    {
        if(!humanEdited)
        {
            int layerMask = 1 << 31;
            bool checkBoxOverlap = Physics.CheckBox(Position, prefab.transform.localScale, Quaternion.identity, layerMask);
            if(!checkBoxOverlap)
            {
                Debug.Log("Removing");
                if (Proba <= 1 && Proba > 0.3) Proba -= 0.3f;
                if (State && Proba < 0.6f) destroy();
            }
        }
        else
        {
            Proba = -2;
            destroy(humanEdited);
        }
        
        
    }

    private void create(bool humanEdited = false)
    {
        if(!humanEdited)
        {
            prefab = UnityEngine.Object.Instantiate(PrefabsManager.voxelPrefab, Position, Quaternion.identity,voxelParent.transform);
        }
        else
        {
            if (prefab != null) UnityEngine.Object.Destroy(prefab);
            prefab = UnityEngine.Object.Instantiate(PrefabsManager.addedVoxelPrefab, Position, Quaternion.identity,voxelParent.transform);

        }
        State = true;
        
    }

    private void destroy(bool humanEdited = false)
    {
        if(!humanEdited)
        {
            UnityEngine.Object.Destroy(prefab);
        }
        else
        {
            if (prefab != null) UnityEngine.Object.Destroy(prefab);
            prefab = UnityEngine.Object.Instantiate(PrefabsManager.deletedVoxelPrefab, Position, Quaternion.identity,PrefabsManager.deletedVoxelPrefabParent.transform);     
        }

        State = false; 
    }

    public List<byte> ToByteArray()
    {
        if(!State)
        {
            Debug.Log("Empty");
            return new List<byte>();
            
        }
        List<byte> byteList = new();

        // Convert position to bytes
        byteList.AddRange(BitConverter.GetBytes(Position.x));
        byteList.AddRange(BitConverter.GetBytes(Position.z));
        byteList.AddRange(BitConverter.GetBytes(Position.y));

        return byteList;
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