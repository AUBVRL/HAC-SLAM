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
    public enum ShapeType { Cube, Sphere, Cylinder };
    public GameObject CubeSelector;
    public GameObject SphereSelector;
    public GameObject CylinderSelector;
    public static GameObject SelectorPrefab;

    public GameObject ImageTarget;
    public static GameObject imageTarget;

    public enum AssetType { chargingStation, KLT };
    public GameObject chargingStation, KLT;
    public static GameObject Asset;

    void Start()
    {
        voxelSize = VoxelSize;

        voxelPrefab = VoxelPrefab;
        addedVoxelPrefab = AddedVoxelPrefab;
        deletedVoxelPrefab = DeletedVoxelPrefab;

        voxelPrefabParent = new GameObject("VoxelParent");
        addedVoxelPrefabParent = new GameObject("AddedVoxelParent");
        deletedVoxelPrefabParent = new GameObject("DeletedVoxelParent");
        deletedVoxelPrefabParent.SetActive(false);
        voxelPrefab.transform.localScale = new Vector3(voxelSize, voxelSize, voxelSize);

        SelectorPrefab = CubeSelector;
        imageTarget = ImageTarget;
    }

    public void UpdateImageTargetTransform()
    {
        imageTarget.transform.SetPositionAndRotation(ImageTarget.transform.position, Quaternion.Euler(0, ImageTarget.transform.eulerAngles.y, 0));
        voxelPrefab.transform.rotation = Quaternion.Euler(new Vector3(0, imageTarget.transform.eulerAngles.y, 0));
        addedVoxelPrefab.transform.forward = voxelPrefab.transform.forward;
        deletedVoxelPrefab.transform.forward = voxelPrefab.transform.forward;
        SelectorPrefab.transform.forward = voxelPrefab.transform.forward;
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

    public void SelectAsset(int shape)
    {
        AssetType assetType = (AssetType)shape;
        switch (assetType)
        {
            case AssetType.chargingStation:
                Asset = chargingStation;
                break;
            case AssetType.KLT:
                Asset = KLT;
                break;
        }
    }

}
