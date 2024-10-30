using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class Chunk
{
    public Vector3 Position;

    public List<Voxel> Voxels = new List<Voxel>();
    public Dictionary<Vector3, Voxel> VoxelsDict = new Dictionary<Vector3, Voxel>();

    public Chunk(Vector3 position)
    {
        Position = position; // Redundant since the chunk position is the key in the dictionary
        VoxelsDict.Add(position, new Voxel(position));
        //VoxelsDict.Add(position, new Voxel())
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