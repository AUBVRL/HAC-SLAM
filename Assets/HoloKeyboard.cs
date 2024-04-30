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
    public MixedRealityKeyboardPreview mixedRealityKeyboardPreview;


    void Start()
    {
        MRkeyboard = gameObject.AddComponent<MixedRealityKeyboard>();
        
        if (mixedRealityKeyboardPreview != null)
        {
            mixedRealityKeyboardPreview.gameObject.SetActive(false);
        }
        if (MRkeyboard.OnShowKeyboard != null)
        {
            MRkeyboard.OnShowKeyboard.AddListener(() =>
            {
                if (mixedRealityKeyboardPreview != null)
                {
                    mixedRealityKeyboardPreview.gameObject.SetActive(true);
                }
            });
        }

        if (MRkeyboard.OnHideKeyboard != null)
        {
            MRkeyboard.OnHideKeyboard.AddListener(() =>
            {
                if (mixedRealityKeyboardPreview != null)
                {
                    mixedRealityKeyboardPreview.gameObject.SetActive(false);
                }
            });
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (MRkeyboard.Visible)
        {
            texty = MRkeyboard.Text;
            if (mixedRealityKeyboardPreview != null)
            {
                mixedRealityKeyboardPreview.Text = MRkeyboard.Text;
                mixedRealityKeyboardPreview.CaretIndex = MRkeyboard.CaretIndex;
            }
        }
        else
        {
            MRkeyboard.ClearKeyboardText();

            if (mixedRealityKeyboardPreview != null)
            {
                mixedRealityKeyboardPreview.Text = string.Empty;
                mixedRealityKeyboardPreview.CaretIndex = 0;
            }
        }
    }

    public void OpenKeyboard()
    {
        
        MRkeyboard.ShowKeyboard(MRkeyboard.Text, false);
        // MRkeyboard.OnCommitText.AddListener(OpenKeyboard); Mhemme
    }

    public void SaveName()
    {
        Pub.PublishSavedMapName(texty);
        //Pub.PublishSavedMapName("FixedTextHere");
    }

    public void CloseKeyboard() 
    {
        MRkeyboard.HideKeyboard();
    }
}
