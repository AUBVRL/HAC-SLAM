using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ArchitecturalElementsManager : MonoBehaviour
{
    // key is Scene Understanding ID.
    Dictionary<int, ArchitecturalElement> ArchitecturalElements;
    

    private void Start()
    {
        ArchitecturalElements = new Dictionary<int, ArchitecturalElement>();
    }

    static void AddArchitecturalElement(int SU_ID, int Category_ID,
        Vector3 Position)
    {
        
    }

    static void UpdateArchitecturalElement()
    {

    }

    static void RemoveArchitecturalElement()
    {

    }
}
