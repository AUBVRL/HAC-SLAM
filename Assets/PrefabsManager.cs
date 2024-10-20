using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabsManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject VoxelPrefab, AddedVoxelPrefab, DeletedVoxelPrefab;
    public static GameObject voxelPrefab, addedVoxelPrefab, deletedVoxelPrefab, voxelPrefabParent, addedVoxelPrefabParent, deletedVoxelPrefabParent;
    public float VoxelSize = 0.05f;
    public static float voxelSize;
    
  
    // GameObjects for the selector prefabs
    public enum ShapeType {Cube, Sphere, Cylinder};
    public GameObject CubeSelector;
    public GameObject SphereSelector;
    public GameObject CylinderSelector;
    public static GameObject SelectorPrefab;

    
    void Start()
    {
        voxelSize = VoxelSize;
        
        voxelPrefab = VoxelPrefab;
        addedVoxelPrefab = AddedVoxelPrefab;
        deletedVoxelPrefab = DeletedVoxelPrefab;
        
        voxelPrefabParent = new GameObject("VoxelParent");
        addedVoxelPrefabParent = new GameObject("AddedVoxelParent");
        deletedVoxelPrefabParent = new GameObject("DeletedVoxelParent");
        voxelPrefab.transform.localScale = new Vector3(voxelSize, voxelSize, voxelSize);

        SelectorPrefab = CubeSelector;
    }

    public void SelectObject(int shape)
    {
        ShapeType shapeType = (ShapeType)shape;
        switch (shapeType)
        {
            case ShapeType.Cube:
                SelectorPrefab = CubeSelector;
                break;
            case ShapeType.Sphere:
                SelectorPrefab = SphereSelector;
                break;
            case ShapeType.Cylinder:
                SelectorPrefab = CylinderSelector;
                break;
        }
    }

    public void OnConvexityToggle(bool convex)
    {
        SelectorPrefab.GetComponent<MeshCollider>().convex = convex;
    }


}
