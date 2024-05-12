using System.Collections;
using System;
using JetBrains.Annotations;
using System.Linq;
//using System.Numerics;
using UnityEngine;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Examples.Demos;

public class VoxelManager : MonoBehaviour
{
    [SerializeField]
    public GameObject cube1;
    float chunkSize = 3f;
    float voxelSize = 0.05f;
    List<Vector3> VoxeletPosition = new List<Vector3>();
    public static List<Chunk> Chunks = new List<Chunk>();
    public static Dictionary<Vector3, Chunk> ChunksDict = new Dictionary<Vector3, Chunk>();
    // Start is called before the first frame update
    //MinecraftBuilder mini = new MinecraftBuilder();
    RaycastHit hit;
    void Start()
    {
        Voxel.cubecube = cube1;
        Voxel voxilaya = new Voxel(new Vector3(1,0,0));
        Voxel voxilaya2 = new Voxel(new Vector3(1,0,0));
        Vector3 vocto = new Vector3(0,0,0);
        Vector3 vocto2 = new Vector3(0,0,1);
        Vector3 vocto3 = new Vector3(0,0,1);

        

        Chunk chik;
        chik = Chunks.Find(x => x.Position == vocto);
        if (chik == null)
        {
            Debug.Log("Kousa");
        }
        
        Chunks.Add(new Chunk(new Vector3(2,2,2)));
        VoxeletPosition.Add(vocto);
        VoxeletPosition.Add(vocto2);
        //Voxel voxilaya3 = new Voxel(new Vector3(2,0,0));
        //Monazzim.Voxelet.Add(voxilaya);
        //Voxelet.Add(voxilaya2);
        //Debug.Log("Yes " + Monazzim.Voxelet.Any(x => x.Position == voxilaya2.Position));
        //Debug.Log("Yes " + VoxeletPosition.Contains(vocto3));
        //Debug.Log(Chunks[1].Voxels.Find(x => x.Position == voxilaya2.Position));
    }

    // Update is called once per frame
    void Update()
    {
        VoxeletPosition = MeshToPointCloud();

        foreach( Vector3 v in VoxeletPosition)
        {
            Manager(v);
        }
    }

    public void Manager(Vector3 RandomVector)
    {
        Vector3 chunkVector, voxelVector;
        Chunk tempChunk;
        Voxel tempVoxel;
        //Vector3 RandomVector = new Vector3(1,2,3);
        chunkVector = RoundToChunk(RandomVector);
        voxelVector = RoundToVoxel(RandomVector);
        if (!ChunksDict.ContainsKey(chunkVector))
        {
            ChunksDict.Add(chunkVector, new Chunk(voxelVector));
        }
        
        tempChunk = ChunksDict[chunkVector];
        
        if(!tempChunk.VoxelsDict.ContainsKey(voxelVector))
        {
            tempChunk.VoxelsDict.Add(voxelVector, new Voxel(voxelVector));
        }
        
        tempVoxel = tempChunk.VoxelsDict[voxelVector];
        tempVoxel.IncreaseProba();

    }

    public Vector3 RoundToChunk(Vector3 v)
    {
        Vector3 roundedVector = new Vector3();
        roundedVector.Set(Mathf.RoundToInt(v.x / chunkSize),
                          Mathf.RoundToInt(v.y / chunkSize),
                          Mathf.RoundToInt(v.z / chunkSize));
        return roundedVector;
    }

    public Vector3 RoundToVoxel(Vector3 v)
    {
        Vector3 roundedVector = new Vector3();
        roundedVector.Set(Mathf.RoundToInt(v.x / voxelSize) * voxelSize,
                          Mathf.RoundToInt(v.y / voxelSize) * voxelSize,
                          Mathf.RoundToInt(v.z / voxelSize) * voxelSize);
        return roundedVector;
    }

    public List<Vector3> MeshToPointCloud()
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
}

public class Voxel
{
    public Vector3 Position;
    private float Proba;
    public bool State;
    public byte[] PoseInBytes;

    public static GameObject cubecube;

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
        if (Proba <= 0.85 && Proba >= 0) Proba += 0.15f;
        if (!State && Proba >= 0.6f) create();
        
    }

    public void DecrementProba()
    {
        if (Proba <= 1 && Proba >= 0.01) Proba -= 0.01f;
        if (State && Proba < 0.6f) destroy();
    }

    private void create()
    {
        UnityEngine.Object.Instantiate(cubecube, Position, Quaternion.identity);
        State = true;
    }

    private void destroy()
    {

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

public class Monazzim 
{
    public static List<Voxel> Voxelet = new List<Voxel>();
}

public class Chunk
{
    public Vector3 Position;

    public List<Voxel> Voxels = new List<Voxel>();
    public Dictionary<Vector3, Voxel> VoxelsDict = new Dictionary<Vector3, Voxel>();

    public Chunk(Vector3 position)
    {
        Position = position;
        Voxels.Add(new Voxel(position));
        //VoxelsDict.Add(position, new Voxel())
    }


    //There should be a function that resets the byte array

}
