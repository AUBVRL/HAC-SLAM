using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Sensor;
using UnityEngine.UI;
using Microsoft.MixedReality.Toolkit.UI;

public class RosSubscriberManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static PointCloud2Msg incomingPointCloudLive;

    public ButtonConfigHelper buttonText;
    void Start()
    {
        ROSConnection.GetOrCreateInstance().Subscribe<PointCloud2Msg>("/com/semantic_pcl", ReceiveMergedMap);
        buttonText.MainLabelText = string.Format("<color=red>{0}</color>","Waiting for PC2Msg");
    }

    // Update is called once per frame
    void ReceiveMergedMap(PointCloud2Msg MergedPointCloud)
    {
        incomingPointCloudLive = MergedPointCloud;
        buttonText.MainLabelText = string.Format("<color=green>{0}</color>","Ready for populating");
        //Debug.Log("Received PointCloud2Msg");
    }
}
