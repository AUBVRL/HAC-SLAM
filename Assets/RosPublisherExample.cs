using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.UnityRoboticsDemo;
using GeometryMsgs = RosMessageTypes.Geometry;
using OGM = RosMessageTypes.Nav; //new
using pics = RosMessageTypes.Sensor; //ktir new (nicolas)
using octom = RosMessageTypes.Octomap; //ktir new (3D)
using pc2 = RosMessageTypes.Sensor;
using transformer = RosMessageTypes.CustomInterfaces; //This is the custom message
using _int = RosMessageTypes.Std;
using System.Runtime.InteropServices;
using System;

/// <summary>
/// 
/// </summary>
public class RosPublisherExample : MonoBehaviour
{

    ROSConnection ros;
    public MinecraftBuilder mcb;

    string topicName = "/twist"; // It was "/joy_teleop/cmd_vel" when we were publishing to control the robot movement
    string topicName2 = "/occupancy_map"; // For 2D mapping
    //string topicName3 = "/sowar"; //For FSLAM. To be tried later
    //string topicName4 = "/octomap"; //To be used to publish Octomaps
    string topicName5 = "/point_cloud"; //For publishing point clouds
    string topicName6 = "/human_edits"; //For publishing edits
    string topicName7 = "/deleted"; //For publishing deleted
    string topicName8 = "/human/Transformation"; //For ID and twist
    string topicName9 = "/labeled_point_cloud"; //For labeled selection of cubes
    string topicName10 = "/human/downsampled_request"; //For request integer
    string topicName11 = "/human/human_label";
    //Texture2D image; //For FSLAM. To be tried later

    float publishMessageFrequency = 3f;

    // Used to determine how much time has elapsed since the last message was published
    float timeElapsed;
    
    bool PublishTwist;

    octom.OctomapMsg octo; //ktir new (3D)
    [NonSerialized]
    public pc2.PointCloud2Msg pc2m;
    [NonSerialized]
    public int IDfrom;
    [NonSerialized]
    public int IDto;
    pc2.PointCloud2Msg pc2e;
    pc2.PointCloud2Msg pc2d;
    pc2.PointCloud2Msg pc2l;
    GeometryMsgs.TwistMsg twist;
    _int.Int16Msg intRequest;
    

    transformer.TransformationMsg newTwist;
    //transformer.LabelPCLMsg labeler;

    uint NewWidth = 0;
    uint NewWidthforEdited = 0;
    uint NewWidthforDeleted = 0;
    byte[] tempData;
    byte[] incomingpc;
    public GameObject global, local;
    bool yalla;
    void Start()
    {
        // start the ROS connection
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<GeometryMsgs.TwistMsg>(topicName);
        ros.RegisterPublisher<OGM.OccupancyGridMsg>(topicName2);
        //ros.RegisterPublisher<octom.OctomapMsg>(topicName4);
        ros.RegisterPublisher<pc2.PointCloud2Msg>(topicName5);
        ros.RegisterPublisher<pc2.PointCloud2Msg>(topicName6);
        ros.RegisterPublisher<pc2.PointCloud2Msg>(topicName7);

        // This is not initialized on ros tcp. Wait for malak
        //ros.RegisterPublisher<transformer.TransformationMsg>(topicName8); 
        ros.RegisterPublisher<_int.Int16Msg>(topicName10); //For Minimap requests
        ros.RegisterPublisher<pc2.PointCloud2Msg>(topicName11);

        //The below is for the robot rotation 
        PublishTwist = false;
        yalla = false;
        //The below is for the Octomap
        /*octo = new octom.OctomapMsg();
        octo.header.frame_id = "map";
        octo.id = "octo";
        octo.resolution = 0.5;
        octo.binary = false;
        sbyte[] octoData = new sbyte[16];
        octoData[0] = 0;*/


        //for point cloud 2:
        pc2m = new pc2.PointCloud2Msg();
        pc2m.header.frame_id = "map";
        pc2m.fields = new pc2.PointFieldMsg[]
        {
            new pc2.PointFieldMsg { name = "x", offset = 0, datatype = pc2.PointFieldMsg.FLOAT32, count = 1 },
            new pc2.PointFieldMsg { name = "y", offset = 4, datatype = pc2.PointFieldMsg.FLOAT32, count = 1 },
            new pc2.PointFieldMsg { name = "z", offset = 8, datatype = pc2.PointFieldMsg.FLOAT32, count = 1 }
        };
        pc2m.is_bigendian = false;
        pc2m.point_step = 12;
        pc2m.row_step = pc2m.point_step;
        pc2m.is_dense = true;
        pc2m.width = NewWidth;
        pc2m.height = 1;
        pc2m.data = new byte[0];

        newTwist = new transformer.TransformationMsg();

        //For edited point clouds
        pc2e = new pc2.PointCloud2Msg();
        pc2e.header.frame_id = "map";
        pc2e.fields = new pc2.PointFieldMsg[]
        {
            new pc2.PointFieldMsg { name = "x", offset = 0, datatype = pc2.PointFieldMsg.FLOAT32, count = 1 },
            new pc2.PointFieldMsg { name = "y", offset = 4, datatype = pc2.PointFieldMsg.FLOAT32, count = 1 },
            new pc2.PointFieldMsg { name = "z", offset = 8, datatype = pc2.PointFieldMsg.FLOAT32, count = 1 }
        };
        pc2e.is_bigendian = false;
        pc2e.point_step = 12;
        pc2e.row_step = pc2e.point_step;
        pc2e.is_dense = true;
        pc2e.width = NewWidthforEdited;
        pc2e.height = 1;
        pc2e.data = new byte[0];




        //For deleted point clouds:
        pc2d = new pc2.PointCloud2Msg();
        pc2d.header.frame_id = "map";
        pc2d.fields = new pc2.PointFieldMsg[]
        {
            new pc2.PointFieldMsg { name = "x", offset = 0, datatype = pc2.PointFieldMsg.FLOAT32, count = 1 },
            new pc2.PointFieldMsg { name = "y", offset = 4, datatype = pc2.PointFieldMsg.FLOAT32, count = 1 },
            new pc2.PointFieldMsg { name = "z", offset = 8, datatype = pc2.PointFieldMsg.FLOAT32, count = 1 }
        };
        pc2d.is_bigendian = false;
        pc2d.point_step = 12;
        pc2d.row_step = pc2e.point_step;
        pc2d.is_dense = true;
        pc2d.width = NewWidthforEdited;
        pc2d.height = 1;
        pc2d.data = new byte[0];



        //For labeled point clouds:
        pc2l = new pc2.PointCloud2Msg();
        pc2l.header.frame_id = "map";
        pc2l.header.stamp.nanosec = 2;
        pc2l.fields = new pc2.PointFieldMsg[]
        {

            new pc2.PointFieldMsg { name = "x", offset = 0, datatype = pc2.PointFieldMsg.FLOAT32, count = 1 },
            new pc2.PointFieldMsg { name = "y", offset = 4, datatype = pc2.PointFieldMsg.FLOAT32, count = 1 },
            new pc2.PointFieldMsg { name = "z", offset = 8, datatype = pc2.PointFieldMsg.FLOAT32, count = 1 },
            new pc2.PointFieldMsg { name = "rgb", offset = 16, datatype = pc2.PointFieldMsg.FLOAT32, count = 1 },
            



            /*new pc2.PointFieldMsg { name = "x", offset = 0, datatype = pc2.PointFieldMsg.FLOAT32, count = 1 },
            new pc2.PointFieldMsg { name = "y", offset = 4, datatype = pc2.PointFieldMsg.FLOAT32, count = 1 },
            new pc2.PointFieldMsg { name = "z", offset = 8, datatype = pc2.PointFieldMsg.FLOAT32, count = 1 },
            new pc2.PointFieldMsg { name = "one", offset = 12, datatype = pc2.PointFieldMsg.FLOAT32, count = 1 },
            new pc2.PointFieldMsg { name = "r", offset = 16, datatype = pc2.PointFieldMsg.UINT8, count = 1 },
            new pc2.PointFieldMsg { name = "g", offset = 17, datatype = pc2.PointFieldMsg.UINT8, count = 1 },
            new pc2.PointFieldMsg { name = "b", offset = 18, datatype = pc2.PointFieldMsg.UINT8, count = 1 },
            new pc2.PointFieldMsg { name = "z1", offset = 19, datatype = pc2.PointFieldMsg.FLOAT32, count = 1 },
            new pc2.PointFieldMsg { name = "z2", offset = 23, datatype = pc2.PointFieldMsg.FLOAT32, count = 1 },
            new pc2.PointFieldMsg { name = "z3", offset = 27, datatype = pc2.PointFieldMsg.INT16, count = 1 },
            new pc2.PointFieldMsg { name = "z4", offset = 31, datatype = pc2.PointFieldMsg.UINT8, count = 1 },*/
            
            
        };
        pc2l.is_bigendian = false;
        pc2l.point_step = 32;
        pc2l.row_step = pc2l.point_step;
        pc2l.is_dense = true;
        pc2l.width = NewWidthforEdited;
        pc2l.height = 1;
        pc2l.data = new byte[0];

        

        /*pc2l = new pc2.PointCloud2Msg();
        pc2l.header.frame_id = "map";
        pc2l.header.stamp.nanosec = 2;
        pc2l.fields = new pc2.PointFieldMsg[]
        {
            new pc2.PointFieldMsg { name = "x", offset = 0, datatype = pc2.PointFieldMsg.FLOAT32, count = 1 },
            new pc2.PointFieldMsg { name = "y", offset = 4, datatype = pc2.PointFieldMsg.FLOAT32, count = 1 },
            new pc2.PointFieldMsg { name = "z", offset = 8, datatype = pc2.PointFieldMsg.FLOAT32, count = 1 },
            new pc2.PointFieldMsg { name = "r", offset = 12, datatype = pc2.PointFieldMsg.UINT8, count = 1 },
            new pc2.PointFieldMsg { name = "g", offset = 13, datatype = pc2.PointFieldMsg.UINT8, count = 1 },
            new pc2.PointFieldMsg { name = "b", offset = 14, datatype = pc2.PointFieldMsg.UINT8, count = 1 },
            
            
            *//*new pc2.PointFieldMsg { name = "proba", offset = 12, datatype = pc2.PointFieldMsg.UINT8, count = 1 },
            new pc2.PointFieldMsg { name = "label", offset = 13, datatype = pc2.PointFieldMsg.UINT8, count = 1 },
            new pc2.PointFieldMsg { name = "instance", offset = 14, datatype = pc2.PointFieldMsg.UINT8, count = 1 },*//*
        };
        pc2l.is_bigendian = false;
        pc2l.point_step = 15;
        pc2l.row_step = pc2e.point_step;
        pc2l.is_dense = true;
        pc2l.width = NewWidthforEdited;
        pc2l.height = 1;
        pc2l.data = new byte[0];*/



        //For twist messages:
        twist = new GeometryMsgs.TwistMsg();

        intRequest = new _int.Int16Msg();

    }

    private void Update()
    {

        timeElapsed += Time.deltaTime;
        
        if (timeElapsed > publishMessageFrequency) // && yalla == true) //new
        {
            ros.Publish(topicName5, pc2m);
            PopulatePointCloudMsg();
            timeElapsed = 0;
        }


        /*if (PublishTwist == true)
        {

            twist.linear.x = 0.0f;
            twist.linear.y = 1.0f;
            twist.linear.z = 0.0f;
            twist.angular.x = 0.0f;
            twist.angular.y = 0.0f;
            twist.angular.z = 1.57f;
            ros.Publish(topicName, twist);

            PublishTwist = false;
        }*/

        
    }

    public void AddPointCloudtoROSMessage(Vector3 point)
    {
        tempData = new byte[pc2m.data.Length];
        tempData = pc2m.data;
        pc2m.data = new byte[pc2m.data.Length + 12];
        for (int i = 0; i < tempData.Length; i++)
        {
            pc2m.data[i] = tempData[i];
        }
        byte[] xBytes = System.BitConverter.GetBytes(point.x);
        byte[] yBytes = System.BitConverter.GetBytes(point.z);
        byte[] zBytes = System.BitConverter.GetBytes(point.y);

        int offset = tempData.Length;
        System.Buffer.BlockCopy(xBytes, 0, pc2m.data, offset, 4);
        System.Buffer.BlockCopy(yBytes, 0, pc2m.data, offset + 4, 4);
        System.Buffer.BlockCopy(zBytes, 0, pc2m.data, offset + 8, 4);

        NewWidth++;
        pc2m.width = NewWidth;
        
    }

    public void EditedPointCloudPublisher(Vector3 point)
    {

        tempData = new byte[pc2e.data.Length];
        tempData = pc2e.data;
        pc2e.data = new byte[pc2e.data.Length + 12];
        for (int i = 0; i < tempData.Length; i++)
        {
            pc2e.data[i] = tempData[i];
        }
        byte[] xBytes = System.BitConverter.GetBytes(point.x);
        byte[] yBytes = System.BitConverter.GetBytes(point.z);
        byte[] zBytes = System.BitConverter.GetBytes(point.y);

        int offset = tempData.Length;
        System.Buffer.BlockCopy(xBytes, 0, pc2e.data, offset, 4);
        System.Buffer.BlockCopy(yBytes, 0, pc2e.data, offset + 4, 4);
        System.Buffer.BlockCopy(zBytes, 0, pc2e.data, offset + 8, 4);

        NewWidthforEdited++;
        pc2e.width = NewWidthforEdited;

    }

    public void DeletedPointCloudPublisher(Vector3 point)
    {
        tempData = new byte[pc2d.data.Length];
        tempData = pc2d.data;
        pc2d.data = new byte[pc2d.data.Length + 12];
        for (int i = 0; i < tempData.Length; i++)
        {
            pc2d.data[i] = tempData[i];
        }
        byte[] xBytes = System.BitConverter.GetBytes(point.x);
        byte[] yBytes = System.BitConverter.GetBytes(point.z);
        byte[] zBytes = System.BitConverter.GetBytes(point.y);

        int offset = tempData.Length;
        System.Buffer.BlockCopy(xBytes, 0, pc2d.data, offset, 4);
        System.Buffer.BlockCopy(yBytes, 0, pc2d.data, offset + 4, 4);
        System.Buffer.BlockCopy(zBytes, 0, pc2d.data, offset + 8, 4);

        NewWidthforDeleted++;
        pc2d.width = NewWidthforDeleted;
    }
    
    public void EducatedGuessForICP()
    {
        newTwist.idfrom = IDfrom;
        newTwist.idto = IDto;
        /*newTwist.tf.linear.x = 1;
        newTwist.tf.linear.y = 2;
        newTwist.tf.linear.z = 3;
        newTwist.tf.angular.x = 4;
        newTwist.tf.angular.y = 5;
        newTwist.tf.angular.z = 6;*/
        newTwist.tf.linear.x = -1 * (global.transform.position.x - local.transform.position.x) / global.transform.localScale.x;
        newTwist.tf.linear.y = -1 * (global.transform.position.z - local.transform.position.z) / global.transform.localScale.z;
        newTwist.tf.linear.z = -1 * (global.transform.position.y - local.transform.position.y) / global.transform.localScale.y;
        newTwist.tf.angular.x = (global.transform.rotation.eulerAngles.z - local.transform.rotation.eulerAngles.z) * Mathf.Deg2Rad;
        newTwist.tf.angular.y = (local.transform.rotation.eulerAngles.x - global.transform.rotation.eulerAngles.x) * Mathf.Deg2Rad;
        newTwist.tf.angular.z = (global.transform.rotation.eulerAngles.y - local.transform.rotation.eulerAngles.y) * Mathf.Deg2Rad;


        /* twist.linear.x = -1 * (global.transform.position.x - local.transform.position.x) / global.transform.localScale.x; //(global.transform.position.z - local.transform.position.z) / global.transform.localScale.z;
         twist.linear.y = -1 * (global.transform.position.z - local.transform.position.z) / global.transform.localScale.z;
         twist.linear.z = -1 * (global.transform.position.y - local.transform.position.y) / global.transform.localScale.y;
         twist.angular.x = (global.transform.rotation.eulerAngles.z - local.transform.rotation.eulerAngles.z) * Mathf.Deg2Rad;
         twist.angular.y = (local.transform.rotation.eulerAngles.x - global.transform.rotation.eulerAngles.x) * Mathf.Deg2Rad;
         twist.angular.z = (global.transform.rotation.eulerAngles.y - local.transform.rotation.eulerAngles.y) * Mathf.Deg2Rad;*/

        /*twist.linear.x = -1 * (global.transform.position.z - local.transform.position.z) / global.transform.localScale.z; //(global.transform.position.z - local.transform.position.z) / global.transform.localScale.z;
        twist.linear.y = -1 * (global.transform.position.x - local.transform.position.x) / global.transform.localScale.x;
        twist.linear.z = -1 * (global.transform.position.y - local.transform.position.y) / global.transform.localScale.y;
        twist.angular.x = (global.transform.rotation.eulerAngles.z - local.transform.rotation.eulerAngles.z) * Mathf.Deg2Rad;
        twist.angular.y = (local.transform.rotation.eulerAngles.x - global.transform.rotation.eulerAngles.x) * Mathf.Deg2Rad;
        twist.angular.z = (global.transform.rotation.eulerAngles.y - local.transform.rotation.eulerAngles.y) * Mathf.Deg2Rad;*/

        //ros.Publish(topicName, twist);
        
        //Not initialized for ros tcp . wait for malak
        //ros.Publish(topicName8, newTwist);
        
        
        //PublishTwist = !PublishTwist;
    }

    public void PublishEditedPointCloudMsg()
    {
        yalla = true;
        ros.Publish(topicName6, pc2e);
    }

    public void PublishDeletedPointCloudMsg()
    {
        ros.Publish(topicName7, pc2d);
    }

    public void PopulatePointCloudMsg()
    {
        pc2m.data = new byte[mcb.Papa.transform.childCount * 12];

        for (int i = 0; i < mcb.Papa.transform.childCount; i++)
        {
            byte[] xBytes = System.BitConverter.GetBytes(mcb.Papa.transform.GetChild(i).transform.position.x);
            byte[] yBytes = System.BitConverter.GetBytes(mcb.Papa.transform.GetChild(i).transform.position.z);
            byte[] zBytes = System.BitConverter.GetBytes(mcb.Papa.transform.GetChild(i).transform.position.y);

            int offset = i * 12;
            System.Buffer.BlockCopy(xBytes, 0, pc2m.data, offset, 4);
            System.Buffer.BlockCopy(yBytes, 0, pc2m.data, offset + 4, 4);
            System.Buffer.BlockCopy(zBytes, 0, pc2m.data, offset + 8, 4);
        }

        pc2m.width = (uint)mcb.Papa.transform.childCount;
    }

    public void PublishRequest3(int x)
    {
        intRequest.data = (short)x;
        ros.Publish(topicName10, intRequest);
    }

    public void LabeledPointCloudPopulater(Vector3 point, byte Label, byte Instance)
    {
        byte[] laabel = new byte[] { Label };
        byte[] iinstance = new byte[] { Instance };
        tempData = new byte[pc2l.data.Length];
        tempData = pc2l.data;
        pc2l.data = new byte[pc2l.data.Length + 32];
        for (int i = 0; i < tempData.Length; i++)
        {
            pc2l.data[i] = tempData[i];
        }
        byte[] xBytes = System.BitConverter.GetBytes(point.x);
        byte[] yBytes = System.BitConverter.GetBytes(point.z);
        byte[] zBytes = System.BitConverter.GetBytes(point.y);
        byte[] probaBytes = System.BitConverter.GetBytes(0);
        

        
        
        Debug.Log("iinstance: " + iinstance[0]);
        /*byte[] labelBytes = System.BitConverter.GetBytes(77);
        byte[] instanceBytes = System.BitConverter.GetBytes(66)*/;

        byte[] one = System.BitConverter.GetBytes(1f);
        byte[] byteArrayZeros = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};

        int offset = tempData.Length;
        System.Buffer.BlockCopy(xBytes, 0, pc2l.data, offset, 4);
        System.Buffer.BlockCopy(yBytes, 0, pc2l.data, offset + 4, 4);
        System.Buffer.BlockCopy(zBytes, 0, pc2l.data, offset + 8, 4);
        System.Buffer.BlockCopy(one, 0, pc2l.data, offset + 12, 4);
        /*System.Buffer.BlockCopy(instanceBytes, 0, pc2l.data, offset + 16, 1);
        System.Buffer.BlockCopy(labelBytes, 0, pc2l.data, offset + 17, 1);*/

        System.Buffer.BlockCopy(iinstance, 0, pc2l.data, offset + 16, 1);
        System.Buffer.BlockCopy(laabel, 0, pc2l.data, offset + 17, 1);

        System.Buffer.BlockCopy(probaBytes, 0, pc2l.data, offset + 18, 1);
        System.Buffer.BlockCopy(byteArrayZeros, 0, pc2l.data, offset + 19, 13);


        NewWidthforEdited++;
        pc2l.width = NewWidthforEdited;
        pc2l.row_step = (uint) pc2l.data.Length;

    }

    public void LabelPublisher()
    {
        ros.Publish(topicName11, pc2l);
        //pc2l.data = new byte[0];
    }

}
