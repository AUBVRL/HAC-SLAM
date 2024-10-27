using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleVoxelParents : MonoBehaviour
{

    public void ToggleMappedVoxelParent(bool state)
    {
        PrefabsManager.voxelPrefabParent.SetActive(state);
    }

    public void ToggleAddedVoxelParent(bool state)
    {
        PrefabsManager.addedVoxelPrefabParent.SetActive(state);
    }

    public void ToggleDeletedVoxelParent(bool state)
    {
        PrefabsManager.deletedVoxelPrefabParent.SetActive(state);
    }

}
