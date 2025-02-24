using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UIElements;

public class Voxel
{
    public GameObject prefab;
    GameObject voxelParent;
    public Vector3 position;
    float probability;
    public bool state;
    public byte[] positionInBytes;


    public Voxel(Vector3 vector, GameObject chunkParent, bool humanEdited = false)
    {
        position = vector;
        voxelParent = chunkParent;
        if (humanEdited)
        {
            IncreaseProba(humanEdited);
        }
        else
        {
            probability = 0.15f;
            state = false;
        }

    }

    public void IncreaseProba(bool humanEdited = false)
    {
        if (!humanEdited)
        {
            if (probability < 0.75 && probability >= 0) probability += 0.25f;
            if (!state && probability >= 0.6f) create();
        }
        else
        {
            probability = 2f;
            create(humanEdited);
        }
    }

    public void DecrementProba(bool humanEdited = false)
    {
        if (!humanEdited)
        {
            int layerMask = 1 << 31;
            bool checkBoxOverlap = Physics.CheckBox(position, prefab.transform.localScale, Quaternion.identity, layerMask);
            if (!checkBoxOverlap)
            {
                if (probability <= 1 && probability > 0.3) probability -= 0.3f;
                if (state && probability < 0.6f) destroy();
            }
        }
        else
        {
            probability = -2;
            destroy(humanEdited);
        }
    }



    private void create(bool humanEdited = false)
    {
        if (!humanEdited)
        {
            prefab = UnityEngine.Object.Instantiate(PrefabsManager.voxelPrefab, position, Quaternion.identity, voxelParent.transform);
        }
        else
        {
            if (prefab != null) UnityEngine.Object.Destroy(prefab);
            prefab = UnityEngine.Object.Instantiate(PrefabsManager.addedVoxelPrefab, position, Quaternion.identity, voxelParent.transform);
        }
        state = true;
    }

    private void destroy(bool humanEdited = false)
    {
        if (!humanEdited)
        {
            UnityEngine.Object.Destroy(prefab);
        }
        else
        {
            if (prefab != null) UnityEngine.Object.Destroy(prefab);
            prefab = UnityEngine.Object.Instantiate(PrefabsManager.deletedVoxelPrefab, position, Quaternion.identity, PrefabsManager.deletedVoxelPrefabParent.transform);
        }
        state = false;
    }

    public List<byte> ToByteArray()
    {
        if (!state)
        {
            return new List<byte>();
        }
        List<byte> byteList = new();

        // Convert position to bytes
        byteList.AddRange(BitConverter.GetBytes(position.x));
        byteList.AddRange(BitConverter.GetBytes(position.z));
        byteList.AddRange(BitConverter.GetBytes(position.y));

        return byteList;
    }


}