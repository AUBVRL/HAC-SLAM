using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;
using Unity.Robotics.ROSTCPConnector;
using Microsoft.MixedReality.Toolkit.Experimental.SceneUnderstanding;

public class TextWriter : MonoBehaviour
{
    public TMP_Text texet;
    //public GameObject image;
   // public GameObject pln;
    //public ROSConnection ros;
    //public RosSubscriberExample sub;
    //public GameObject Robot;
    public DemoSceneUnderstandingController demo;

    //[NonSerialized]
    //public string meshName;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //RaycastHit = 
        //texet.text = "Cam:" + Camera.main.transform.position + "\n Tar:" + image.transform.position + "\n MESH:" + meshName; // "\n Pln:" + pln.transform.position;

        //texet.text = "Cam:" + Camera.main.transform.position + "\n Tar:" + image.transform.position + "\n ptcld:" + sub.pcarr.Length; // "\n Pln:" + pln.transform.position;
        //texet.text = "Cam:" + Camera.main.transform.position + "\n Pos:" + Robot.transform.position + "\n Rot:" + Robot.transform.rotation;

        texet.text = demo.testt;

    }
}
