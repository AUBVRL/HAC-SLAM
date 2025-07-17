using System.Collections;
using System.Collections.Generic;
using Unity.Robotics.ROSTCPConnector;
using UnityEngine;
//using transformer = RosMessageTypes.CustomInterfaces;

public class RosPublisherManager : MonoBehaviour
{
    ROSConnection ros;
    string mappedVoxelsTopic = "/point_cloud"; //For publishing the mapped voxels as point cloud
    string addedVoxelsTopic = "/human/add"; //For publishing added voxels
    string deletedVoxelsTopic = "human/delete"; //For publishing deleted voxels
    float publishRate = 3f; //Rate at which the point cloud is published
    RosMessageTypes.Sensor.PointCloud2Msg mappedPointCloud;
    //transformer.InstanceMsg custom;
    float timeElapsed = 0.0f;
    int publishMessageFrequency = 3;
    RosMessageTypes.Std.Int16Msg integer;


    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();

        ros.RegisterPublisher<RosMessageTypes.Sensor.PointCloud2Msg>(mappedVoxelsTopic);
        //ros.RegisterPublisher<transformer.InstanceMsg>("/com/delete_instance");
        ros.RegisterPublisher<RosMessageTypes.Std.Int16Msg>("/integer");
        //Check how to populate the constructor later
        mappedPointCloud = new RosMessageTypes.Sensor.PointCloud2Msg();
        mappedPointCloud.header.frame_id = "map";
        mappedPointCloud.header.stamp.nanosec = 2;
        mappedPointCloud.fields = new RosMessageTypes.Sensor.PointFieldMsg[]
        {
            new RosMessageTypes.Sensor.PointFieldMsg { name = "x", offset = 0, datatype = RosMessageTypes.Sensor.PointFieldMsg.FLOAT32, count = 1 },
            new RosMessageTypes.Sensor.PointFieldMsg { name = "y", offset = 4, datatype = RosMessageTypes.Sensor.PointFieldMsg.FLOAT32, count = 1 },
            new RosMessageTypes.Sensor.PointFieldMsg { name = "z", offset = 8, datatype = RosMessageTypes.Sensor.PointFieldMsg.FLOAT32, count = 1 }
        };
        mappedPointCloud.is_bigendian = false;
        mappedPointCloud.point_step = 12;
        mappedPointCloud.row_step = mappedPointCloud.point_step;
        mappedPointCloud.is_dense = true;
        mappedPointCloud.width = 0;
        mappedPointCloud.height = 1;
        mappedPointCloud.data = new byte[0];

        //custom = new transformer.InstanceMsg();
        //custom.label = 1;
        //custom.instance = 2;

        integer = new RosMessageTypes.Std.Int16Msg(42);
    }

    private void Update()
    {
        timeElapsed += Time.deltaTime;

        if (timeElapsed > publishMessageFrequency) // && yalla == true) //new
        {
            // ros.Publish("/com/delete_instance", custom);
            ros.Publish("/integer", integer);
            timeElapsed = 0;
        }
    }

    public void PublishMappedVoxels()
    {
        List<byte> byteList = new();
        foreach (var chunk in VoxelManager.ChunksDict.Values)
        {
            byteList.AddRange(chunk.GetChunkByteData());
        }
        mappedPointCloud.data = byteList.ToArray();
        mappedPointCloud.width = (uint)byteList.Count / 12;
        ros.Publish(mappedVoxelsTopic, mappedPointCloud);
    }


}