using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using pc2 = RosMessageTypes.Sensor.PointCloud2Msg;
using System;
using TMPro;

public class MergedVoxelDisplay : MonoBehaviour
{
    public GameObject cubz;
    public RosSubscriberExample Sub;
    public GameObject Parent, ImageTarget;
    public static GameObject imageTarget;
    [NonSerialized]
    public float x, y, z, rx, ry, rz;
    bool once = true;
    Vector3 transformed;
    GameObject kuby;
    public Material SelectedMaterial;
    MeshRenderer VoxelMeshRenderer;
    public MinecraftBuilder mcb;
    Coroutine FillIncomingCoroutine;
    Vector3 cameraPosition;
    public TextMeshPro TextMeshPro;


    // Start is called before the first frame update
    void Start()
    {
        imageTarget = ImageTarget;
        x = 0;
        y = 0;
        z = 0;
        rx = 0;
        ry = 0;
        rz = 0;
    }

    IEnumerator FillIncoming(pc2 pointcloud)
    {
        Vector3 point;
        int j;
        int countTillYield = 0;
        Debug.Log("BEGAN VOXELIZING POINT CLOUD");
        for (int i = 0; i < pointcloud.width; i++)
        {
            j = i * Mathf.RoundToInt(pointcloud.point_step);
            point.x = System.BitConverter.ToSingle(pointcloud.data, j);
            point.z = System.BitConverter.ToSingle(pointcloud.data, j + 4);
            point.y = System.BitConverter.ToSingle(pointcloud.data, j + 8);
            VoxelManager.AddVoxel(point, false);
            countTillYield++;
            if (countTillYield % 500 == 0) yield return null;
        }
        Debug.Log("Done");
        PrefabsManager.chunkParentPrefab.transform.position = new Vector3(1.85f, -1.65f, -3.6f);
        PrefabsManager.chunkParentPrefab.transform.eulerAngles = new Vector3(0, 90, 0);
        PrefabsManager.chunkParentPrefab.transform.parent = ImageTarget.transform;
        TextMeshPro.text = "DONE!!";
    }




    public void Clean()
    {
        if (Parent.transform.childCount > 0)
        {
            for (int i = 0; i < Parent.transform.childCount; i++)
            {
                Destroy(Parent.transform.GetChild(i).gameObject);
            }
        }
    }

    public void ShowMergedMap()
    {
        x = (float)Sub.x;
        y = (float)Sub.y;
        z = (float)Sub.z;
        rx = (float)Sub.rx;
        ry = (float)Sub.ry;
        rz = (float)Sub.rz;
        Parent.transform.rotation = Quaternion.identity;
        Parent.transform.position = Vector3.zero;
        Parent.transform.Rotate(new Vector3(0, ry, 0), Space.Self);
        Parent.transform.Translate(new Vector3(x, y, z), Space.Self);
        Clean();
        FillIncomingCoroutine = StartCoroutine(FillIncoming(Sub.incomingPointCloudLive));
    }

}
