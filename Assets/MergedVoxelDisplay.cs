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
    //public float x, y, z, rx, ry, rz;
    bool once = true;
    Vector3 transformed;
    GameObject kuby;
    public Material SelectedMaterial;
    MeshRenderer VoxelMeshRenderer;
    public MinecraftBuilder mcb;
    Coroutine FillIncomingCoroutine;
    Vector3 cameraPosition;
    public TextMeshPro TextMeshPro;
    Vector3 ImageTargetTranslation, ImageTargetRotation, PointCloudTranslation, PointCloudRotation, InitialPose;
    public Vector3 rotation,pose, orientation;
    public GameObject iwhub;
    public TextMeshPro teext;

    // Start is called before the first frame update
    void Start()
    {
        imageTarget = ImageTarget;
        /*x = 0;
        y = 0;
        z = 0;
        rx = 0;
        ry = 0;
        rz = 0;*/
        PointCloudTranslation = new(1.85f, -1.65f, -3.6f);
        PointCloudRotation = new(0, 90f, 0);
        InitialPose = iwhub.transform.position;
    }

    private void Update()
    {
        ImageTargetTranslation = imageTarget.transform.position;
        
        //Debug.Log(ImageTargetTranslation);
        //Quaternion r = imageTarget.transform.rotation;

        Vector3 flatten = new(imageTarget.transform.up.x, 0f, imageTarget.transform.up.z);


        //print(imageTarget.transform.rotation.eulerAngles.y);
        //print(imageTarget.transform.localEulerAngles.y);
        //ImageTargetRotation.Set(ImageTarget.transform.eulerAngles.x, ImageTarget.transform.eulerAngles.z, -ImageTarget.transform.eulerAngles.y);
        Debug.Log(Vector3.Angle(flatten, Vector3.forward));
        float angle = Vector3.Angle(flatten,Vector3.forward);
        float angle2 = Mathf.Atan2(imageTarget.transform.up.x, imageTarget.transform.up.z) * Mathf.Rad2Deg;
        ImageTargetRotation = new(0f, angle2, 0f);
        //float angle = Mathf.Atan2(imageTarget.transform.up.z, imageTarget.transform.up.x);
        //Debug.Log(angle * Mathf.Rad2Deg);
        //imageTarget.transform.forward = v;
        iwhub.transform.position = TransformToImageTarget(InitialPose);
        teext.text = imageTarget.transform.eulerAngles.ToString();
        /*Vector3 hamburger = TransformToImageTarget(HamburgerPosition);
        Vector3 iamgetargetrotnew = new Vector3(0, imageTarget.transform.eulerAngles.z, 0);
        PointCloudTranslation.Set(hamburger.x + PointCloudTranslation.x * Mathf.Cos(imageTarget.transform.eulerAngles.z) - PointCloudTranslation.z * Mathf.Sin(imageTarget.transform.eulerAngles.z),
                                  hamburger.y + PointCloudTranslation.y,
                                  hamburger.z + PointCloudTranslation.z * Mathf.Sin(imageTarget.transform.eulerAngles.z) + PointCloudTranslation.x * Mathf.Cos(imageTarget.transform.eulerAngles.z));

        Hamburger.transform.position = TransformToPointCloud(HamburgerPosition);
        //Vector3 pose = iwhub.transform.position;
        Vector3 rot = Quaternion.Euler(rotation) * pose;
        Vector3 orient = Quaternion.Euler(rotation) * orientation;
        print(orient);
        //iwhub.transform.position = rot;
        iwhub.transform.rotation = Quaternion.Euler(orient); // Quaternion.Euler(orient.x, orient.y, orient.z);*/



    }

    public Vector3 TransformToImageTarget(Vector3 Point)
    {
        Quaternion rotationQuaternion = Quaternion.Euler(ImageTargetRotation);
        Point = rotationQuaternion * Point + ImageTargetTranslation;
        return Point;
    }

    public Vector3 TransformToPointCloud(Vector3 Point)
    {
        Quaternion rotationQuaternion = Quaternion.Euler(PointCloudRotation);
        Point = rotationQuaternion * Point + PointCloudTranslation;
        return Point;
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
        /*x = (float)Sub.x;
        y = (float)Sub.y;
        z = (float)Sub.z;
        rx = (float)Sub.rx;
        ry = (float)Sub.ry;
        rz = (float)Sub.rz;
        Parent.transform.rotation = Quaternion.identity;
        Parent.transform.position = Vector3.zero;
        Parent.transform.Rotate(new Vector3(0, ry, 0), Space.Self);
        Parent.transform.Translate(new Vector3(x, y, z), Space.Self);*/
        Clean();
        FillIncomingCoroutine = StartCoroutine(FillIncoming(Sub.incomingPointCloudLive));
    }

}
