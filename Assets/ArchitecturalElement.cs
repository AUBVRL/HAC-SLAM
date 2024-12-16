using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArchitecturalElement : MonoBehaviour
{

    int Category_ID; // 0 for wall, 1 for ceiling, 2 for floor
    Vector3 Position;
    Vector3 Rotation;
    Vector2 Scale;
    GameObject Prefab;

    ArchitecturalElement(int _Category_ID, Vector3 _Position,
        Vector3 _Rotation, Vector2 _Scale, GameObject _Prefab)
    {
        Category_ID = _Category_ID;
        Position = _Position;
        Rotation = _Rotation;
        Scale = _Scale;
        Prefab = _Prefab;
    }

    void ArchitecturalElementUpdate(Vector3 _Position, Vector3 _Rotation,
        Vector2 _Scale) 
    { 
        Position = _Position;
        Rotation = _Rotation;
        Scale = _Scale;
    }
}
