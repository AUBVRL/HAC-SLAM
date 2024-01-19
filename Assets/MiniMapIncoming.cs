using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using pc2 = RosMessageTypes.Sensor.PointCloud2Msg;

public class MiniMapIncoming : MonoBehaviour
{
    public BoxCollider box;
    Vector3 min, max, zeros;
    GameObject kuby;
    public GameObject cubz;
    public RosSubscriberExample Sub;
    public RosPublisherExample Pub;
    // Start is called before the first frame update

    void Start()
    {
        min = new Vector3(1f, 1f, 1f);
        zeros = new Vector3(0f, 0f, 0f);
        //Debug.Log(max);
    }

    // Update is called once per frame
    void Update()
    {
        box.size = max * 2 + min;
    }

    public void ShowIncomingMap()
    {
        Clean();
        FillIncoming(Sub.incomingPointCloudDownSampled);
        //FillIncoming(Pub.pc2m);

    }
    public void Clean()
    {
        if (this.gameObject.transform.childCount > 1)
        {
            for (int i = 0; i < this.gameObject.transform.childCount - 1; i++)
            {
                Destroy(this.gameObject.transform.GetChild(i).gameObject);
            }
        }
        max = zeros;
    }

    public void FillIncoming(pc2 pointcloud)
    {

        Vector3 cubePose;
        int j;
        for(int i = 0; i < pointcloud.width; i++)
        {
            j = i * Mathf.RoundToInt(pointcloud.point_step);
            cubePose.x = System.BitConverter.ToSingle(pointcloud.data, j);
            cubePose.z = System.BitConverter.ToSingle(pointcloud.data, j + 4);
            cubePose.y = System.BitConverter.ToSingle(pointcloud.data, j + 8);

            /*cubePose.z = System.BitConverter.ToSingle(pointcloud.data, j);
            cubePose.x = -1 * System.BitConverter.ToSingle(pointcloud.data, j + 4);
            cubePose.y = System.BitConverter.ToSingle(pointcloud.data, j + 8);*/

            kuby = Instantiate(cubz, cubePose, Quaternion.identity);
            kuby.transform.SetParent(this.gameObject.transform, false);
            max.x = Mathf.Max(Mathf.Abs(kuby.transform.localPosition.x), max.x);
            max.y = Mathf.Max(Mathf.Abs(kuby.transform.localPosition.y), max.y);
            max.z = Mathf.Max(Mathf.Abs(kuby.transform.localPosition.z), max.z);
        }
    }

}
