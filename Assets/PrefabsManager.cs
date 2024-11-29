using System.Collections;
using System.Collections.Generic;
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

    public enum ShapeType {Cube, Sphere, Cylinder}; // what about Assets?
    public GameObject CubeSelector;
    public GameObject SphereSelector;
    public GameObject CylinderSelector;
    public static GameObject Selector;

    private void Start()
    {
        VoxelSize = 0.1f;
        ChunkSize = 3f;
        voxelPrefab = VoxelPrefab;
        addedVoxelPrefab = AddedVoxelPrefab;
        deletedVoxelPrefab = DeletedVoxelPrefab;
        voxelSize = VoxelSize;
        chunkSize = ChunkSize;
        deletedVoxelPrefabParent = new GameObject("DeletedVoxelParent");
        deletedVoxelPrefabParent.SetActive(false);
        chunkPrefab = new GameObject("Chunk");
        chunkParentPrefab = new GameObject("chunkParent");
        Selector = CubeSelector; 
    }
}
