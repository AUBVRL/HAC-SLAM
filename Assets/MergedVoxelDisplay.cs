using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using pc2 = RosMessageTypes.Sensor.PointCloud2Msg;
using System;

public class MergedVoxelDisplay : MonoBehaviour
{
    public GameObject cubz;
    public RosSubscriberExample Sub;
    public GameObject Parent;
    [NonSerialized]
    public float x, y, z, rx, ry, rz;
    bool once = true;
    Vector3 transformed;
    GameObject kuby;

    public Material SelectedMaterial;
    MeshRenderer VoxelMeshRenderer;
    public MinecraftBuilder mcb;
    Coroutine FillIncomingCoroutine;


    // Start is called before the first frame update
    void Start()
    {
        x = 0;
        y = 0;
        z = 0;
        rx = 0;
        ry = 0;
        rz = 0;
    }

 
    IEnumerator FillIncoming(pc2 pointcloud)
    {

        Vector3 cubePose;
        int j;
        int countTillYield = 0;

        for (int i = 0; i < pointcloud.width; i++)
        {
            j = i * Mathf.RoundToInt(pointcloud.point_step);
            cubePose.x = System.BitConverter.ToSingle(pointcloud.data, j);
            cubePose.z = System.BitConverter.ToSingle(pointcloud.data, j + 4);
            cubePose.y = System.BitConverter.ToSingle(pointcloud.data, j + 8);
            transformed = mcb.TransformPCL(Camera.main.transform.localPosition);
            if (Vector3.Distance(cubePose,transformed) < 30)
            {
                kuby = Instantiate(cubz, cubePose, Quaternion.identity);
                kuby.transform.SetParent(Parent.transform, false);
                kuby.gameObject.name = "MergedVoxel";
                if (pointcloud.data[j + 17] != 0)
                {
                    VoxelMeshRenderer = kuby.gameObject.GetComponent<MeshRenderer>();
                    VoxelMeshRenderer.material = SelectedMaterial;
                    Debug.Log("Oui");
                }

            }

            

            countTillYield++;
            

            if (countTillYield % 500 == 0) // Spawn 10 voxels each frame
            {
                yield return null;
            }
            
        }
        Debug.Log("Done");
        //this.transform.rotation = Quaternion.Euler(rx, ry, rz);
        //this.transform.position = new Vector3(x, y, z);



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
