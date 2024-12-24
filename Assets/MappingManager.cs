using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MappingManager : MonoBehaviour
{
    public Vector2Int rayCastArray = new Vector2Int(20, 20);
    void FixedUpdate()
    {
        List<Vector3> VoxelsPosition = MeshToPointCloudParallel();
        foreach (Vector3 v in VoxelsPosition)
        {
            VoxelManager.AddVoxel(v);
        }
    }

    public List<Vector3> MeshToPointCloudParallel()
    {
        List<Vector3> meshPoints = new List<Vector3>();
        Vector3 Gaze_direction = Camera.main.transform.forward;
        Vector3 Gaze_position = Camera.main.transform.position;

        int layerMask = 1 << 31;
        for (int i = -rayCastArray.x / 2; i < rayCastArray.x / 2; i++)
        {
            Vector3 xGazeposition = Gaze_position + new Vector3(i * PrefabsManager.voxelSize, 0, 0);
            for (int j = -rayCastArray.y / 2; j < rayCastArray.y / 2; j++)
            {
                Vector3 newGazeposition = xGazeposition + new Vector3(0, j * PrefabsManager.voxelSize, 0);
                bool raycastHit = Physics.Raycast(newGazeposition, Gaze_direction, out RaycastHit hit, 10f, layerMask);
                if (raycastHit)
                {
                    meshPoints.Add(hit.point);
                }
            }
        }
        return meshPoints;
    }


}
