using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using GeometryMsgs = RosMessageTypes.Geometry;
using pc2 = RosMessageTypes.Sensor;
using transformer = RosMessageTypes.CustomInterfaces; //This is the custom message
using _int = RosMessageTypes.Std;
using System.Runtime.InteropServices;
using System;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Examples.Demos;
using RosMessageTypes.Std;

/// <summary>
/// 
/// </summary>
public class RosPublisherExample : MonoBehaviour
{
    
    ROSConnection ros;
    public MinecraftBuilder mcb;
    public GameObject RobotTarget;
    public RosSubscriberExample sub;

    string topicName = "/twist";
    
    string topicName5 = "/point_cloud"; //For publishing point clouds
    string topicName6 = "/human/add"; //For publishing edits
    string topicName7 = "human/delete"; //For publishing deleted
    string topicName8 = "/human/Transformation"; //For ID and twist
    string topicName9 = "/labeled_point_cloud"; //For labeled selection of cubes
    string topicName10 = "/human/downsampled_request"; //For request integer
    string topicName11 = "/human/human_label";
    string SaveTopic = "/human/save_map";
    string RequestNamesTopic = "/human/map_names";
    string LoadMapTopic = "/human/load_map";
    string localizeHumanTopic = "/human/localize";
    string DeleteLabelTopic = "/human/delete_label";
    string DeleteInstanceTopic = "/human/delete_instance";

    float publishMessageFrequency = 3f;

    // Used to determine how much time has elapsed since the last message was published
    float timeElapsed; 
    public bool FirstAlignment;

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
    GeometryMsgs.TwistMsg robot_twist;
    _int.Int16Msg intRequest, deleteLabel;
    _int.StringMsg SaveMapName, LoadMapName;
    _int.BoolMsg RequestNames;

    transformer.TransformationMsg newTwist;
    transformer.InstanceMsg deleteInstance;
    //transformer.LabelPCLMsg labeler;

    uint NewWidth = 0;
    uint NewWidthforEdited = 0;
    uint NewWidthforDeleted = 0;
    byte[] tempData;
    byte[] incomingpc;
    public GameObject global, local;
    
    ButtonConfigHelper ButtonName;
    Vector3 TransformedPose, TransformedRot;
    Quaternion Rotation;
    void Start()
    {
        // Start the ROS connection
        ros = ROSConnection.GetOrCreateInstance();
        
        // Define the ROS topics
        ros.RegisterPublisher<GeometryMsgs.TwistMsg>(topicName);
        
        ros.RegisterPublisher<pc2.PointCloud2Msg>(topicName5);
        ros.RegisterPublisher<pc2.PointCloud2Msg>(topicName6);
        ros.RegisterPublisher<pc2.PointCloud2Msg>(topicName7);
        ros.RegisterPublisher<transformer.TransformationMsg>(topicName8);
        ros.RegisterPublisher<transformer.InstanceMsg>(DeleteInstanceTopic);
        ros.RegisterPublisher<_int.Int16Msg>(DeleteLabelTopic);
        ros.RegisterPublisher<_int.Int16Msg>(topicName10); //For Minimap requests
        ros.RegisterPublisher<pc2.PointCloud2Msg>(topicName11);
        ros.RegisterPublisher<_int.StringMsg>(SaveTopic);
        ros.RegisterPublisher<_int.BoolMsg>(RequestNamesTopic);
        ros.RegisterPublisher<_int.StringMsg>(LoadMapTopic);
        ros.RegisterPublisher<GeometryMsgs.TwistMsg>(localizeHumanTopic);


        //Initializing pointcloud message variable for mapping
        pc2m = new pc2.PointCloud2Msg();
        pc2m.header.frame_id = "map";
        pc2m.header.stamp.nanosec = 2;
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

        

        //Initializing pointcloud message variable for adding
        pc2e = new pc2.PointCloud2Msg();
        pc2e.header.frame_id = "map";
        pc2e.header.stamp.nanosec = 2;
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



        // Initializing pointcloud message variable for deleting
        pc2d = new pc2.PointCloud2Msg();
        pc2d.header.frame_id = "map";
        pc2d.header.stamp.nanosec = 2;
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



        //Initializing pointcloud message variable for labeling
        pc2l = new pc2.PointCloud2Msg();
        pc2l.header.frame_id = "map";
        pc2l.header.stamp.nanosec = 2;
        pc2l.fields = new pc2.PointFieldMsg[]
        {

            new pc2.PointFieldMsg { name = "x", offset = 0, datatype = pc2.PointFieldMsg.FLOAT32, count = 1 },
            new pc2.PointFieldMsg { name = "y", offset = 4, datatype = pc2.PointFieldMsg.FLOAT32, count = 1 },
            new pc2.PointFieldMsg { name = "z", offset = 8, datatype = pc2.PointFieldMsg.FLOAT32, count = 1 },
            new pc2.PointFieldMsg { name = "rgb", offset = 16, datatype = pc2.PointFieldMsg.FLOAT32, count = 1 },
            
            
        };
        pc2l.is_bigendian = false;
        pc2l.point_step = 32;
        pc2l.row_step = pc2l.point_step;
        pc2l.is_dense = true;
        pc2l.width = NewWidthforEdited;
        pc2l.height = 1;
        pc2l.data = new byte[0];



        //Initializing twist message variable for transforms
        twist = new GeometryMsgs.TwistMsg();

        newTwist = new transformer.TransformationMsg();
        intRequest = new _int.Int16Msg();
        SaveMapName = new _int.StringMsg();
        LoadMapName = new _int.StringMsg();
        RequestNames = new _int.BoolMsg();


        FirstAlignment = true;

        deleteLabel = new _int.Int16Msg();
        deleteInstance = new transformer.InstanceMsg();


    }

    private void Update()
    {
        // Publishing every "time elapsed" seconds
        // (Should be an invoked repeating function that could be paused).
        timeElapsed += Time.deltaTime;

        if (timeElapsed > publishMessageFrequency)
        {
            //ros.Publish(topicName5, pc2m);
            ///PopulateMappedPointCloudMsg();
            timeElapsed = 0;
        }


    }

    public void EducatedGuessForICP()
    {
        
        newTwist.idfrom = IDfrom;
        newTwist.idto = IDto;

        newTwist.tf.linear.x = (local.transform.position.x - global.transform.position.x) / global.transform.localScale.x;
        newTwist.tf.linear.y = (local.transform.position.z - global.transform.position.z) / global.transform.localScale.z;
        newTwist.tf.linear.z = (local.transform.position.y - global.transform.position.y) / global.transform.localScale.y;
        newTwist.tf.angular.x = (global.transform.rotation.eulerAngles.x - local.transform.rotation.eulerAngles.x) * Mathf.Deg2Rad;
        newTwist.tf.angular.y = (global.transform.rotation.eulerAngles.z - local.transform.rotation.eulerAngles.z) * Mathf.Deg2Rad;
        newTwist.tf.angular.z = (global.transform.rotation.eulerAngles.y - local.transform.rotation.eulerAngles.y) * Mathf.Deg2Rad;

        ros.Publish(topicName8, newTwist);

    }

    public void PublishAddedPointCloudMsg()
    {
        pc2e.data = mcb.AddedVoxelByte.ToArray();
        pc2e.width = (uint)(mcb.AddedVoxelByte.Count / 12);
        ros.Publish(topicName6, pc2e);
    }

    public void PublishDeletedPointCloudMsg()
    {
        pc2d.data = mcb.DeletedVoxelByte.ToArray();
        pc2d.width = (uint)(mcb.DeletedVoxelByte.Count / 12);
        ros.Publish(topicName7, pc2d);
    }

    public void PopulateMappedPointCloudMsg()
    {

        pc2m.data = mcb.VoxelByte.ToArray();
        pc2m.width = (uint)(mcb.VoxelByte.Count / 12);
    }

    public void LabeledPointCloudPopulater(Vector3 point, byte Label, byte Instance)
    {
        // Needs fixing
        point = (point / 0.05f) * 0.04999999f;
        byte[] laabel = new byte[] { Label };
        byte[] iinstance = new byte[] { Instance };
        tempData = new byte[pc2l.data.Length];
        tempData = pc2l.data;
        pc2l.data = new byte[pc2l.data.Length + 32];
        for (int i = 0; i < tempData.Length; i++)
        {
            pc2l.data[i] = tempData[i];
        }
        byte[] xBytes = System.BitConverter.GetBytes(point.x); // / 0.05f) * 0.0499f);
        byte[] yBytes = System.BitConverter.GetBytes(point.z); // 0.05f) * 0.0499f);
        byte[] zBytes = System.BitConverter.GetBytes(point.y); // 0.05f) * 0.0499f);
        byte[] probaBytes = System.BitConverter.GetBytes(0);

        byte[] one = System.BitConverter.GetBytes(1f);
        byte[] byteArrayZeros = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

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
        pc2l.row_step = (uint)pc2l.data.Length;

    }
    
    public void LabelPublisher()
    {
        ros.Publish(topicName11, pc2l);
    }

    public void RequestDownsampledMapFrom(int x)
    {
        // Referenced in the align menu buttons
        intRequest.data = (short)x;
        ros.Publish(topicName10, intRequest);

    }

    public void RequestDownsampledMapTo()
    {
        if (FirstAlignment)
        {
            intRequest.data = 1;
            ros.Publish(topicName10, intRequest);
        }
        else
        {
            intRequest.data = 0;
            ros.Publish(topicName10, intRequest);
        }
    }

    public void PublishSavedMapName(string name)
    {
        name.ToLower();
        ///////SaveMapName.data = name;
        SaveMapName.data = "savediw";
        ros.Publish(SaveTopic, SaveMapName);
    }

    public void RequestSavedMapNames()
    {
        // Referenced in the "Load Map" button
        RequestNames.data = true;
        ros.Publish(RequestNamesTopic, RequestNames);
    }

    public void RequestSpecificMap(GameObject gameObject)
    {
        ButtonName = gameObject.GetComponent<ButtonConfigHelper>();
        LoadMapName.data = "savediw.bt";
        //LoadMapName.data = ButtonName.MainLabelText;
        ros.Publish(LoadMapTopic, LoadMapName);
    }

    public void HumanLocalizationPublisher()
    {
        // Transform the robot's pose from the local frame to global frame
        TransformedPose = mcb.TransformPCL(RobotTarget.transform.position);

        // Get the rotation between the local and global map
        Rotation = Quaternion.Euler(new Vector3(-(float)sub.rx, -(float)sub.ry, -(float)sub.rz));

        // Apply acquired rotation to the robot rotation
        TransformedRot = Rotation*RobotTarget.transform.localRotation.eulerAngles;

        // Populate and publish the twist message
        robot_twist = new GeometryMsgs.TwistMsg();
        robot_twist.linear.x = TransformedPose.x;
        robot_twist.linear.y = TransformedPose.z;
        robot_twist.linear.z = TransformedPose.y;
        robot_twist.angular.x = TransformedRot.x;
        robot_twist.angular.y = TransformedRot.z;
        robot_twist.angular.z = TransformedRot.y;

        ros.Publish(localizeHumanTopic, robot_twist);
    }

    public void PublishDeleteLabel(byte label)
    {
        deleteLabel.data = label;
        ros.Publish(DeleteLabelTopic, deleteLabel);
    }

    public void PublishDeleteInstance(byte label, byte instance)
    {
        deleteInstance.label = (sbyte) label;
        deleteInstance.instance = (sbyte) instance;
        ros.Publish(DeleteInstanceTopic, deleteInstance);

    }

}