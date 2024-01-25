using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
//using RosColor = RosMessageTypes.UnityRoboticsDemo.UnityColorMsg;
using OGGM = RosMessageTypes.Nav.OccupancyGridMsg;
using Posemsg = RosMessageTypes.Geometry.PoseMsg;
using pc2m = RosMessageTypes.Sensor.PointCloud2Msg;
using twist = RosMessageTypes.Geometry.TwistMsg;
using TMPro;

public class RosSubscriberExample : MonoBehaviour
{
    //public GameObject cube;
    public RosPublisherExample pub;
    public sbyte[] arr;
    public byte[] pcarr;
    public float resol;
    public float heig;
    public float wid;
    public GameObject objectToEnable;
    private Posemsg odom;
    public uint pcwidth;
    public pc2m incomingPointCloudDownSampled;
    public pc2m localPointCloudDownSampled;
    public pc2m incomingPointCloudLive;
    public double x, y, z, rx, ry, rz;
    public MiniMapIncoming mmincom;
    public MiniMap miniMap;
    public TextMeshPro MenuText;
    
    void Start()
    {
        //ROSConnection.GetOrCreateInstance().Subscribe<RosColor>("color", ColorChange);
        ROSConnection.GetOrCreateInstance().Subscribe<OGGM>("/mapToUnity", Ocupo);
        ROSConnection.GetOrCreateInstance().Subscribe<pc2m>("/robot_map_downsampled", pointCloud); // No need for them anymore
        ROSConnection.GetOrCreateInstance().Subscribe<pc2m>("/local_map_downsampled", localPointCloud);
        ROSConnection.GetOrCreateInstance().Subscribe<pc2m>("/com/semantic_pcl", pointCloudLive);
        ROSConnection.GetOrCreateInstance().Subscribe<twist>("/trans_topic_merger", twistReceived);
        ROSConnection.GetOrCreateInstance().Subscribe<pc2m>("/com/downsampled", pointCloudDownsampled);
        ROSConnection.GetOrCreateInstance().Subscribe<pc2m>("/human/human_label", pointCloudDownsampledTest);
        

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
        Debug.Log(ptcldlive.data[17]);
    }
    public void twistReceived(twist Twisty)
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


        x = -1 * Twisty.linear.x;
        y = -1 * Twisty.linear.z;
        z = -1 * Twisty.linear.y;
        rx = 0; //1 * Twisty.angular.x * Mathf.Rad2Deg;
        ry = 1 * Twisty.angular.z * Mathf.Rad2Deg;
        rz = 0; // 1 * Twisty.angular.y * Mathf.Rad2Deg; //was -1

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

    
}
