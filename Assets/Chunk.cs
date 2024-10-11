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
        Position = position;
        Voxels.Add(new Voxel(position));
        //VoxelsDict.Add(position, new Voxel())
    }

    public byte[] GetChunkByteData()
    {
        List<byte> byteList = new List<byte>();

        // Iterate over the voxels in the chunk
        foreach (var voxelEntry in Voxels)
        {
            byteList.AddRange(voxelEntry.ToByteArray());  // Use Voxel's ToByteArray method
        }

        return byteList.ToArray();  // Return byte array for the whole chunk
    }

    //There should be a function that resets the byte array

}