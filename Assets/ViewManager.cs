using System.Collections;
using System.Collections.Generic;
using RosMessageTypes.Sensor;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;
using Microsoft.MixedReality.Toolkit.UI;
using TMPro;

public class ViewManager : MonoBehaviour
{
    Vector3Int currentChunk;
    public TextMeshPro MenuText;
    public Vector3 FixedPointCloudPosition, FixedPointCloudRotation;
    Vector3 ImageTargetPosition, ImageTargetRotation;
    public GameObject ImageTarget, VoxelPrefab;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating(nameof(checkCameraPosition), 0, 0.1f);
    }

    // Update is called once per frame
    void Update()
    {
        ImageTargetPosition = ImageTarget.transform.position;
        //ImageTargetRotation = ImageTarget.transform.rotation.eulerAngles;
        Quaternion rot = Quaternion.Euler(FixedPointCloudRotation);
        Vector3 transformedPoint = rot * FixedPointCloudPosition;
        // Debug.Log(transformedPoint);
        VoxelPrefab.transform.position = transformedPoint;

    }
    
    
    void checkCameraPosition()
    {
        Vector3 cameraPosition = Camera.main.transform.position;
        Vector3Int cameraChunk = Vector3Int.RoundToInt(VoxelManager.RoundToChunk(cameraPosition));
        
        if (cameraChunk != currentChunk)
        {
            //Debug.Log(cameraChunk);
            ManageEnabledChunks(cameraChunk);
            currentChunk = cameraChunk;
        }
    }

    void initialVisualization()
    {
        Vector3 cameraPosition = Camera.main.transform.position;
        Vector3Int cameraChunk = Vector3Int.RoundToInt(VoxelManager.RoundToChunk(cameraPosition));
        for(int i =  - 3 ; i <= 3; i+=3)
        {
            for(int j = 0; j <= 6; j+=3)
            {
                for(int k = - 3; k <= 3; k+=3)
                {
                    Vector3 surroundingChunk = new(i, j, k);

                    Vector3 newChunkToEnable = cameraChunk + surroundingChunk;

                    if (VoxelManager.ChunksDict.ContainsKey(newChunkToEnable))
                    {
                        VoxelManager.ChunksDict[newChunkToEnable].ChunkGameObject.SetActive(true);
                    }

                }
            }
        }

    }
    
    public void ManageEnabledChunks(Vector3Int v)
    {
        Vector3Int diff = v - currentChunk;
        Vector3Int Absdiff = new(Mathf.Abs(diff.x), Mathf.Abs(diff.y), Mathf.Abs(diff.z));
        
        for(int i = Absdiff.x - 3 ; i <= 3 - Absdiff.x; i+=3)
        {
            for(int j = Absdiff.y ; j <= 6 - Absdiff.y; j+=3)
            {
                for(int k = Absdiff.z - 3; k <= 3 - Absdiff.z; k+=3)
                {
                    Vector3 surroundingChunk = new Vector3(i, j, k);

                    Vector3 newChunkToEnable = v + surroundingChunk + diff;

                    Vector3 oldChunkToDisable = currentChunk + surroundingChunk - diff;

                    if (VoxelManager.ChunksDict.ContainsKey(newChunkToEnable))
                    {
                        print("Activating chunk");
                        VoxelManager.ChunksDict[newChunkToEnable].ChunkGameObject.SetActive(true);
                    }

                    if (VoxelManager.ChunksDict.ContainsKey(oldChunkToDisable))
                    {
                        print("Deactivating chunk");
                        VoxelManager.ChunksDict[oldChunkToDisable].ChunkGameObject.SetActive(false);
                    }
                }
            }
        }
        
    }

    public void DisplayReceivedVoxels()
    {
        
        StartCoroutine(FillIncoming(RosSubscriberManager.incomingPointCloudLive));
    }

    IEnumerator FillIncoming(PointCloud2Msg pointcloud)
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
            
            //The transformation logic should be added here
            //cubePose = TransformPointFromTargetToFixed(cubePose);
            //cubePose = TransformPointFromCameraToTarget(cubePose);

            VoxelManager.AddVoxel(cubePose);
            
            countTillYield++;
        
            if (countTillYield % 500 == 0) 
            {
                int percentage = (int) (i / pointcloud.width * 100);
                MenuText.text = "Loading: " + percentage + "%";
                yield return null;
            }

        }
        MenuText.text = "Main Menu";

        Debug.Log("Done");

        //this.transform.rotation = Quaternion.Euler(rx, ry, rz);
        //this.transform.position = new Vector3(x, y, z);



    }

    public Vector3 TransformPointFromTargetToFixed(Vector3 point)
    {
        //The transformation logic should be added here
        Quaternion rotation = Quaternion.Euler(FixedPointCloudRotation);
        Vector3 position = FixedPointCloudPosition;
        Vector3 transformedPoint = rotation * point + position;
        return transformedPoint;
    }

    public Vector3 TransformPointFromCameraToTarget(Vector3 point)
    {
        //The transformation logic should be added here
        Vector3 position = ImageTargetPosition;
        float angle = Mathf.Atan2(ImageTarget.transform.up.x, ImageTarget.transform.up.z) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0f, angle, 0f);
        Vector3 transformedPoint = rotation * point + position;
        return transformedPoint;
    }

}
