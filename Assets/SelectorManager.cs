using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectorManager : MonoBehaviour
{
    // Start is called before the first frame update
    public enum ShapeType {Cube, Sphere, Cylinder};
    
    public GameObject CubeSelector;
    public GameObject SphereSelector;
    public GameObject CylinderSelector;
    public static GameObject SelectorPrefab;

    void Start()
    {
        SelectorPrefab = CubeSelector;
    }

    public void SelectObject(int shape)
    {
        ShapeType shapeType = (ShapeType)shape;
        switch (shapeType)
        {
            case ShapeType.Cube:
                SelectorPrefab = CubeSelector;
                break;
            case ShapeType.Sphere:
                SelectorPrefab = SphereSelector;
                break;
            case ShapeType.Cylinder:
                SelectorPrefab = CylinderSelector;
                break;
        }
    }

    public void OnConvexityToggle(bool convex)
    {
        SelectorPrefab.GetComponent<MeshCollider>().convex = convex;
    }


}
