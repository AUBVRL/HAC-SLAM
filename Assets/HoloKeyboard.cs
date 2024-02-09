using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Microsoft.MixedReality.Toolkit.Experimental.UI;
using TMPro;
using System;

public class HoloKeyboard : MonoBehaviour
{
    // Start is called before the first frame update
    MixedRealityKeyboard MRkeyboard;
    public RosPublisherExample Pub;
    [NonSerialized]
    public string texty;
    


    void Start()
    {
        MRkeyboard = gameObject.AddComponent<MixedRealityKeyboard>();
    }

    // Update is called once per frame
    void Update()
    {

        if (MRkeyboard.Visible)
        {
            texty = MRkeyboard.Text;
        }
        else
        {
            MRkeyboard.ClearKeyboardText();
        }
    }

    public void OpenKeyboard()
    {
        //Debug.Log("Yow");
        MRkeyboard.ShowKeyboard(MRkeyboard.Text, false);
        // MRkeyboard.OnCommitText.AddListener(OpenKeyboard); Mhemme
    }

    public void SaveName()
    {
        Pub.PublishSavedMapName(texty);
    }

    public void CloseKeyboard() 
    {
        MRkeyboard.HideKeyboard();
    }
}
