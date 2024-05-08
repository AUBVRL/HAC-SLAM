using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Drawing;
using Unity.VisualScripting;
using System.Linq;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using RosMessageTypes.Sensor;


public class MinecraftBuilder : MonoBehaviour
{

    public MiniMap miniMap;
    public GameObject cube, holder, VoxelsParent, AdditonParent, DeletionParent, cube222;
    public Material[] materials;
    
    public RosSubscriberExample sub;
    public TextWriter txtwrtr;
    [NonSerialized]
    public float cubesize;
    static int Hor_angle_window = 18; //90; //36
    static int Ver_angle_window = 8; //46; //16
    static float angle_size = 2f;
    float Hor_angle_min = -((float)Hor_angle_window / 2);
    float Ver_angle_min = -((float)Ver_angle_window / 2);
    Vector3 Hor_Ray_direction, Gaze_direction, Gaze_position;
    Vector3 Ver_Ray_direction;
    //[NonSerialized]
    public GameObject[][][] Taj;
    public GameObject parenttest;
    GameObject kube;
    int xSize;
    int ySize;
    int zSize;
    float distx_in_cm;
    float disty_in_cm;
    float distz_in_cm;
    int distx_in_cubes;
    int disty_in_cubes;
    int distz_in_cubes;
    //float min_hit;
    Vector3 nearest_pt, TransformedPoints;
    Vector3 nearest_pt2;
    Vector3[] pt = new Vector3[(int)(Hor_angle_window / angle_size)];
    float hit_distance;
    float Gaze_distance;
    //float  for time 
    float[] grid_arr;
    int indexo;
    bool MappingSwitch;

    List<Vector3> VoxelPose;
    [NonSerialized]
    public List<Byte> VoxelByte, AddedVoxelByte, DeletedVoxelByte;
    List<float> VoxelProba;
    List<bool> VoxelExists;
    List<int> VoxelByteMap;
    Collider[] overlaps;
    Vector3 cubesizeScale;
    RaycastHit hit;
    RaycastHit[] hits;
    bool spatial, raycastHit;
    MeshRenderer VoxelMeshRenderer;

    //Refactoring variables:
    List<Vector3> extractedPoints;
    



    // Start is called before the first frame update
    void Start()
    {
        cubesize = cube.transform.localScale.x;

        MappingSwitch = true;

        VoxelPose = new List<Vector3>();
        VoxelByte = new List<Byte>();
        AddedVoxelByte = new List<Byte>();
        DeletedVoxelByte = new List<Byte>();
        VoxelProba = new List<float>();
        VoxelExists = new List<bool>();
        VoxelByteMap = new List<int>();
        cubesizeScale = new Vector3(cubesize - 0.001f, cubesize - 0.001f, cubesize - 0.001f);
        //VoxelProba = new Dictionary<Vector3, float>();
        spatial = false;




    }

    // Update is called once per frame
   private void Update()
    {
        extractedPoints = MeshToPointCloud();

        VoxelInstantiator(extractedPoints);
        
        VoxelDestroyer(extractedPoints);
    }

    public void DisableMinecraft()
    {
        MappingSwitch = false;
    }

    public void EnableMinecraft()
    {
        MappingSwitch = true;
    }

    public void VoxelInstantiator(Vector3 point)
    {
        //point = point / cubesize;
        //Vector3Int Roundedpoint = Vector3Int.RoundToInt(point);
        distx_in_cm = Mathf.RoundToInt(point.x / cubesize) * cubesize;
        disty_in_cm = Mathf.RoundToInt(point.y / cubesize) * cubesize;
        distz_in_cm = Mathf.RoundToInt(point.z / cubesize) * cubesize;
        point.Set(distx_in_cm, disty_in_cm, distz_in_cm);
    
        if (VoxelPose.Contains(point))
        {
            if (VoxelProba[VoxelPose.IndexOf(point)] < 1 && VoxelProba[VoxelPose.IndexOf(point)] >= 0)
            {
                VoxelProba[VoxelPose.IndexOf(point)] = VoxelProba[VoxelPose.IndexOf(point)] + 0.15f;
            }

            if (!VoxelExists[VoxelPose.IndexOf(point)] && VoxelProba[VoxelPose.IndexOf(point)] > 0.6f && VoxelProba[VoxelPose.IndexOf(point)] <= 1)
            {
                kube = Instantiate(cube, point, Quaternion.identity, VoxelsParent.transform);
                //kube.gameObject.name = "Voxel";
                VoxelMeshRenderer = kube.GetComponent<MeshRenderer>();
                VoxelMeshRenderer.material = materials[0];
                
                
                VoxelExists[VoxelPose.IndexOf(point)] = true;

                VoxelByteMap.Add(VoxelPose.IndexOf(point));

                VoxelByte.AddRange(BitConverter.GetBytes((VoxelPose[VoxelPose.IndexOf(point)].x / cubesize) * 0.04999999f));
                VoxelByte.AddRange(BitConverter.GetBytes((VoxelPose[VoxelPose.IndexOf(point)].z / cubesize) * 0.04999999f));
                VoxelByte.AddRange(BitConverter.GetBytes((VoxelPose[VoxelPose.IndexOf(point)].y / cubesize) * 0.04999999f));

            }
        }

        else
        {
            VoxelPose.Add(point);
            VoxelProba.Add(0.05f);
            VoxelExists.Add(false);
        }
    }

    public void VoxelInstantiator(List<Vector3> vectors)
    {
        foreach (Vector3 vector in vectors)
        {
            VoxelInstantiator(vector);
        }
    }
    
    public void VoxelDestroyer(Vector3 point)
    {

        distx_in_cm = Mathf.RoundToInt(point.x / cubesize) * cubesize;
        disty_in_cm = Mathf.RoundToInt(point.y / cubesize) * cubesize;
        distz_in_cm = Mathf.RoundToInt(point.z / cubesize) * cubesize;
        point.Set(distx_in_cm, disty_in_cm, distz_in_cm);


        if (VoxelProba[VoxelPose.IndexOf(point)] > 0 && VoxelProba[VoxelPose.IndexOf(point)] < 1.1)
        {
            VoxelProba[VoxelPose.IndexOf(point)] = VoxelProba[VoxelPose.IndexOf(point)] - 0.01f;
        }

        if (VoxelExists[VoxelPose.IndexOf(point)] && VoxelProba[VoxelPose.IndexOf(point)] < 0.6f && VoxelProba[VoxelPose.IndexOf(point)] >= 0)
        {

            overlaps = Physics.OverlapBox(point, cubesizeScale / 2);
            foreach (Collider overlap in overlaps)
            {
                if (overlap.gameObject.name.StartsWith("Voxel"))
                {
                    //Instantiate(cube222, point, Quaternion.identity);
                    Destroy(overlap.gameObject);
                    break;
                }

            }
            VoxelExists[VoxelPose.IndexOf(point)] = false;
            
            VoxelByte.RemoveRange(12 * VoxelByteMap.IndexOf(VoxelPose.IndexOf(point)), 12);
            VoxelByteMap.Remove(VoxelPose.IndexOf(point));
        }
            
    }

    public void VoxelDestroyer(List<Vector3> vectors)
    {
        int voxelLayerMask = 1 << 7;
        int spatialMeshLayerMask = 1 << 31;
        
        foreach (Vector3 vector in vectors)
        {
            Gaze_distance = Vector3.Distance(Camera.main.transform.position, vector) - 0.09f;
            Vector3 direction =  vector - Camera.main.transform.position;
            hits = Physics.RaycastAll(Camera.main.transform.position, direction.normalized, Gaze_distance, voxelLayerMask);
            foreach (RaycastHit Hit in hits)
            {  
                bool checkBoxOverlap = Physics.CheckBox(Hit.transform.position, cubesizeScale, Quaternion.identity, spatialMeshLayerMask);
                    if (!checkBoxOverlap)
                    {
                        VoxelDestroyer(Hit.transform.position);
                    }
            }
        }
    }

    public void UserVoxelAddition(Vector3 point)
    {
        distx_in_cm = Mathf.RoundToInt(point.x / cubesize) * cubesize;
        disty_in_cm = Mathf.RoundToInt(point.y / cubesize) * cubesize;
        distz_in_cm = Mathf.RoundToInt(point.z / cubesize) * cubesize;
        point = new Vector3(distx_in_cm, disty_in_cm, distz_in_cm);
        
        if (VoxelPose.Contains(point))
        {
            VoxelProba[VoxelPose.IndexOf(point)] = 2f;
            if (!VoxelExists[VoxelPose.IndexOf(point)]) //Fix to include if the voxel already exists
            {
                VoxelExists[VoxelPose.IndexOf(point)] = true;

                kube = Instantiate(cube, point, Quaternion.identity);
                kube.name = "VoxelAdded";
                VoxelMeshRenderer = kube.GetComponent<MeshRenderer>();
                VoxelMeshRenderer.material = materials[1];
                kube.transform.SetParent(AdditonParent.gameObject.transform);

                VoxelByteMap.Add(VoxelPose.IndexOf(point));

                VoxelByte.AddRange(BitConverter.GetBytes((VoxelPose[VoxelPose.IndexOf(point)].x / cubesize) * 0.04999999f));
                VoxelByte.AddRange(BitConverter.GetBytes((VoxelPose[VoxelPose.IndexOf(point)].z / cubesize) * 0.04999999f));
                VoxelByte.AddRange(BitConverter.GetBytes((VoxelPose[VoxelPose.IndexOf(point)].y / cubesize) * 0.04999999f));
            }
            else
            {
                overlaps = Physics.OverlapBox(point, cubesizeScale / 2);
                foreach (Collider overlap in overlaps)
                {
                    if (overlap.gameObject.name.Contains("Voxel"))
                    {
                        overlap.gameObject.name = "VoxelAdded";
                        VoxelMeshRenderer = overlap.gameObject.GetComponent<MeshRenderer>();
                        VoxelMeshRenderer.material = materials[1];
                        overlap.gameObject.transform.SetParent(AdditonParent.gameObject.transform);
                    }

                }
            }
        }
        else
        {

            VoxelPose.Add(point);
            VoxelProba.Add(2f);
            VoxelExists.Add(true);

            VoxelByteMap.Add(VoxelPose.IndexOf(point));

            VoxelByte.AddRange(BitConverter.GetBytes((VoxelPose[VoxelPose.IndexOf(point)].x / cubesize) * 0.04999999f));
            VoxelByte.AddRange(BitConverter.GetBytes((VoxelPose[VoxelPose.IndexOf(point)].z / cubesize) * 0.04999999f));
            VoxelByte.AddRange(BitConverter.GetBytes((VoxelPose[VoxelPose.IndexOf(point)].y / cubesize) * 0.04999999f));

            
            kube = Instantiate(cube, point, Quaternion.identity);
            kube.gameObject.name = "VoxelAdded";
            VoxelMeshRenderer = kube.GetComponent<MeshRenderer>();
            VoxelMeshRenderer.material = materials[1];
            kube.transform.SetParent(AdditonParent.gameObject.transform);

        }

        TransformedPoints = TransformPCL((point / cubesize) * 0.04999999f);
        AddedVoxelByte.AddRange(BitConverter.GetBytes(TransformedPoints.x));
        AddedVoxelByte.AddRange(BitConverter.GetBytes(TransformedPoints.z));
        AddedVoxelByte.AddRange(BitConverter.GetBytes(TransformedPoints.y));
    }

    public void UserAssetAddition(Vector3 point)
    {
        distx_in_cm = Mathf.RoundToInt(point.x / cubesize) * cubesize;
        disty_in_cm = Mathf.RoundToInt(point.y / cubesize) * cubesize;
        distz_in_cm = Mathf.RoundToInt(point.z / cubesize) * cubesize;
        point = new Vector3(distx_in_cm, disty_in_cm, distz_in_cm);

        if (VoxelPose.Contains(point))
        {
            VoxelProba[VoxelPose.IndexOf(point)] = 2f;
            if (!VoxelExists[VoxelPose.IndexOf(point)]) //Fix to include if the voxel already exists
            {
                VoxelExists[VoxelPose.IndexOf(point)] = true;

                kube = Instantiate(cube, point, Quaternion.identity);
                kube.name = "VoxelAdded";
                VoxelMeshRenderer = kube.GetComponent<MeshRenderer>();
                VoxelMeshRenderer.material = materials[3];
                kube.transform.SetParent(AdditonParent.gameObject.transform);

                VoxelByteMap.Add(VoxelPose.IndexOf(point));

                VoxelByte.AddRange(BitConverter.GetBytes((VoxelPose[VoxelPose.IndexOf(point)].x / cubesize) * 0.04999999f));
                VoxelByte.AddRange(BitConverter.GetBytes((VoxelPose[VoxelPose.IndexOf(point)].z / cubesize) * 0.04999999f));
                VoxelByte.AddRange(BitConverter.GetBytes((VoxelPose[VoxelPose.IndexOf(point)].y / cubesize) * 0.04999999f));
            }
            else
            {
                overlaps = Physics.OverlapBox(point, cubesizeScale / 2);
                foreach (Collider overlap in overlaps)
                {
                    if (overlap.gameObject.name.Contains("Voxel"))
                    {
                        overlap.gameObject.name = "VoxelAdded";
                        VoxelMeshRenderer = overlap.gameObject.GetComponent<MeshRenderer>();
                        VoxelMeshRenderer.material = materials[3];
                        overlap.gameObject.transform.SetParent(AdditonParent.gameObject.transform);
                    }

                }
            }
        }
        else
        {
            VoxelPose.Add(point);
            VoxelProba.Add(2f);
            VoxelExists.Add(true);

            VoxelByteMap.Add(VoxelPose.IndexOf(point));

            VoxelByte.AddRange(BitConverter.GetBytes((VoxelPose[VoxelPose.IndexOf(point)].x / cubesize) * 0.04999999f));
            VoxelByte.AddRange(BitConverter.GetBytes((VoxelPose[VoxelPose.IndexOf(point)].z / cubesize) * 0.04999999f));
            VoxelByte.AddRange(BitConverter.GetBytes((VoxelPose[VoxelPose.IndexOf(point)].y / cubesize) * 0.04999999f));

            
            kube = Instantiate(cube, point, Quaternion.identity);
            kube.gameObject.name = "VoxelAdded";
            VoxelMeshRenderer = kube.GetComponent<MeshRenderer>();
            VoxelMeshRenderer.material = materials[3];
            kube.transform.SetParent(AdditonParent.gameObject.transform);
        }
        TransformedPoints = TransformPCL((point / cubesize) * 0.04999999f);
        AddedVoxelByte.AddRange(BitConverter.GetBytes(TransformedPoints.x));
        AddedVoxelByte.AddRange(BitConverter.GetBytes(TransformedPoints.z));
        AddedVoxelByte.AddRange(BitConverter.GetBytes(TransformedPoints.y));
    }

    public void VuforiaAddition(Vector3 point)
    {
        distx_in_cm = Mathf.RoundToInt(point.x / cubesize) * cubesize;
        disty_in_cm = Mathf.RoundToInt(point.y / cubesize) * cubesize;
        distz_in_cm = Mathf.RoundToInt(point.z / cubesize) * cubesize;
        point = new Vector3(distx_in_cm, disty_in_cm, distz_in_cm);

        if (VoxelPose.Contains(point))
        {
            VoxelProba[VoxelPose.IndexOf(point)] = 2f;
            if (!VoxelExists[VoxelPose.IndexOf(point)]) //Fix to include if the voxel already exists
            {
                VoxelExists[VoxelPose.IndexOf(point)] = true;

                kube = Instantiate(cube, point, Quaternion.identity);
                kube.name = "VoxelAdded";
                VoxelMeshRenderer = kube.GetComponent<MeshRenderer>();
                VoxelMeshRenderer.material = materials[4];
                kube.transform.SetParent(AdditonParent.gameObject.transform);

                VoxelByteMap.Add(VoxelPose.IndexOf(point));

                VoxelByte.AddRange(BitConverter.GetBytes((VoxelPose[VoxelPose.IndexOf(point)].x / cubesize) * 0.04999999f));
                VoxelByte.AddRange(BitConverter.GetBytes((VoxelPose[VoxelPose.IndexOf(point)].z / cubesize) * 0.04999999f));
                VoxelByte.AddRange(BitConverter.GetBytes((VoxelPose[VoxelPose.IndexOf(point)].y / cubesize) * 0.04999999f));
            }
            else
            {
                overlaps = Physics.OverlapBox(point, cubesizeScale / 2);
                foreach (Collider overlap in overlaps)
                {
                    if (overlap.gameObject.name.Contains("Voxel"))
                    {
                        overlap.gameObject.name = "VoxelAdded";
                        VoxelMeshRenderer = overlap.gameObject.GetComponent<MeshRenderer>();
                        VoxelMeshRenderer.material = materials[4];
                        overlap.gameObject.transform.SetParent(AdditonParent.gameObject.transform);
                    }

                }
            }
        }
        else
        {
            VoxelPose.Add(point);
            VoxelProba.Add(2f);
            VoxelExists.Add(true);

            VoxelByteMap.Add(VoxelPose.IndexOf(point));

            VoxelByte.AddRange(BitConverter.GetBytes((VoxelPose[VoxelPose.IndexOf(point)].x / cubesize) * 0.04999999f));
            VoxelByte.AddRange(BitConverter.GetBytes((VoxelPose[VoxelPose.IndexOf(point)].z / cubesize) * 0.04999999f));
            VoxelByte.AddRange(BitConverter.GetBytes((VoxelPose[VoxelPose.IndexOf(point)].y / cubesize) * 0.04999999f));

            
            kube = Instantiate(cube, point, Quaternion.identity);
            kube.gameObject.name = "VoxelAdded";
            VoxelMeshRenderer = kube.GetComponent<MeshRenderer>();
            VoxelMeshRenderer.material = materials[4];
            kube.transform.SetParent(AdditonParent.gameObject.transform);
        }
        TransformedPoints = TransformPCL((point / cubesize) * 0.04999999f);
        AddedVoxelByte.AddRange(BitConverter.GetBytes(TransformedPoints.x));
        AddedVoxelByte.AddRange(BitConverter.GetBytes(TransformedPoints.z));
        AddedVoxelByte.AddRange(BitConverter.GetBytes(TransformedPoints.y));
    }

    public void UserVoxelDeletion(Vector3 point)
    {

        distx_in_cm = Mathf.RoundToInt(point.x / cubesize) * cubesize;
        disty_in_cm = Mathf.RoundToInt(point.y / cubesize) * cubesize;
        distz_in_cm = Mathf.RoundToInt(point.z / cubesize) * cubesize;
        point.Set(distx_in_cm, disty_in_cm, distz_in_cm);
        if (VoxelPose.Contains(point))
        {
            VoxelProba[VoxelPose.IndexOf(point)] = 2f;
            if (VoxelExists[VoxelPose.IndexOf(point)])
            {
                overlaps = Physics.OverlapBox(point, cubesizeScale / 2);
                foreach (Collider overlap in overlaps)
                {
                    if (overlap.gameObject.name.Contains("Voxel"))
                    {
                        //Instantiate(cube222, point, Quaternion.identity);
                        Destroy(overlap.gameObject);
                        break;
                    }

                }
                VoxelByte.RemoveRange(12 * VoxelByteMap.IndexOf(VoxelPose.IndexOf(point)), 12);
                VoxelByteMap.Remove(VoxelPose.IndexOf(point));
            }
        }
        else
        {
            VoxelPose.Add(point);
            VoxelProba.Add(2f);
            VoxelExists.Add(false);

            
        }
        TransformedPoints = TransformPCL((point / cubesize) * 0.04999999f);
        DeletedVoxelByte.AddRange(BitConverter.GetBytes(TransformedPoints.x));
        DeletedVoxelByte.AddRange(BitConverter.GetBytes(TransformedPoints.z));
        DeletedVoxelByte.AddRange(BitConverter.GetBytes(TransformedPoints.y));

        kube = Instantiate(cube, point, Quaternion.identity);
        kube.gameObject.name = "VoxelDeleted";
        VoxelMeshRenderer = kube.GetComponent<MeshRenderer>();
        VoxelMeshRenderer.material = materials[2];
        kube.transform.SetParent(DeletionParent.gameObject.transform);
    }

    public Vector3 TransformPCL(Vector3 Pooint)
    {

        Vector3 rotationAngles = new Vector3(-(float)sub.rx, -(float)sub.ry, -(float)sub.rz);
        Vector3 translation = new Vector3(-(float)sub.x, -(float)sub.y, -(float)sub.z);
        //Vector3 rotationAngles = new Vector3(0, 133.951803f, 0);
        //Vector3 translation = new Vector3(-2.622f, 1.4178f, -0.0768f);
        Quaternion rotationQuaternion = Quaternion.Euler(rotationAngles);
        //newPose = this.transform.position;
        Pooint = rotationQuaternion * Pooint + translation;
        return Pooint;
    }

    public List<Vector3> MeshToPointCloud()
    {
        List<Vector3> meshPoints = new List<Vector3>();
        Gaze_direction = Camera.main.transform.forward;
        Gaze_position = Camera.main.transform.position;
        int layerMask = 1 << 31;

        for (int i = 0; i < (Hor_angle_window / angle_size); i++)
        {
            Hor_Ray_direction = Quaternion.Euler(0, (Hor_angle_min + (angle_size * i)), 0) * Gaze_direction;
            for (int j = 0; j < (int)(Ver_angle_window / angle_size); j++)
            {
                Ver_Ray_direction = Quaternion.Euler((Ver_angle_min + (angle_size * j)), 0, 0) * Hor_Ray_direction;
                raycastHit = Physics.Raycast(Gaze_position, Ver_Ray_direction, out hit, 10f, layerMask);
                if (raycastHit)
                    {
                        meshPoints.Add(hit.point);
                    }
            }
        }
        
        return meshPoints;
    }

}
        
