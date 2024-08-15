using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Drawing;
using Unity.VisualScripting;
using System.Linq;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;

using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;


public class MinecraftBuilder : MonoBehaviour
{


    public MiniMap miniMap;
    public GameObject cube, holder, VoxelsParent, AdditonParent, DeletionParent, cube222, voxel2;
    public Material[] materials;
    public RosPublisherExample pub;
    public RosSubscriberExample sub;
    public TextWriter txtwrtr;
    [NonSerialized]
    public float cubesize;
    static int Hor_angle_window = 18; //90; //36
    static int Ver_angle_window = 8; //46; //16
    static float angle_size = 2f;
    float Hor_angle_min = -((float)Hor_angle_window / 2);
    float Ver_angle_min = -((float)Ver_angle_window / 2);
    Vector3 Hor_Ray_direction;
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
    RaycastHit[] hits;
    bool spatial;
    MeshRenderer VoxelMeshRenderer;
    //Dictionary<Vector3, float> VoxelProba;



    // Start is called before the first frame update
    void Start()
    {
        cubesize = cube.transform.localScale.x;
        //Debug.Log(cubesize);
        xSize = 800; //800; //300; //yamin shmel
        ySize = 100; //100; // lafo2 latahet
        zSize = 500; //500; // edem lawara
        //nearest_pt = new Vector3(0, 0, 0);
        //grid_arr = new float[xSize * ySize * zSize];
        //timeRemaining = 2;
        /*Taj = new GameObject[xSize][][];
        for (int i = 0; i < xSize; i++)
        {
            Taj[i] = new GameObject[ySize][];
            for (int j = 0; j < ySize; j++)
            {
                Taj[i][j] = new GameObject[zSize];
            }
        }*/

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
    void Update()
    {
        if(MappingSwitch)  
        {
            Vector3 Gaze_direction = Camera.main.transform.forward;
            Vector3 Gaze_position = Camera.main.transform.position;
            RaycastHit hit;
            bool raycastHit; 
            //raycastHit = Physics.Raycast(Gaze_position, Gaze_direction, out hit, 10f); //distance was 2f

            for (int i = 0; i < (Hor_angle_window / angle_size); i++)
            {
                Hor_Ray_direction = Quaternion.Euler(0, (Hor_angle_min + (angle_size * i)), 0) * Gaze_direction;
                for (int j = 0; j < (int)(Ver_angle_window / angle_size); j++)
                {
                    //RaycastHit hit;
                    //bool raycastHit = false;
                    Ver_Ray_direction = Quaternion.Euler((Ver_angle_min + (angle_size * j)), 0, 0) * Hor_Ray_direction;
                    raycastHit = Physics.Raycast(Gaze_position, Ver_Ray_direction, out hit, 10f);

                    if (raycastHit && hit.transform.parent.name.Contains("World")) //The second condition ensures that only the spatial mesh is mapped
                    {
                        //txtwrtr.meshName = hit.collider.name;
                        distx_in_cubes = Mathf.RoundToInt(hit.point.x / cubesize);
                        disty_in_cubes = Mathf.RoundToInt(hit.point.y / cubesize);
                        distz_in_cubes = Mathf.RoundToInt(hit.point.z / cubesize);
                        nearest_pt2 = new Vector3(distx_in_cubes, disty_in_cubes, distz_in_cubes);
                        if (Mathf.Abs(distx_in_cubes) < xSize / 2 && Mathf.Abs(disty_in_cubes) < ySize / 2 && Mathf.Abs(distz_in_cubes) < zSize / 2)
                        {
                            // Construction area for the List optimization:
                            // VoxelInstantiator(hit.point);
                            // Instantiator(hit.point);
                            // Rasterizer(Gaze_position, hit.point);
                        }
                        ///if (hit.point.y >= -1.65)
                        ////{
                            VoxelInstantiator(hit.point);
                        ///}
                        


                        ///////For deleting
                        float Gaze_distance = Vector3.Distance(Gaze_position, hit.point);

                        hits = Physics.RaycastAll(Gaze_position, Ver_Ray_direction, Gaze_distance, 4);
                        
                        /*foreach (RaycastHit hity in hits)
                        {
                            if (hity.transform.name == "Voxel")
                            {
                                overlaps = Physics.OverlapBox(hity.transform.position, cubesizeScale / 2);
                                foreach (Collider overlap in overlaps)
                                {
                                    spatial = false;
                                    if (overlap.gameObject.name.Contains("SpatialMesh"))
                                    {
                                        spatial = true;
                                        break;
                                    }
                                }
                                if (spatial) continue;
                                VoxelDestroyer(hity.transform.position);


                                //VoxelDestroyer(hity.transform.position);
                                //Instantiate(cube222, hity.transform.position, Quaternion.identity);
                                //Debug.Log(Gaze_distance);
                            }
                        }*/
                        //////////////////////////// For deleting

                        //Rasterizer(Gaze_position, hit.point);
                    }
                    else if(raycastHit && hit.transform.name.Contains("Quad") && hit.transform.parent.tag != "Finish")
                    {
                        Debug.Log(GetID(hit) + ", (" + hit.transform.parent.position.x + "," + hit.transform.parent.position.y + "," + hit.transform.parent.position.z
                        + "),  " + hit.transform.parent.rotation + " ,  (" + hit.transform.localScale.x + "," + hit.transform.localScale.y + ")");
                        hit.transform.parent.tag = "Finish";
                    }
                }
            }
        }
    }


    private int GetID(RaycastHit hit)    //by hasan sayour
        {
            switch (hit.transform.parent.name[0])
            {
                case 'W':
                    return 0;
                case 'C':
                    return 1;
                case 'F':
                    return 2;
                default :
                    return -9;
            }

        }
    
    public void Rasterizer(Vector3 start, Vector3 end)
    {
        
        Vector3Int startcuby = new Vector3Int(Mathf.RoundToInt(start.x / cubesize), Mathf.RoundToInt(start.y / cubesize), Mathf.RoundToInt(start.z / cubesize));
        //float length = Mathf.Sqrt((end.x * end.x) + (end.y * end.y) + (end.z * end.z));// - 2 * cubesize;
        Vector3 direction = end - start;
        float length = direction.magnitude;
        length = length - 2 * cubesize;
        direction = direction.normalized * length;
        end = direction + start;
        //Debug.Log(end);
        Vector3 startcm = new Vector3(Mathf.RoundToInt(start.x / cubesize) * cubesize, Mathf.RoundToInt(start.y / cubesize) * cubesize, Mathf.RoundToInt(start.z / cubesize) * cubesize);
        //slopes
        float m_in_yx = (end.y - start.y) / (end.x - start.x);
        float m_in_zx = (end.z - start.z) / (end.x - start.x);
        float m_in_yz = (end.y - start.y) / (end.z - start.z);
        //intercepts
        float b_in_yx = start.y - m_in_yx * start.x;
        float b_in_zx = start.z - m_in_zx * start.x;
        float b_in_yz = start.y - m_in_yz * start.z;
        //directions
        int sx = end.x >= start.x ? 1 : -1;
        int sy = end.y >= start.y ? 1 : -1;
        int sz = end.z >= start.z ? 1 : -1;
        //equation of lines
        float tempyx; // = m_in_yx * (end.x + sx * (cubesize / 2)) + b_in_yx;
        float tempzx; // = m_in_zx * (end.x + sx * (cubesize / 2)) + b_in_zx;
        float tempyz;

        Vector3Int endcuby = new Vector3Int(Mathf.RoundToInt(end.x / cubesize), Mathf.RoundToInt(end.y / cubesize), Mathf.RoundToInt(end.z / cubesize));
        
        //Debug.Log(endcuby);
        int temp_yx_int;
        int temp_zx_int;
        int temp_yz_int;

        int counter = 0;
        while (startcuby != endcuby)
        //for (int i = 0; i < 150 ; i++)   
        {
            counter++;
            if (counter > 200)
            {
                Debug.Log("Ktir hek");
                break;
            }
            tempyx = m_in_yx * (startcm.x + sx * (cubesize / 2)) + b_in_yx;
            temp_yx_int = Mathf.RoundToInt(tempyx / cubesize);

            tempzx = m_in_zx * (startcm.x + sx * (cubesize / 2)) + b_in_zx;
            temp_zx_int = Mathf.RoundToInt(tempzx / cubesize);

            tempyz = m_in_yz * (startcm.z + sz * (cubesize / 2)) + b_in_yz;
            temp_yz_int = Mathf.RoundToInt(tempyz / cubesize);

            if (startcuby.y == temp_yx_int)
            {
                if (startcuby.z == temp_zx_int)
                {
                    startcuby.x = startcuby.x + sx;
                    startcm.x = startcm.x + sx * cubesize;
                }
                else
                {
                    startcuby.z = startcuby.z + sz;
                    startcm.z = startcm.z + sz * cubesize;
                }
            }
            else
            {

                if (startcuby.y == temp_yz_int)
                {
                    startcuby.z = startcuby.z + sz;
                    startcm.z = startcm.z + sz * cubesize;
                }
                else
                {
                    startcuby.y = startcuby.y + sy;
                    startcm.y = startcm.y + sy * cubesize;
                } 
            }
            //Debug.Log("Before: " + startcm);
            VoxelDestroyer(startcm);
            //Instantiate(cube222, startcm, Quaternion.identity);


            // Recently edited:

            /*indexo = (startcuby.x - 1 + xSize / 2) + (startcuby.y- 1 + ySize / 2) * xSize + (startcuby.z - 1 + zSize / 2) * xSize * ySize;
            if (grid_arr[indexo] > 0)// && grid_arr[indexo] < 1)
            {
                grid_arr[indexo] = grid_arr[indexo] - 0.001f;
            }
            

            if (Taj[startcuby.x - 1 + xSize / 2][startcuby.y - 1 + ySize / 2][startcuby.z - 1 + zSize / 2] != null && grid_arr[indexo] < 0.6f)
            {
                Destroy(Taj[startcuby.x - 1 + xSize / 2][startcuby.y - 1 + ySize / 2][startcuby.z - 1 + zSize / 2]);
            }*/


        }
    }

    public void Instantiator(Vector3 place, bool humanApproved = false, bool fromEditor = false)
    {
        distx_in_cubes = Mathf.RoundToInt(place.x / cubesize);
        disty_in_cubes = Mathf.RoundToInt(place.y / cubesize);
        distz_in_cubes = Mathf.RoundToInt(place.z / cubesize);

        if (Mathf.Abs(distx_in_cubes) < xSize / 2 && Mathf.Abs(disty_in_cubes) < ySize / 2 && Mathf.Abs(distz_in_cubes) < zSize / 2)
        {

            distx_in_cm = Mathf.RoundToInt(place.x / cubesize) * cubesize;
            disty_in_cm = Mathf.RoundToInt(place.y / cubesize) * cubesize;
            distz_in_cm = Mathf.RoundToInt(place.z / cubesize) * cubesize;
            nearest_pt = new Vector3(distx_in_cm, disty_in_cm, distz_in_cm);
            indexo = (distx_in_cubes - 1 + xSize / 2) + (disty_in_cubes - 1 + ySize / 2) * xSize + (distz_in_cubes - 1 + zSize / 2) * xSize * ySize;

            if (humanApproved)
            {
                grid_arr[indexo] = 100;
            }

            if (grid_arr[indexo] < 1)
            {
                grid_arr[indexo] = grid_arr[indexo] + 0.05f;
            }

            if (Taj[distx_in_cubes - 1 + xSize / 2][disty_in_cubes - 1 + ySize / 2][distz_in_cubes - 1 + zSize / 2] == null && grid_arr[indexo] >= 0.6f)
            {
                Taj[distx_in_cubes - 1 + xSize / 2][disty_in_cubes - 1 + ySize / 2][distz_in_cubes - 1 + zSize / 2] = Instantiate(cube, nearest_pt, Quaternion.identity);
                Taj[distx_in_cubes - 1 + xSize / 2][disty_in_cubes - 1 + ySize / 2][distz_in_cubes - 1 + zSize / 2].gameObject.name = "Voxel";
                Taj[distx_in_cubes - 1 + xSize / 2][disty_in_cubes - 1 + ySize / 2][distz_in_cubes - 1 + zSize / 2].transform.SetParent(VoxelsParent.gameObject.transform);
                //miniMap.Fill(nearest_pt);
                
                /*if (fromEditor)
                {
                    pub.EditedPointCloudPublisher(nearest_pt);
                }*/
                /*else
                {
                    pub.AddPointCloudtoROSMessage(nearest_pt);
                }*/
                
            }
        }
    }

    public void Destroyer(Vector3 bomb_incubes)
    {
        Destroy(Taj[Mathf.RoundToInt(bomb_incubes.x) - 1 + xSize / 2][Mathf.RoundToInt(bomb_incubes.y) - 1 + ySize / 2][Mathf.RoundToInt(bomb_incubes.z) - 1 + zSize / 2]);
    }

    public void InstantiateEditor(Vector3Int MaxDist, Vector3Int RecentDist)
    {
 
        int Startx = Mathf.Min(MaxDist.x, RecentDist.x);
        int Starty = Mathf.Min(MaxDist.y, RecentDist.y);
        int Startz = Mathf.Min(MaxDist.z, RecentDist.z);
        int Endx = Mathf.Max(MaxDist.x, RecentDist.x);
        int Endy = Mathf.Max(MaxDist.y, RecentDist.y);
        int Endz = Mathf.Max(MaxDist.z, RecentDist.z);

        for (int i = Startx; i <= Endx; i++)
        {
            for (int j = Starty; j <= Endy; j++)
            {
                for (int k = Startz; k <= Endz; k++)
                {
                    Instantiator(new Vector3(i, j, k) * cubesize, true, true);
                }
            }
        }
        pub.PublishEditedPointCloudMsg();
        Debug.Log("Adder");
    }

    /*public void DestroyEditor(Vector3Int MaxDist, Vector3Int RecentDist)
    {
        int Startx = Mathf.Min(MaxDist.x, RecentDist.x);
        int Starty = Mathf.Min(MaxDist.y, RecentDist.y);
        int Startz = Mathf.Min(MaxDist.z, RecentDist.z);
        int Endx = Mathf.Max(MaxDist.x, RecentDist.x);
        int Endy = Mathf.Max(MaxDist.y, RecentDist.y);
        int Endz = Mathf.Max(MaxDist.z, RecentDist.z);

        int Endxds = (Endx - Startx) * 2;
        int Endyds = (Endy - Starty) * 2;
        int Endzds = (Endz - Startz) * 2;

        for (int i = Startx; i <= Endxds; i++)
        {
            for (int j = Starty; j <= Endyds; j++)
            {
                for (int k = Startz; k <= Endzds; k++)
                {
                    pub.DeletedPointCloudPublisher(new Vector3(i, j, k) * 0.025f);
                    if(Taj[i - 1 + xSize / 2][j - 1 + ySize / 2][k - 1 + zSize / 2] != null)
                    {
                        pub.DeletedPointCloudPublisher(new Vector3(i, j, k) * cubesize);
                    }
                    //Destroyer(new Vector3(i, j, k));
                }
            }
        }
        pub.PublishDeletedPointCloudMsg();
    }*/

    public void DisableMinecraft()
    {
        MappingSwitch = false;
        //Papa.SetActive(false);
    }

    public void EnableMinecraft()
    {
        MappingSwitch = true;
        //Papa.SetActive(true);
    }

    /*public void DestroyEditor(Vector3Int MaxDist, Vector3Int RecentDist)
    {
        int Startx = Mathf.Min(MaxDist.x, RecentDist.x);
        int Starty = Mathf.Min(MaxDist.y, RecentDist.y);
        int Startz = Mathf.Min(MaxDist.z, RecentDist.z);
        int Endx = Mathf.Max(MaxDist.x, RecentDist.x);
        int Endy = Mathf.Max(MaxDist.y, RecentDist.y);
        int Endz = Mathf.Max(MaxDist.z, RecentDist.z);

        for (int i = Startx; i <= Endx; i++)
        {
            for (int j = Starty; j <= Endy; j++)
            {
                for (int k = Startz; k <= Endz; k++)
                {
                    pub.DeletedPointCloudPublisher(new Vector3(i, j, k) * cubesize);
                    if (Taj[i - 1 + xSize / 2][j - 1 + ySize / 2][k - 1 + zSize / 2] != null)
                    {
                        pub.DeletedPointCloudPublisher(new Vector3(i, j, k) * cubesize);
                    }
                    Destroyer(new Vector3(i, j, k));
                }
            }
        }
        pub.PublishDeletedPointCloudMsg();
    }*/

    public void DestroyEditor(Vector3Int MaxDist, Vector3Int RecentDist)
    {

        //MaxDist = new Vector3Int(1, 1, 1);
        //RecentDist = new Vector3Int(2, 3, 4);

        int Startx = Mathf.Min(MaxDist.x, RecentDist.x);
        int Starty = Mathf.Min(MaxDist.y, RecentDist.y);
        int Startz = Mathf.Min(MaxDist.z, RecentDist.z);
        int Endx = Mathf.Max(MaxDist.x, RecentDist.x);
        int Endy = Mathf.Max(MaxDist.y, RecentDist.y);
        int Endz = Mathf.Max(MaxDist.z, RecentDist.z);
        int Diffx = Endx - Startx;
        int Diffy = Endy - Starty;
        int Diffz = Endz - Startz;

        for (int i = 0; i <= Diffx; i++)
        {
            for (int j = 0; j <= Diffy; j++)
            {
                for (int k = 0; k <= Diffz; k++)
                {
                    pub.DeletedPointCloudPublisher(new Vector3(i, j, k) * 0.049f + new Vector3(Startx, Starty, Startz) * cubesize);
                    /*if (Taj[i - 1 + xSize / 2][j - 1 + ySize / 2][k - 1 + zSize / 2] != null)
                    {
                        pub.DeletedPointCloudPublisher(new Vector3(i, j, k) * cubesize);
                    }*/
                    Destroyer(new Vector3(i, j, k) + new Vector3(Startx, Starty, Startz));   ////uncomment
                }
            }
        }
        pub.PublishDeletedPointCloudMsg();
    }

    public void VoxelInstantiator(Vector3 point)
    {
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
                //hon haneshte8el.
                //hon haneshte8el.
                //hon haneshte8el.
                //hon haneshte8el.
                //hon haneshte8el.
                //hon haneshte8el.
                        kube = Instantiate(cube, point, Quaternion.identity);
                        kube.gameObject.name = "Voxel";
                        VoxelMeshRenderer = kube.GetComponent<MeshRenderer>();
                        VoxelMeshRenderer.material = materials[0];
                        kube.transform.SetParent(VoxelsParent.transform);
                        //hon makhasna
                        //hon makhasna
                        //hon makhasna
                        //hon makhasna

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

    public void VoxelDestroyer(Vector3 point)
    {

        distx_in_cm = Mathf.RoundToInt(point.x / cubesize) * cubesize;
        disty_in_cm = Mathf.RoundToInt(point.y / cubesize) * cubesize;
        distz_in_cm = Mathf.RoundToInt(point.z / cubesize) * cubesize;
        point = new Vector3(distx_in_cm, disty_in_cm, distz_in_cm);
        //Debug.Log("After: " + point);
        //Instantiate(cube222, point, Quaternion.identity);


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
            /*if (VoxelPose.Contains(point))
        {
            //Debug.Log("Why" + point);
            //Instantiate(cube222, point, Quaternion.identity);
            if (VoxelProba[VoxelPose.IndexOf(point)] > 0 && VoxelProba[VoxelPose.IndexOf(point)] < 1.1)
            {
                VoxelProba[VoxelPose.IndexOf(point)] = VoxelProba[VoxelPose.IndexOf(point)] - 0.01f;
            }

            if (VoxelExists[VoxelPose.IndexOf(point)] && VoxelProba[VoxelPose.IndexOf(point)] < 0.6f)
            {

                overlaps = Physics.OverlapBox(point, cubesizeScale / 2);
                
                if (overlaps != null)
                {
                    foreach (Collider overlap in overlaps)
                    {
                        if (overlap.gameObject.name.StartsWith("SpatialMesh"))
                        {
                            spatial = true;
                            break;
                        }
                    }
                    if (!spatial)
                    {
                        foreach (Collider overlap in overlaps)
                        {
                            if (overlap.gameObject.name.StartsWith("Voxel"))
                            {
                                //Instantiate(cube222, point, Quaternion.identity);
                                Destroy(overlap.gameObject);
                                break;
                            }
                        }
                    }
                    spatial = false;
                    *//*foreach (Collider overlap in overlaps)
                    {
                        if (overlap.gameObject.name.StartsWith("Voxel"))
                        {

                            Destroy(overlap.gameObject);
                            break;
                        }
                    }*//*
                }

                VoxelExists[VoxelPose.IndexOf(point)] = false;

                VoxelByte.RemoveRange(12 * VoxelByteMap.IndexOf(VoxelPose.IndexOf(point)), 12);
                VoxelByteMap.Remove(VoxelPose.IndexOf(point));
            }*/
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

            //pub.PublishEditedPointCloudMsg();
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

            //pub.PublishEditedPointCloudMsg();
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

            //pub.PublishEditedPointCloudMsg();
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
        
    
    
    // Old script:
    /*if (VoxelPose.Contains(point))
    {
        if (VoxelProba[VoxelPose.IndexOf(point)] > 0 && VoxelProba[VoxelPose.IndexOf(point)] < 1.1)
        {
            VoxelProba[VoxelPose.IndexOf(point)] = VoxelProba[VoxelPose.IndexOf(point)] - 0.001f;
        }

        if (VoxelExists[VoxelPose.IndexOf(point)] && VoxelProba[VoxelPose.IndexOf(point)] < 0.6f)
        {

            overlaps = Physics.OverlapBox(point, cubesizeScale / 2);
            if (overlaps != null)
            {
                foreach (Collider overlap in overlaps)
                {
                    if (overlap.gameObject.name == "Voxel")
                    {
                        Destroy(overlap.gameObject);
                        break;
                    }
                }
            }

            VoxelExists[VoxelPose.IndexOf(point)] = false;

            ///for(int i=VoxelByteMap.IndexOf(VoxelPose.IndexOf(point)); i <)

            VoxelByte.AddRange(BitConverter.GetBytes(VoxelPose[VoxelPose.IndexOf(point)].x));
            VoxelByte.AddRange(BitConverter.GetBytes(VoxelPose[VoxelPose.IndexOf(point)].y));
            VoxelByte.AddRange(BitConverter.GetBytes(VoxelPose[VoxelPose.IndexOf(point)].z));

            VoxelByteMap.Remove(VoxelPose.IndexOf(point));
        }
    }*/

    /*else
    {
        VoxelPose.Add(point);
        VoxelProba.Add(0.05f);
        VoxelExists.Add(false);
    }*/
    //}

    ////////////// Backup for Voxel Instantiator
    /* public void VoxelInstantiator(Vector3 point)
     {
         if (VoxelPose.Contains(point))
         {
             if (VoxelProba[VoxelPose.IndexOf(point)] < 1)
             {
                 VoxelProba[VoxelPose.IndexOf(point)] = VoxelProba[VoxelPose.IndexOf(point)] + 0.05f;
             }

             overlaps = Physics.OverlapBox(point, cubesizeScale / 2);
             if (overlaps != null)
             {
                 foreach (Collider overlap in overlaps)
                 {
                     if (overlap.gameObject.name == "Prism")
                     {

                         foreach (Collider overlap2 in overlaps)
                         {
                             if (overlap2.gameObject.name == "Voxel")
                             {
                                 overlap2.gameObject.name = "Labeled";
                                 //Destroy(overlap2.gameObject);   //this works
                             }
                         }
                     }
                 }
             }

         }
         else
         {
             VoxelPose.Add(point);
             VoxelProba.Add(0.05f);
         }
     }*/







    ////////This is plan b.
    /*public void DestroyEditor(Vector3Int MaxDist, Vector3Int RecentDist)
    {
        //MaxDist = new Vector3Int(1, 1, 1);
        //RecentDist = new Vector3Int(2, 3, 4);

        int Startx = Mathf.Min(MaxDist.x, RecentDist.x);
        int Starty = Mathf.Min(MaxDist.y, RecentDist.y);
        int Startz = Mathf.Min(MaxDist.z, RecentDist.z);
        int Endx = Mathf.Max(MaxDist.x, RecentDist.x);
        int Endy = Mathf.Max(MaxDist.y, RecentDist.y);
        int Endz = Mathf.Max(MaxDist.z, RecentDist.z);
        int Diffx = Endx - Startx;
        int Diffy = Endy - Starty;
        int Diffz = Endz - Startz;

        for (int i = 0; i <= Diffx * 2; i++)
        {
            for (int j = 0; j <= Diffy * 2; j++)
            {
                for (int k = 0; k <= Diffz * 2; k++)
                {
                    pub.DeletedPointCloudPublisher(new Vector3(i, j, k) * cubesize);
                    Debug.Log(new Vector3(i, j, k) * 0.025f + new Vector3(Startx,Starty,Startz));
                    *//*if (Taj[i - 1 + xSize / 2][j - 1 + ySize / 2][k - 1 + zSize / 2] != null)
                    {
                        pub.DeletedPointCloudPublisher(new Vector3(i, j, k) * cubesize);
                    }
                    Destroyer(new Vector3(i, j, k));*//*
                }
            }
        }
        pub.PublishDeletedPointCloudMsg();
    }*/
}
