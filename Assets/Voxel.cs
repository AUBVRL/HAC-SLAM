using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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




}
