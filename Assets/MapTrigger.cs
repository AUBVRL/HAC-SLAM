using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTrigger : MonoBehaviour
{
    int mask;
    public string MaskName;
    // Start is called before the first frame update
    void Start()
    {
        mask = LayerMask.NameToLayer(MaskName);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == mask)
        {
            other.gameObject.GetComponent<MeshRenderer>().enabled = true;
        }
        
        //Debug.Log("hey");
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == mask)
        {
            other.gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
    }
    
}
