using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
//using RosColor = RosMessageTypes.UnityRoboticsDemo.UnityColorMsg;
using OGGM = RosMessageTypes.Nav.OccupancyGridMsg;
using Posemsg = RosMessageTypes.Geometry.PoseMsg;
using pc2m = RosMessageTypes.Sensor.PointCloud2Msg;
using twist = RosMessageTypes.Geometry.TwistMsg;
using transformer = RosMessageTypes.CustomInterfaces;
using Geo = RosMessageTypes.Geometry;
using Nav = RosMessageTypes.Nav;
using StringMsg = RosMessageTypes.Std.StringMsg;
using TMPro;
using System.Security.Cryptography;
using System.Collections.Generic;
using System;
using Microsoft.MixedReality.Toolkit.Utilities.GameObjectManagement;
using Microsoft.MixedReality.Toolkit.UI;

public class RosSubscriberExample : MonoBehaviour
{
    //public GameObject cube;
    public RosPublisherExample pub;
    [NonSerialized]
    public sbyte[] arr;
    [NonSerialized]
    public byte[] pcarr;

    public GameObject[] ButtonsForMaps;

    [NonSerialized]
    public float resol;
    [NonSerialized]
    public float heig;
    [NonSerialized]
    public float wid;

    public GameObject objectToEnable;
    private Posemsg odom;
    [NonSerialized]
    public uint pcwidth;
    [NonSerialized]
    public pc2m incomingPointCloudDownSampled, localPointCloudDownSampled, incomingPointCloudLive;
    
    //public Geo.PoseStampedMsg GoalPose;
    //public Nav.PathMsg Path;
    //Nav.OdometryMsg Odometry;
    [NonSerialized]
    public double x, y, z, rx, ry, rz;
    public MiniMapIncoming mmincom;
    public MiniMap miniMap;
    public TextMeshPro MenuText;
    public MergedVoxelDisplay mvd;
    GameObject PathParent, PathElement;
    Vector3 Shift = new Vector3();
    Vector3 FixedShift = new Vector3(0, 0, 0);
    Vector3 PathGoal = new Vector3();
    List<string> ReceivedMapNames = new List<string>();
    void Start()
    {

        //ROSConnection.GetOrCreateInstance().Subscribe<RosColor>("color", ColorChange);
        ROSConnection.GetOrCreateInstance().Subscribe<OGGM>("/mapToUnity", Ocupo);
        ROSConnection.GetOrCreateInstance().Subscribe<pc2m>("/robot_map_downsampled", pointCloud); // No need for them anymore
        ROSConnection.GetOrCreateInstance().Subscribe<pc2m>("/local_map_downsampled", localPointCloud);
        ROSConnection.GetOrCreateInstance().Subscribe<pc2m>("/com/semantic_pcl", pointCloudLive);
        ROSConnection.GetOrCreateInstance().Subscribe<transformer.TransformationMsg>("/refined_tf", twistReceived); //This should become the edited message type that has idto idfrom
        //ROSConnection.GetOrCreateInstance().Subscribe<twist>("/trans_topic_merger", twistReceived);
        ROSConnection.GetOrCreateInstance().Subscribe<pc2m>("/com/downsampled", pointCloudDownsampled);
        ROSConnection.GetOrCreateInstance().Subscribe<pc2m>("/human/human_label", pointCloudDownsampledTest);

        ROSConnection.GetOrCreateInstance().Subscribe<Nav.PathMsg>("/plan", PathDataSub);
        ROSConnection.GetOrCreateInstance().Subscribe<Nav.OdometryMsg>("/odom", OdomSub);

        ROSConnection.GetOrCreateInstance().Subscribe<StringMsg>("/com/map_names", MapNames);

        //new
        //ROSConnection.GetOrCreateInstance().Subscribe<OGGM>("occupancy_map", Ocupo);

    }
    public void Ocupo(OGGM bata)
    {
        resol = bata.info.resolution;
        heig = bata.info.height;
        wid = bata.info.width;
        arr = bata.data;
        objectToEnable.SetActive(true);
        //cube.GetComponent<Renderer>().material.color = new Color32((byte)colorMessage.r, (byte)colorMessage.g, (byte)colorMessage.b, (byte)colorMessage.a);
    }
    public void pointCloud(pc2m ptcld)
    {
        incomingPointCloudDownSampled = ptcld;
    }
    public void pointCloudLive(pc2m ptcldlive)
    {
        incomingPointCloudLive = ptcldlive;
        Debug.Log("Ejit");
        //mvd.ShowMergedMap();
        //Debug.Log(ptcldlive.data[17]);    
    }
    public void twistReceived(transformer.TransformationMsg Twisty)
    {


        /*x = 1 * Twisty.linear.y;
        y = -1 * Twisty.linear.z;
        z = -1 * Twisty.linear.x;
        rx = 1 * Twisty.angular.y * Mathf.Rad2Deg;
        ry = -1 * Twisty.angular.z * Mathf.Rad2Deg;
        rz = -1 * Twisty.angular.x * Mathf.Rad2Deg;*/

        /*x = -1 * Twisty.linear.y;
        y = -1 * Twisty.linear.z;
        z = 1 * Twisty.linear.x;
        rx = 1 * Twisty.angular.y * Mathf.Rad2Deg;
        ry = 1 * Twisty.angular.z * Mathf.Rad2Deg;
        rz = 1 * Twisty.angular.x * Mathf.Rad2Deg; //was -1
        */
        /*Debug.Log(Twisty.tf.linear.x);
        Debug.Log(Twisty.tf.linear.y);
        Debug.Log(Twisty.tf.linear.z);
        Debug.Log(Twisty.tf.angular.x);
        Debug.Log(Twisty.tf.angular.y);
        Debug.Log(Twisty.tf.angular.z);*/
        if (Twisty.idfrom == 2)
        {
            x = -1 * Twisty.tf.linear.x;
            y = -1 * Twisty.tf.linear.z;
            z = -1 * Twisty.tf.linear.y;
            rx = 1 * Twisty.tf.angular.x * Mathf.Rad2Deg;
            ry = 1 * Twisty.tf.angular.z * Mathf.Rad2Deg;
            rz = 1 * Twisty.tf.angular.y * Mathf.Rad2Deg; //was -1
            //Debug.Log(ry);
            pub.FirstAlignment = false;
        }
        
        

    }

    public void localPointCloud(pc2m localptcld)
    {
        localPointCloudDownSampled = localptcld;
        //Debug.Log("Stla2ayna");
    }

    public void pointCloudDownsampled(pc2m downsampled)
    {
        if (downsampled.header.stamp.nanosec <= 1)
        {
            pub.IDto = (int)downsampled.header.stamp.nanosec;
            mmincom.Clean();
            mmincom.FillIncoming(downsampled);
        }
        else
        {
            pub.IDfrom = (int)downsampled.header.stamp.nanosec;
            miniMap.Clean();
            miniMap.FillLocal(downsampled);
            MenuText.text = "Maps received";
            Debug.Log("Hry");
            pub.RequestDownsampledTo();
        }

    }
    public void pointCloudDownsampledTest(pc2m label)
    {

        //Debug.Log(label.data.Length);
        //Debug.Log(label.data[13]+" "+label.data[14]);
        /*for (int i = 0; i < 32; i++)
        {
            Debug.Log(i + ": " + label.data[i]);
        }*/

    }

    public void PathDataSub(Nav.PathMsg path)
    {
        Debug.Log("eja pathhh");
        Vector3 posePath = new Vector3();
        Vector3 posePath2 = new Vector3();
        Vector3 dir = new Vector3();
        Quaternion quaternion = new Quaternion();
        //Debug.Log(path.poses[path.poses.Length  - 1].pose.position);
        // Check if we got a new goal from Rviz
        posePath.Set((float)path.poses[path.poses.Length - 1].pose.position.x, (float)path.poses[path.poses.Length - 1].pose.position.z, (float)path.poses[path.poses.Length - 1].pose.position.y);


        if (PathGoal != posePath)
        {
            PathGoal = posePath;
            //FixedShift = Shift;
        }


        //Delete old path points
        if (PathParent != null) Destroy(PathParent);


        //Instantiate new path objects (arrows)
        PathParent = new GameObject("PathParent");
        PathParent.transform.Rotate(new Vector3((float)rx, (float)ry, (float)rz), Space.Self);
        PathParent.transform.Translate(new Vector3((float)x, (float)y, (float)z), Space.Self);
        for (int i = 0; i < path.poses.Length - 1; i++)
        {
            posePath.Set((float)path.poses[i].pose.position.x, (float)path.poses[i].pose.position.z, (float)path.poses[i].pose.position.y);

            //posePath -= FixedShift;
            //posePath = TransformFromGlobalToLocal(posePath);
            
            posePath2.Set((float)path.poses[i + 1].pose.position.x, (float)path.poses[i + 1].pose.position.z, (float)path.poses[i + 1].pose.position.y);
            //posePath2 -= FixedShift;
            //posePath2 = TransformFromGlobalToLocal(posePath2);

            dir = posePath2 - posePath;

            quaternion.SetFromToRotation(Vector3.forward, dir);

            PathElement = Instantiate(objectToEnable, posePath, quaternion);
            PathElement.transform.SetParent(PathParent.transform, false);

        }
    }
    public Vector3 TransformFromGlobalToLocal(Vector3 poiint)
    {
        Vector3 rotationAngles = new Vector3((float)rx, (float)ry, (float)rz);
        Vector3 translation = new Vector3((float)x, (float)y, (float)z);
        
        Quaternion rotationQuaternion = Quaternion.Euler(rotationAngles);
        
        Vector3 point = rotationQuaternion * poiint + translation;

        return point;
    }
    public void OdomSub(Nav.OdometryMsg odom)
    {
        //When Vuforia is able to localize the robot, please add the transform that vuforia returns to the shift.
        // Also don't forget to account for the rotation of the robot

        Shift.Set((float)odom.pose.pose.position.x, (float)odom.pose.pose.position.z, (float)odom.pose.pose.position.y);

    }

    public void MapNames(StringMsg msg)
    {
        if (!ReceivedMapNames.Contains(msg.data.ToLower()))
        {
            ReceivedMapNames.Add(msg.data.ToLower());
            ButtonsForMaps[ReceivedMapNames.Count - 1].SetActive(true);
            ButtonsForMaps[ReceivedMapNames.Count - 1].GetComponent<ButtonConfigHelper>().MainLabelText = msg.data.ToLower();
        }
    }
}