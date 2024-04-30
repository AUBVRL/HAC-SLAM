using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using pc2 = RosMessageTypes.Sensor.PointCloud2Msg;

public class MiniMapIncoming : MonoBehaviour
{
    public BoxCollider box;
    Vector3 min, max, zeros, Pose, Rot;
    GameObject kuby;
    public GameObject cubz;

    void Start()
    {
        min = new Vector3(1f, 1f, 1f);
        zeros = new Vector3(0f, 0f, 0f);
        Rot = new Vector3(0f, 0f, 0f);
    }

    private void OnDisable()
    {
        ResetPose();
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

            kuby = Instantiate(cubz, cubePose, Quaternion.identity);
            kuby.transform.SetParent(this.gameObject.transform, false);
            max.x = Mathf.Max(Mathf.Abs(kuby.transform.localPosition.x), max.x);
            max.y = Mathf.Max(Mathf.Abs(kuby.transform.localPosition.y), max.y);
            max.z = Mathf.Max(Mathf.Abs(kuby.transform.localPosition.z), max.z);
        }
        box.size = max * 2 + min;
    }

    public void ResetPose()
    {
        Pose.x = Camera.main.transform.localPosition.x + Mathf.Sin(Camera.main.transform.localRotation.eulerAngles.y * Mathf.Deg2Rad);
        Pose.y = Camera.main.transform.localPosition.y - 0.5f;
        Pose.z = Camera.main.transform.localPosition.z + Mathf.Cos(Camera.main.transform.localRotation.eulerAngles.y * Mathf.Deg2Rad);

        Rot.Set(0, Camera.main.transform.localRotation.eulerAngles.y, 0);

        gameObject.transform.position = Pose;
        gameObject.transform.rotation = Quaternion.Euler(Rot);
    }
}
