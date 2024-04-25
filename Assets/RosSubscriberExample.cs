using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using pc2m = RosMessageTypes.Sensor.PointCloud2Msg;
using twist = RosMessageTypes.Geometry.TwistMsg;
using transformer = RosMessageTypes.CustomInterfaces;

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
    public GameObject[] ButtonsForMaps;
    public GameObject objectToEnable;
    [NonSerialized]
    public uint pcwidth;
    [NonSerialized]
    public pc2m incomingPointCloudLive;
    
    [NonSerialized]
    public double x, y, z, rx, ry, rz;
    public MiniMapIncoming mmincom;
    public MiniMap miniMap;
    public TextMeshPro MenuText;
    public MergedVoxelDisplay mvd;
    GameObject PathParent, PathElement;
    Vector3 Shift = new Vector3();
    Vector3 FixedShift = new Vector3(0, 0, 0);
    //Vector3 PathGoal = new Vector3();

    Vector3 posePath = new();
    Vector3 posePath2 = new();
    Vector3 dir = new();
    Quaternion quaternion = new();
    Vector3 rotationAngles = new();
    Vector3 translation = new();

    List<string> SavedNamesList = new List<string>();

    void Start()
    {

        ROSConnection.GetOrCreateInstance().Subscribe<pc2m>("/com/semantic_pcl", ReceiveMergedMap);
        ROSConnection.GetOrCreateInstance().Subscribe<transformer.TransformationMsg>("/refined_tf", ReceiveTwist);
        ROSConnection.GetOrCreateInstance().Subscribe<pc2m>("/com/downsampled", ReceiveDownsampledMaps);
        ROSConnection.GetOrCreateInstance().Subscribe<Nav.PathMsg>("/plan", ReceiveRobotPath);
        ROSConnection.GetOrCreateInstance().Subscribe<StringMsg>("/com/map_names", ReceiveSavedMapNames);

    }
    
    public void ReceiveMergedMap(pc2m MergedPointCloud)
    {
        incomingPointCloudLive = MergedPointCloud;
        Debug.Log("Map received");   
    }

    public void ReceiveTwist(transformer.TransformationMsg TransformMsg)
    {

        if (TransformMsg.idfrom == 2)
        {
            x = -1 * TransformMsg.tf.linear.x;
            y = -1 * TransformMsg.tf.linear.z;
            z = -1 * TransformMsg.tf.linear.y;
            rx = TransformMsg.tf.angular.x * Mathf.Rad2Deg;
            ry = TransformMsg.tf.angular.z * Mathf.Rad2Deg;
            rz = TransformMsg.tf.angular.y * Mathf.Rad2Deg;

            rotationAngles.Set((float)rx, (float)ry, (float)rz);
            translation.Set((float)x, (float)y, (float)z);

            pub.FirstAlignment = false;
        }
    }

    public void ReceiveDownsampledMaps(pc2m downsampled)
    {
        // The downsampled maps are for aligning purposes

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
            pub.RequestDownsampledMapTo();
        }

    }

    public void ReceiveRobotPath(Nav.PathMsg path)
    {
        // If the node is publishing numerous times for the same path, then it might be better to implement a "check for change" condition. But that is for when we are receiving real time odom updates from the moving robot.
        Debug.Log("Path received");

        //Delete old path points
        if (PathParent != null) Destroy(PathParent);


        //Instantiate new path objects (arrows)
        PathParent = new GameObject("PathParent");
        PathParent.transform.Rotate(rotationAngles, Space.Self);
        PathParent.transform.Translate(translation, Space.Self);
        for (int i = 0; i < path.poses.Length - 1; i++)
        {
            // Get path point
            posePath.Set((float)path.poses[i].pose.position.x, (float)path.poses[i].pose.position.z, (float)path.poses[i].pose.position.y);

            // Get consequent path point
            posePath2.Set((float)path.poses[i + 1].pose.position.x, (float)path.poses[i + 1].pose.position.z, (float)path.poses[i + 1].pose.position.y);
            
            // Get direction between two points to align the path arrow with it
            dir = posePath2 - posePath;

            quaternion.SetFromToRotation(Vector3.forward, dir);

            PathElement = Instantiate(objectToEnable, posePath, quaternion, PathParent.transform);

        }
    }

    public void ReceiveSavedMapNames(StringMsg NameMsg)
    {
        // Creates new button for newly added name in the list
        
        if (!SavedNamesList.Contains(NameMsg.data.ToLower()))
        {
            SavedNamesList.Add(NameMsg.data.ToLower());
            ButtonsForMaps[SavedNamesList.Count - 1].SetActive(true);
            ButtonsForMaps[SavedNamesList.Count - 1].GetComponent<ButtonConfigHelper>().MainLabelText = NameMsg.data.ToLower();
        }
    }
}