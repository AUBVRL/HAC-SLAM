using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class ImageTargetFollower : MonoBehaviour
{
    public GameObject imageTarget;
    Vector3 initialPose, imagetargetRot,initialRot;
    // Start is called before the first frame update
    void Start()
    {
        initialPose = this.transform.position; 
        initialRot = this.transform.rotation.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        Shift();
    }

    public void Shift()
    {
        this.transform.position = initialPose + imageTarget.transform.position;
        imagetargetRot.Set(0, imageTarget.transform.rotation.eulerAngles.z + 180, 0);
        this.transform.rotation = Quaternion.Euler(imagetargetRot + initialRot);
    }
}
