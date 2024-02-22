using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetRotation : MonoBehaviour
{
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * Time.deltaTime * 100);
    }

    /*public void Roto()
    {
        transform.rotation = Quaternion.identity;
        transform.position = Vector3.zero;
        transform.Rotate(Vector3.one * 25f);
    }*/
}
