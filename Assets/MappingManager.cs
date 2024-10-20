using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MappingManager : MonoBehaviour
{
    public Vector2Int rayCastArray = new Vector2Int(20, 20);

    void FixedUpdate()
    {
        List<Vector3> VoxelsPosition = MeshToPointCloudParallel();
        foreach( Vector3 v in VoxelsPosition)
        {
            VoxelManager.AddVoxel(v);
        }
    }

    public List<Vector3> MeshToPointCloudBeam()
    {
        List<Vector3> meshPoints = new List<Vector3>();
        Vector3 Gaze_direction = Camera.main.transform.forward;
        Vector3 Gaze_position = Camera.main.transform.position;
        
        int layerMask = 1 << 31;
        for (int i = 0; i < (MinecraftBuilder.Hor_angle_window / MinecraftBuilder.angle_size); i++)
        {
            Vector3 Hor_Ray_direction = Quaternion.Euler(0, (MinecraftBuilder.Hor_angle_min + (MinecraftBuilder.angle_size * i)), 0) * Gaze_direction;
            for (int j = 0; j < (int)(MinecraftBuilder.Ver_angle_window / MinecraftBuilder.angle_size); j++)
            {
                Vector3 Ver_Ray_direction = Quaternion.Euler((MinecraftBuilder.Ver_angle_min + (MinecraftBuilder.angle_size * j)), 0, 0) * Hor_Ray_direction;
                
                bool raycastHit = Physics.Raycast(Gaze_position, Ver_Ray_direction, out RaycastHit hit, 10f, layerMask);
                
                if (raycastHit)
                    {
                        meshPoints.Add(hit.point);
                    }
            }
        }
        return meshPoints;
    }
    public List<Vector3> MeshToPointCloudParallel()
    {
        List<Vector3> meshPoints = new List<Vector3>();
        Vector3 Gaze_direction = Camera.main.transform.forward;
        Vector3 Gaze_position = Camera.main.transform.position;

        int layerMask = 1 << 31;
        int layerMask2 = 1 << 7;
        for (int i = -rayCastArray.x /2; i < rayCastArray.x/2 ; i++)
        {
            Vector3 xGazeposition = Gaze_position + new Vector3(i*PrefabsManager.voxelSize , 0, 0);
            for (int j = -rayCastArray.y / 2; j < rayCastArray.y / 2; j++)
            {
                Vector3 newGazeposition = xGazeposition + new Vector3(0, j * PrefabsManager.voxelSize, 0);
                bool raycastHit = Physics.Raycast(newGazeposition, Gaze_direction, out RaycastHit hit, 10f, layerMask | layerMask2);
                if (raycastHit && hit.transform.gameObject.layer == 31)
                {
                    meshPoints.Add(hit.point);
                }

            }
        }
        return meshPoints;
    }
    public List<Vector3> StateCheckerParallel()
    {
        List<Vector3> meshPoints = new List<Vector3>();
        Vector3 Gaze_direction = Camera.main.transform.forward;
        Vector3 Gaze_position = Camera.main.transform.position;

        int layerMask = 1 << 7;
        for (int i = -rayCastArray.x / 2; i < rayCastArray.x / 2; i++)
        {
            Vector3 xGazeposition = Gaze_position + new Vector3(i * PrefabsManager.voxelSize, 0, 0);
            for (int j = -rayCastArray.y / 2; j < rayCastArray.y / 2; j++)
            {
                Vector3 newGazeposition = xGazeposition + new Vector3(0, j * PrefabsManager.voxelSize, 0);
                bool raycastHit = Physics.Raycast(newGazeposition, Gaze_direction, out RaycastHit hit, 10f, layerMask);
                if (raycastHit)
                {
                    meshPoints.Add(hit.transform.position);
                }
                
            }
        }
        return meshPoints;
    }

    
    void VoxelCleanse() 
    {
        List<Vector3> VoxelsPosition = StateCheckerParallel();
        foreach(Vector3 v in VoxelsPosition)
        {   
            VoxelManager.RemoveVoxel(v);
        }
    }
    void OnDisable()
    {
        CancelInvoke("VoxelCleanse");
    }
    void OnEnable()
    {
        InvokeRepeating("VoxelCleanse", 0, 1);
    }

}
