using System.Collections;
using System;
using JetBrains.Annotations;
using System.Linq;
//using System.Numerics;
using UnityEngine;
using System.Collections.Generic;

public class VoxelManager : MonoBehaviour
{
    
    List<Vector3> VoxeletPosition = new List<Vector3>();
    // Start is called before the first frame update
    void Start()
    {
        Voxel voxilaya = new Voxel(new Vector3(1,0,0));
        Voxel voxilaya2 = new Voxel(new Vector3(1,0,0));
        Vector3 vocto = new Vector3(0,0,0);
        Vector3 vocto2 = new Vector3(0,0,1);
        Vector3 vocto3 = new Vector3(0,0,1);

        VoxeletPosition.Add(vocto);
        VoxeletPosition.Add(vocto2);
        //Voxel voxilaya3 = new Voxel(new Vector3(2,0,0));
        Monazzim.Voxelet.Add(voxilaya);
        //Voxelet.Add(voxilaya2);
        Debug.Log("Yes " + Monazzim.Voxelet.Any(x => x.Posito == voxilaya2.Posito));
        //Debug.Log("Yes " + VoxeletPosition.Contains(vocto3));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

class Voxel
{
    public Vector3 Posito;
    public float Proba;
    public bool State;
    public byte[] PoseInBytes;

    private void Converter() 
    {
        PoseInBytes = BitConverter.GetBytes(Posito.x);
    }
    
    public Voxel(Vector3 vecto)
    {
        //Debug.Log("Akalz");
        Posito = vecto;
        //Debug.Log(Pose);

    }
}

static class Monazzim 
{
    public static List<Voxel> Voxelet = new List<Voxel>();
}
