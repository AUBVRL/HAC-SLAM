using System.Collections;
using System.Collections.Generic;
using Unity.Robotics.ROSTCPConnector;
using UnityEngine;

public class RosPublisherManager : MonoBehaviour
{
    ROSConnection ros;
    string pointCloudTopic = "/map";
    float publishRate = 3f; //Rate at which the point cloud is published
    RosMessageTypes.Sensor.PointCloud2Msg pointCloudMsg;
    // Start is called before the first frame update
    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();

        ros.RegisterPublisher<RosMessageTypes.Sensor.PointCloud2Msg>(pointCloudTopic);
        //Check how to populate the constructor later
        pointCloudMsg = new RosMessageTypes.Sensor.PointCloud2Msg();
        pointCloudMsg.header.frame_id = "map";
        pointCloudMsg.header.stamp.nanosec = 2;
        pointCloudMsg.fields = new RosMessageTypes.Sensor.PointFieldMsg[]
        {
            new RosMessageTypes.Sensor.PointFieldMsg { name = "x", offset = 0, datatype = RosMessageTypes.Sensor.PointFieldMsg.FLOAT32, count = 1 },
            new RosMessageTypes.Sensor.PointFieldMsg { name = "y", offset = 4, datatype = RosMessageTypes.Sensor.PointFieldMsg.FLOAT32, count = 1 },
            new RosMessageTypes.Sensor.PointFieldMsg { name = "z", offset = 8, datatype = RosMessageTypes.Sensor.PointFieldMsg.FLOAT32, count = 1 }
        };
        pointCloudMsg.is_bigendian = false;
        pointCloudMsg.point_step = 12;
        pointCloudMsg.row_step = pointCloudMsg.point_step;
        pointCloudMsg.is_dense = true;
        pointCloudMsg.width = 0;
        pointCloudMsg.height = 1;
        pointCloudMsg.data = new byte[0];
    }

    public void PublishVoxels()
    {
        List<byte> byteList = new();
        foreach (var chunk in VoxelManager.ChunksDict.Values)
        {
            byteList.AddRange(chunk.GetChunkByteData());
        }
        pointCloudMsg.data = byteList.ToArray();
        pointCloudMsg.width = (uint)byteList.Count / 12;
        ros.Publish(pointCloudTopic, pointCloudMsg);
    }
}