using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnOff : MonoBehaviour
{
    List<Vector3> vec;
    private void Start()
    {
        vec = new List<Vector3>();
        Vector3 testvec = new Vector3(1f, 2f, 3f);
        Vector3 testvec2 = new Vector3(2f, 2f, 3f);
        vec.Add(testvec);
        //Debug.Log("Mawjud? " + vec.Contains(testvec2));

        byte[] byteArray = new byte[] { 0, 0, 3, 255 };
        float floatValue = System.BitConverter.ToSingle(byteArray, 0);
        //Debug.Log(floatValue);
        byte ex = 255;
        byte[] fff = System.BitConverter.GetBytes(ex);
        Debug.Log(fff[1]);
    }

    private void Update()
    {
        
    }

}
