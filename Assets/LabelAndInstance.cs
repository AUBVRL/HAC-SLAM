using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabelAndInstance : MonoBehaviour
{
    [NonSerialized]
    public byte label, instance;
    RosPublisherExample pub;
    GameObject publisher;

    private void Start()
    {
        publisher = GameObject.Find("Publisher");
        pub = publisher.GetComponent<RosPublisherExample>();
    }
    public void DeleteLabel()
    {
        pub.PublishDeleteLabel(label);
    }
    public void DeleteInstance()
    {
        pub.PublishDeleteInstance(label, instance);
    }

}
