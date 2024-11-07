using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class Chunk
{
    public Vector3 Position;

    public List<Voxel> Voxels = new List<Voxel>();
    public Dictionary<Vector3, Voxel> VoxelsDict = new Dictionary<Vector3, Voxel>();

    public GameObject ChunkGameObject;

    public Chunk(Vector3 position)
    {
        Position = VoxelManager.RoundToChunk(position);
        ChunkGameObject = new("Chunk" + Position);
        ChunkGameObject.transform.parent = PrefabsManager.voxelPrefabParent.transform;
        ChunkGameObject.transform.position = Position;
        //ChunkGameObject.SetActive(false); // Uncomment this for the comparison app 
        VoxelsDict.Add(position, new Voxel(position, ChunkGameObject));
        //VoxelsDict.Add(position, new Voxel())
    }

    public void AddVoxel(Vector3 position, bool humanEdited = false)
    {
        VoxelsDict.Add(position, new Voxel(position, ChunkGameObject, humanEdited));
    }
    
    public List<byte> GetChunkByteData()
    {
        List<byte> byteList = new();
        
        // Iterate over the voxels in the chunk
        foreach (var voxelEntry in VoxelsDict.Values)
        {
            byteList.AddRange(voxelEntry.ToByteArray());  // Use Voxel's ToByteArray method
        }
        
        return byteList;  // Return byte array for the whole chunk
    }

    //There should be a function that resets the byte array

}