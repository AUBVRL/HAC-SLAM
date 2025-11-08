using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Animations;
using UnityEngine;

public class PrefabsManager : MonoBehaviour
{
    public GameObject VoxelPrefab, AddedVoxelPrefab, DeletedVoxelPrefab; // point cloud, user addition, user deletion
    public static GameObject voxelPrefab, addedVoxelPrefab, deletedVoxelPrefab;
    public static GameObject deletedVoxelPrefabParent;
    public static GameObject chunkPrefab;
    public static GameObject chunkParentPrefab;
    float VoxelSize;
    public static float voxelSize;
    float ChunkSize;
    public static float chunkSize;

    public enum ShapeType { Cube, Sphere, Cylinder }; 
    public GameObject CubeSelector;
    public GameObject SphereSelector;
    public GameObject CylinderSelector;
    public static GameObject Selector;
    
    public enum AssetType { iwHub, chargingStation, dollyOne, dollyTwo, palletDocker};
    public GameObject iwHub;
    public GameObject chargingStation;
    public GameObject dollyOne;
    public GameObject dollyTwo;
    public GameObject palletDocker;
    public static GameObject Asset;
    public GameObject VRL;
    public static GameObject vrl;

    public GameObject ImageTarget;
    public static GameObject imageTarget;
    static bool Convexity = false;

    private void Start()
    {
        VoxelSize = 0.05f;
        ChunkSize = 10f;
        voxelPrefab = VoxelPrefab;
        addedVoxelPrefab = AddedVoxelPrefab;
        deletedVoxelPrefab = DeletedVoxelPrefab;
        voxelPrefab.transform.localScale = Vector3.one * VoxelSize;
        addedVoxelPrefab.transform.localScale = Vector3.one * VoxelSize;
        deletedVoxelPrefab.transform.localScale = Vector3.one * VoxelSize;
        voxelSize = VoxelSize;
        chunkSize = ChunkSize;
        deletedVoxelPrefabParent = new GameObject("DeletedVoxelParent");
        deletedVoxelPrefabParent.SetActive(false);
        chunkPrefab = new GameObject("Chunk");
        chunkParentPrefab = new GameObject("chunkParent");
        Selector = CubeSelector;
        imageTarget = ImageTarget;
        //imageTarget.transform.SetPositionAndRotation(ImageTarget.transform.position, ImageTarget.transform.rotation);
        //SaveVRL();
    }

    public void UpdateImageTargetTransform()
    {

        imageTarget.transform.SetPositionAndRotation(ImageTarget.transform.position, Quaternion.Euler(0, ImageTarget.transform.eulerAngles.y, 0));
        voxelPrefab.transform.rotation = Quaternion.Euler(new Vector3(0,imageTarget.transform.eulerAngles.y,0));
        addedVoxelPrefab.transform.forward = voxelPrefab.transform.forward;
        deletedVoxelPrefab.transform.forward= voxelPrefab.transform.forward;
        Selector.transform.forward = voxelPrefab.transform.forward;
    }

    public void SelectAsset(int shape)
    {
        AssetType assetType = (AssetType)shape;
        switch (assetType)
        {
            case AssetType.iwHub:
                Asset = iwHub;
                break;
            case AssetType.chargingStation:
                Asset = chargingStation;
                break;
            case AssetType.dollyOne:
                Asset = dollyOne;
                break;
            case AssetType.dollyTwo:  
                Asset = dollyTwo;
                break;
            case AssetType.palletDocker:
                Asset = palletDocker;
                break;
        }
    }

    public void SelectObject(int shape)
    {
        ShapeType shapeType = (ShapeType)shape;
        switch (shapeType)
        {
            case ShapeType.Cube:
                Selector = CubeSelector;
                break;
            case ShapeType.Sphere:
                Selector = SphereSelector;
                break;
            case ShapeType.Cylinder:
                Selector = CylinderSelector;
                break;
        }
        Selector.GetComponent<MeshCollider>().convex = Convexity;
    }

    public static void SelectorOnConvexityToggle(bool convex)
    {
        Convexity = convex;
        Selector.GetComponent<MeshCollider>().convex = convex;
    }

    public void AssetOnConvexityToggle(bool convex)
    {
        Asset.GetComponent<MeshCollider>().convex = convex;
    }

    public void SetChunkParent(bool state)
    {
        chunkParentPrefab.SetActive(state);
    }

    //public static void SaveVRL()
    //{
    //    foreach (Transform chunk in vrl.transform)
    //    {
    //        foreach (Transform voxel in chunk)
    //        {
    //            // Do something with grandChild
    //            VoxelManager.AddVoxel(voxel.transform.position, true);
    //        }
    //    }
    //    Debug.Log("DONE!!!!");
    //}

}
