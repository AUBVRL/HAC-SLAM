using System.Collections;
using System;
using JetBrains.Annotations;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Examples.Demos;
using Microsoft.MixedReality.Toolkit.UI;
using Unity.VisualScripting;

public class VoxelManager : MonoBehaviour
{
    
    
    public float ChunkSize = 3f;
    public static float chunkSize;
    
    public Vector2Int rayCastArray = new Vector2Int(20, 20);
    List<Vector3> VoxelsPosition = new List<Vector3>();
    //public static List<Chunk> Chunks = new List<Chunk>();
    public static Dictionary<Vector3, Chunk> ChunksDict = new Dictionary<Vector3, Chunk>();
    // Start is called before the first frame update
    //MinecraftBuilder mini = new MinecraftBuilder();
    RaycastHit hit;
    
    public static Dictionary<Vector3, byte[]> VoxelByteDict = new();

    void Awake()
    {
        chunkSize = ChunkSize;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        VoxelsPosition = MeshToPointCloudParallel(); //MeshToPointCloudParallel();

        foreach( Vector3 v in VoxelsPosition)
        {
            AddVoxel(v);
        }
       
    }

    public static void AddVoxel(Vector3 RandomVector)
    {
        //Vector3 chunkVector, voxelVector;
        Chunk tempChunk;
        Voxel tempVoxel;
        //Vector3 RandomVector = new Vector3(1,2,3);
        Vector3 voxelVector = RoundToVoxel(RandomVector);
        Vector3 chunkVector = RoundToChunk(voxelVector);
        
        if (!ChunksDict.ContainsKey(chunkVector))
        {
            ChunksDict.Add(chunkVector, new Chunk(voxelVector));
        }
        
        tempChunk = ChunksDict[chunkVector];
        
        if(!tempChunk.VoxelsDict.ContainsKey(voxelVector))
        {
            tempChunk.VoxelsDict.Add(voxelVector, new Voxel(voxelVector));
        }
        else
        {
            tempVoxel = tempChunk.VoxelsDict[voxelVector];
            tempVoxel.IncreaseProba();
        }
        

    }

    public void RemoveVoxel(Vector3 RandomVector)
    {

        Vector3 chunkVector = RoundToChunk(RandomVector);
        //Vector3 voxelVector = RoundToVoxel(RandomVector);
        ChunksDict[chunkVector].VoxelsDict[RandomVector].DecrementProba();
        
    }

    public static Vector3 RoundToChunk(Vector3 v)
    {
        Vector3 roundedVector = new Vector3();
        roundedVector.Set(Mathf.RoundToInt(v.x / chunkSize) * chunkSize,
                          Mathf.RoundToInt(v.y / chunkSize) * chunkSize,
                          Mathf.RoundToInt(v.z / chunkSize) * chunkSize);
        return roundedVector;

    }

    public static Vector3 RoundToVoxel(Vector3 v)
    {
        Vector3 roundedVector = new Vector3();
        roundedVector.Set(Mathf.RoundToInt(v.x / PrefabsManager.voxelSize) * PrefabsManager.voxelSize,
                          Mathf.RoundToInt(v.y / PrefabsManager.voxelSize) * PrefabsManager.voxelSize,
                          Mathf.RoundToInt(v.z / PrefabsManager.voxelSize) * PrefabsManager.voxelSize);
        return roundedVector;
    }

    public List<Vector3> MeshToPointCloudBeam()
    {
        List<Vector3> meshPoints = new List<Vector3>();
        Vector3 Gaze_direction = Camera.main.transform.forward;
        Vector3 Gaze_position = Camera.main.transform.position;
        
        int layerMask = 1 << 31;
        for (int i = 0; i < (MinecraftBuilder.Hor_angle_window / MinecraftBuilder.angle_size); i++)
        {
            Vector3 Hor_Ray_direction = Quaternion.Euler(0, (MinecraftBuilder.Hor_angle_min + (MinecraftBuilder.angle_size * i)), 0) * Gaze_direction;
            for (int j = 0; j < (int)(MinecraftBuilder.Ver_angle_window / MinecraftBuilder.angle_size); j++)
            {
                Vector3 Ver_Ray_direction = Quaternion.Euler((MinecraftBuilder.Ver_angle_min + (MinecraftBuilder.angle_size * j)), 0, 0) * Hor_Ray_direction;
                bool raycastHit = Physics.Raycast(Gaze_position, Ver_Ray_direction, out hit, 10f, layerMask);
                if (raycastHit)
                    {
                        meshPoints.Add(hit.point);
                    }
            }
        }
        return meshPoints;
    }

    public List<Vector3> MeshToPointCloudParallel()
    {
        List<Vector3> meshPoints = new List<Vector3>();
        Vector3 Gaze_direction = Camera.main.transform.forward;
        Vector3 Gaze_position = Camera.main.transform.position;

        int layerMask = 1 << 31;
        int layerMask2 = 1 << 7;
        for (int i = -rayCastArray.x /2; i < rayCastArray.x/2 ; i++)
        {
            Vector3 xGazeposition = Gaze_position + new Vector3(i*PrefabsManager.voxelSize , 0, 0);
            for (int j = -rayCastArray.y / 2; j < rayCastArray.y / 2; j++)
            {
                Vector3 newGazeposition = xGazeposition + new Vector3(0, j * PrefabsManager.voxelSize, 0);
                bool raycastHit = Physics.Raycast(newGazeposition, Gaze_direction, out hit, 10f, layerMask | layerMask2);
                if (raycastHit && hit.transform.gameObject.layer == 31)
                {
                    meshPoints.Add(hit.point);
                }

            }
        }
        return meshPoints;
    }

    public List<Vector3> StateCheckerParallel()
    {
        List<Vector3> meshPoints = new List<Vector3>();
        Vector3 Gaze_direction = Camera.main.transform.forward;
        Vector3 Gaze_position = Camera.main.transform.position;

        int layerMask = 1 << 7;
        for (int i = -rayCastArray.x / 2; i < rayCastArray.x / 2; i++)
        {
            Vector3 xGazeposition = Gaze_position + new Vector3(i * PrefabsManager.voxelSize, 0, 0);
            for (int j = -rayCastArray.y / 2; j < rayCastArray.y / 2; j++)
            {
                Vector3 newGazeposition = xGazeposition + new Vector3(0, j * PrefabsManager.voxelSize, 0);
                bool raycastHit = Physics.Raycast(newGazeposition, Gaze_direction, out hit, 10f, layerMask);
                if (raycastHit)
                {
                    meshPoints.Add(hit.transform.position);
                }
                
            }
        }
        return meshPoints;
    }
    
    void VoxelCleanse() 
    {
        VoxelsPosition = StateCheckerParallel();
        foreach(Vector3 v in VoxelsPosition)
        {   
            RemoveVoxel(v);
        }
    }

    void OnDisable()
    {
        CancelInvoke("VoxelCleanse");
    }

    void OnEnable()
    {
        InvokeRepeating("VoxelCleanse", 0, 1);
    }
}