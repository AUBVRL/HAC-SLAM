using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testingOverlap : MonoBehaviour
{

    public GameObject tester;
    RaycastHit hit;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*if(Physics.Raycast(transform.position, Vector3.forward, out hit, 10f))
        {
            Instantiate(tester, hit.point, Quaternion.identity);
        }*/
    }
}
