using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using pc2 = RosMessageTypes.Sensor.PointCloud2Msg;
using System;
using TMPro;

public class MergedVoxelDisplay : MonoBehaviour
{
    public LabelerFingerPose labelerFingerPose;
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
    public GameObject redCube, greenCube, blueCube;
    public float xTotal, yTotal, zTotal, xRotationTotal, yRotationTotal, zRotationTotal;
    public GameObject newImage, manualAlign;
    Vector3 originPosition, originRight, originUp, originForward;
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
        Debug.Log(InitialPose);
        xTotal = 0;
        yTotal = 0;
        zTotal = 0;
        newImage = new GameObject("newImage");
    }

    private void Update()
    {

    }

    public void LabelImageTarget()
    {
        labelerFingerPose.AssetToolTip2(ImageTarget.transform.position, ImageTarget.transform.eulerAngles);
    }

    IEnumerator FillIncoming(pc2 pointcloud)
    {
        TextMeshPro.text = "Loading... 0%";
        GameObject dummyObject = new GameObject("dummyObject");
        dummyObject.transform.position = new Vector3(1.85f, -1.65f, -3.6f);
        dummyObject.transform.eulerAngles = new Vector3(0, 90, 0);        

        newImage.transform.position = ImageTarget.transform.position;
        newImage.transform.up = ImageTarget.transform.forward;
        newImage.transform.right = ImageTarget.transform.right;
        newImage.transform.forward = -ImageTarget.transform.up;
        originPosition = dummyObject.transform.TransformPoint(new Vector3(0, 0, 0)); // local coordinates of point cloud origin wrt to image target
        originPosition = newImage.transform.TransformPoint(originPosition); // global coordinates of point cloud origin

        originRight = dummyObject.transform.TransformPoint(new Vector3(1, 0, 0));
        originRight = newImage.transform.TransformPoint(originRight);
        originRight = originRight - originPosition;

        originUp = dummyObject.transform.TransformPoint(new Vector3(0, 1, 0));
        originUp = newImage.transform.TransformPoint(originUp);
        originUp = originUp - originPosition;

        originForward = dummyObject.transform.TransformPoint(new Vector3(0, 0, 1));
        originForward = newImage.transform.TransformPoint(originForward);
        originForward = originForward - originPosition;

        GameObject manualAlign = new GameObject("manualAlign");
        // rotate in the right order z x y
        manualAlign.transform.Rotate(originForward, -2f);
        manualAlign.transform.Rotate(originRight, -2f);
        manualAlign.transform.Rotate(originUp, -2f);

        // manualAlign.transform.Translate(-0.3f * originUp);

        Vector3 point, globalPoint;
        int j;
        int countTillYield = 0;
        for (int i = 0; i < pointcloud.width; i++)
        {
            j = i * Mathf.RoundToInt(pointcloud.point_step);
            point.x = System.BitConverter.ToSingle(pointcloud.data, j);
            point.z = System.BitConverter.ToSingle(pointcloud.data, j + 4);
            point.y = System.BitConverter.ToSingle(pointcloud.data, j + 8);
            globalPoint = dummyObject.transform.TransformPoint(point); // local coordinates of point with respect to image target
            globalPoint = newImage.transform.TransformPoint(globalPoint); // global coordinates of point
            globalPoint = manualAlign.transform.TransformPoint(globalPoint); 
            
            VoxelManager.AddVoxel(globalPoint, false);
            countTillYield++;
            if (countTillYield % 500 == 0)
            {
                float progress = (float) i / pointcloud.width * 100;
                TextMeshPro.text = $"Loading... {progress:F1}%";  // Display percentage with one decimal place
                yield return null;
            }
        }
        TextMeshPro.text = "DONE!!";
    }


/*    public void Debugging()
    {
        GameObject newImage = new GameObject("newImage");
        newImage.transform.position = ImageTarget.transform.position;
        newImage.transform.up = ImageTarget.transform.forward;
        newImage.transform.right = ImageTarget.transform.right;
        greenCube.transform.position = newImage.transform.position + newImage.transform.up * 0.5f;
        redCube.transform.position = newImage.transform.position + newImage.transform.right * 0.5f;
        blueCube.transform.position = newImage.transform.position + newImage.transform.forward * 0.5f;
    }*/

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

/*    public void x_MoveCloud(bool state)
    {
        Vector3 cloudIncrement = 0.1f * originRight;
        float xIncrement = 0.1f;
        if (!state)
        {
            cloudIncrement = -cloudIncrement;
            xIncrement = -xIncrement;
        }
        xTotal += xIncrement;
        PrefabsManager.chunkParentPrefab.transform.position += cloudIncrement;
        Vector3 display = new Vector3(xTotal,yTotal,zTotal);
        TextMeshPro.text = display.ToString();
    }

    public void y_MoveCloud(bool state)
    {
        Vector3 cloudIncrement = 0.1f * originUp;
        float yIncrement = 0.1f;
        if (!state)
        {
            cloudIncrement = -cloudIncrement;
            yIncrement = -yIncrement;
        }
        yTotal += yIncrement;
        PrefabsManager.chunkParentPrefab.transform.position += cloudIncrement;
        Vector3 display = new Vector3(xTotal, yTotal, zTotal);
        TextMeshPro.text = display.ToString();
    }

    public void z_MoveCloud(bool state)
    {
        Vector3 cloudIncrement = 0.1f * originForward;
        float zIncrement = 0.1f;
        if (!state)
        {
            cloudIncrement = -cloudIncrement;
            zIncrement = -zIncrement;
        }
        zTotal += zIncrement;
        PrefabsManager.chunkParentPrefab.transform.position += cloudIncrement;
        Vector3 display = new Vector3(xTotal, yTotal, zTotal);
        TextMeshPro.text = display.ToString();
    }*/

/*    public void x_RotateCloud(bool state)
    {
        float degree = 0.1f;
        if (!state) degree = -degree;
        xRotationTotal += degree;
        PrefabsManager.chunkParentPrefab.transform.rotation = Quaternion.identity;
        PrefabsManager.chunkParentPrefab.transform.Rotate(originForward, zRotationTotal);
        PrefabsManager.chunkParentPrefab.transform.Rotate(originRight, xRotationTotal);
        PrefabsManager.chunkParentPrefab.transform.Rotate(originUp, yRotationTotal);
        Vector3 display = new Vector3(xRotationTotal, yRotationTotal, zRotationTotal);
        TextMeshPro.text = display.ToString();
    }

    public void y_RotateCloud(bool state)
    {
        float degree = 0.1f;
        if (!state) degree = -degree;
        yRotationTotal += degree;
        PrefabsManager.chunkParentPrefab.transform.rotation = Quaternion.identity;
        PrefabsManager.chunkParentPrefab.transform.Rotate(originForward, zRotationTotal);
        PrefabsManager.chunkParentPrefab.transform.Rotate(originRight, xRotationTotal);
        PrefabsManager.chunkParentPrefab.transform.Rotate(originUp, yRotationTotal);
        Vector3 display = new Vector3(xRotationTotal, yRotationTotal, zRotationTotal);
        TextMeshPro.text = display.ToString();
    }

    public void z_RotateCloud(bool state)
    {
        float degree = 0.1f;
        if (!state) degree = -degree;
        zRotationTotal += degree;
        PrefabsManager.chunkParentPrefab.transform.rotation = Quaternion.identity;
        PrefabsManager.chunkParentPrefab.transform.Rotate(originForward, zRotationTotal);
        PrefabsManager.chunkParentPrefab.transform.Rotate(originRight, xRotationTotal);
        PrefabsManager.chunkParentPrefab.transform.Rotate(originUp, yRotationTotal);
        Vector3 display = new Vector3(xRotationTotal, yRotationTotal, zRotationTotal);
        TextMeshPro.text = display.ToString();
    }*/


}
