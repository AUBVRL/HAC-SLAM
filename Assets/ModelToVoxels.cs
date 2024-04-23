using Microsoft.MixedReality.Toolkit.Examples.Demos;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR.OpenXR.Input;
using static System.ComponentModel.Design.ObjectSelectorEditor;

public class ModelToVoxels : MonoBehaviour
{

    [SerializeField]
    GameObject Selector, cube;

    //public MinecraftBuilder mcb;

    Renderer selectorMesh;
    Vector3Int minbound_inCubes, maxbound_inCubes, current_chunkIndex;
    public Vector3Int chunkSize;
    List<Chunk3D> chunks = new List<Chunk3D>();

    Vector3 coliderPose, cubesizeScale;
    [SerializeField]
    float cubesize; 
    [SerializeField]
    TextMeshPro txt;
    int flatIndex;
    // Start is called before the first frame update
    void Start()
    {
        //cubesize = 0.1f;
        cubesizeScale.Set(cubesize, cubesize, cubesize);
        //StartCoroutine(Voxelize());
        selectorMesh = Selector.GetComponent<Renderer>();
        Instantiate(cube, selectorMesh.bounds.max ,Quaternion.identity);
        Instantiate(cube, selectorMesh.bounds.min, Quaternion.identity);
        //var go = new GameObject("jayde");
        

    }

    void Update()
    {
        //Debug.Log(chunks.Count);
    }

    IEnumerator Voxelize()
    {
        LayerMask mask = LayerMask.GetMask("Prism");
        int counterr = 1;

        // Transforms the selector shape into voxels and saves the center of each voxel as a pointcloud ROS message.
        

        //Rounding of the bounds to units of cubes:
        minbound_inCubes.Set(Mathf.RoundToInt(selectorMesh.bounds.min.x / cubesize),
                             Mathf.RoundToInt(selectorMesh.bounds.min.y / cubesize),
                             Mathf.RoundToInt(selectorMesh.bounds.min.z / cubesize));

        maxbound_inCubes.Set(Mathf.RoundToInt(selectorMesh.bounds.max.x / cubesize),
                             Mathf.RoundToInt(selectorMesh.bounds.max.y / cubesize),
                             Mathf.RoundToInt(selectorMesh.bounds.max.z / cubesize));
        //maxbound_inCubes = maxbound_inCubes / 2;


        //Loop from min to max bound:

        for (int i = minbound_inCubes.x; i <= maxbound_inCubes.x; i++)
        {
            for (int j = minbound_inCubes.y; j <= maxbound_inCubes.y; j++)
            {
                for (int k = minbound_inCubes.z; k <= maxbound_inCubes.z; k++)
                {
                    coliderPose.Set(i, j, k);
                    coliderPose = coliderPose * cubesize;

                    if (Physics.CheckBox(coliderPose, cubesizeScale / 2, Quaternion.identity, mask))
                    {
                        AddPosition(coliderPose);
                        //Instantiate(cube, coliderPose, Quaternion.identity, parent.transform);
                        counterr++;

                    }

                    if (counterr % 1000 == 0)
                    {
                        counterr++;
                        txt.text = counterr.ToString();
                        yield return null;

                    }
                    if (counterr >= 7000000)
                    {
                        break;
                    }
                }
                if (counterr >= 7000000)
                {
                    break;
                }
            }
            if (counterr >= 7000000)
            {
                break;
            }
        }
        Debug.Log("Done");
        initialChunkEnabler();
        InvokeRepeating(nameof(EnableChunks), 1, 0.1f);
        

    }

    public void InitiateVoxels()
    {
        StartCoroutine(Voxelize());
        Debug.Log("Go");
        
    }

    public void AddPosition(Vector3 pose2)
    {
        //Vector3 MinBound = selectorMesh.bounds.min;
        //Vector3 MaxBound = selectorMesh.bounds.max;
        Vector3 pose = pose2 - selectorMesh.bounds.min;

        Vector3Int chunkIndex = new Vector3Int(
            Mathf.CeilToInt(pose.x / chunkSize.x),
            Mathf.CeilToInt(pose.y / chunkSize.y),
            Mathf.CeilToInt(pose.z / chunkSize.z)
        );

        // Convert chunk index to flat index
        int flatIndex = GetFlatIndex(chunkIndex);

        // If the chunk doesn't exist, create a new one
        if (flatIndex < 0) Debug.Log(chunkIndex);

        while (flatIndex >= chunks.Count)
        {
            chunks.Add(null);
        }

        if (chunks[flatIndex] == null)
        {
            chunks[flatIndex] = new Chunk3D(chunkIndex);
            
        }

        // Add the position to the chunk
        chunks[flatIndex].positions.Add(pose2);
        Instantiate(cube, pose2, Quaternion.identity, chunks[flatIndex].Parent.transform);
        //mcb.VoxelInstantiator(pose);
    }

    public void initialChunkEnabler()
    {
        Vector3 pose = Camera.main.transform.position - selectorMesh.bounds.min;

        Vector3Int chunkIndex = new Vector3Int(
            Mathf.CeilToInt(pose.x / chunkSize.x),
            Mathf.CeilToInt(pose.y / chunkSize.y),
            Mathf.CeilToInt(pose.z / chunkSize.z)
            );

        //
        Vector3Int Convolute = new Vector3Int();
        for (int i = -1; i <= 1; i++)
        {
            for(int j = -1; j <= 1; j++)
            {
                for (int k = -1; k <= 1; k++)
                {
                    Convolute.Set(i, j, k);
                    flatIndex = GetFlatIndex(chunkIndex + Convolute);

                    if (flatIndex <= chunks.Count && chunks[flatIndex] != null)
                    {
                        chunks[flatIndex].Parent.SetActive(true);
                    }
                }
            }
        }
        current_chunkIndex = chunkIndex;
    }
    
    public void EnableChunks()
    {
        
        Vector3 pose = Camera.main.transform.position - selectorMesh.bounds.min;
        
        Vector3Int chunkIndex = new Vector3Int(
            Mathf.CeilToInt(pose.x / chunkSize.x),
            Mathf.CeilToInt(pose.y / chunkSize.y),
            Mathf.CeilToInt(pose.z / chunkSize.z)
        );

        if (current_chunkIndex !=  chunkIndex)
        {
            Debug.Log("Changed");
            

            
            
            Vector3Int Diff = chunkIndex - current_chunkIndex;
            Vector3Int Convolute = new Vector3Int();

            // For Enabling new chunks
            for (int i = -1 + Mathf.Abs(Diff.x); i <= 1 - Mathf.Abs(Diff.x); i++)
            {
                for (int j = -1 + Mathf.Abs(Diff.y); j <= 1 - Mathf.Abs(Diff.y); j++)
                {
                    for (int k = -1 + Mathf.Abs(Diff.z); k <= 1 - Mathf.Abs(Diff.z); k++)
                    {
                        Convolute.Set(i, j, k);
                        
                        flatIndex = GetFlatIndex(chunkIndex + Convolute + Diff);

                        if (flatIndex <= chunks.Count && chunks[flatIndex] != null)
                        {
                            chunks[flatIndex].Parent.SetActive(true);
                        }
                    }
                }
            }
            //For disabling old chunks
            for (int i = -1 + Mathf.Abs(Diff.x); i <= 1 - Mathf.Abs(Diff.x); i++)
            {
                for (int j = -1 + Mathf.Abs(Diff.y); j <= 1 - Mathf.Abs(Diff.y); j++)
                {
                    for (int k = -1 + Mathf.Abs(Diff.z); k <= 1 - Mathf.Abs(Diff.z); k++)
                    {
                        Convolute.Set(i, j, k);

                        flatIndex = GetFlatIndex(current_chunkIndex + Convolute - Diff);

                        if (flatIndex <= chunks.Count && chunks[flatIndex] != null)
                        {
                            chunks[flatIndex].Parent.SetActive(false);
                        }
                    }
                }
            }

            current_chunkIndex = chunkIndex;
        }

        //flatIndex = GetFlatIndex(chunkIndex);

        /*if (flatIndex <= chunks.Count && chunks[flatIndex] != null)
        {
            chunks[flatIndex].Parent.SetActive(true);
        }*/

    }

    public List<Vector3> GetPositionsFromChunk(Vector3Int chunkIndex)
    {
        int flatIndex = GetFlatIndex(chunkIndex);

        if (flatIndex < 0 || flatIndex >= chunks.Count || chunks[flatIndex] == null)
        {
            Debug.LogWarning("Chunk does not exist.");
            return new List<Vector3>();
        }

        return chunks[flatIndex].positions;
    }

    private int GetFlatIndex(Vector3Int index3D)
    {
        int flatIndex = index3D.z * ((int)selectorMesh.bounds.max.x * (int)selectorMesh.bounds.max.y) + index3D.y * (int)selectorMesh.bounds.max.x + index3D.x;
        return flatIndex;
    }

    public void Chunk3DChangeParent(Vector3Int chunkIndex)
    {
        
        flatIndex = GetFlatIndex(chunkIndex);
        Destroy(chunks[flatIndex].Parent);
        chunks[flatIndex].Parent = new GameObject(chunkIndex.ToString());
    }

}


[System.Serializable]
public class Chunk3D
{
    public Vector3Int chunkID;
    public List<Vector3> positions = new List<Vector3>();
    public GameObject Parent;

    public Chunk3D(Vector3Int id)
    {
        chunkID = id;
        Parent = new GameObject(id.ToString());
        //Parent.layer = 9;
        //Parent.AddComponent<BoxCollider>().size = new Vector3(3,3,3);
        Parent.SetActive(false);
    }
}
