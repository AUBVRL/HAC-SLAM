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
    public TextMeshPro msg;

    [NonSerialized]
    public string texty;
    
    public void OpenKeyboard()
    {
        MRkeyboard.ShowKeyboard(MRkeyboard.Text, false);
        // MRkeyboard.OnCommitText.AddListener(OpenKeyboard); Mhemme
    }
    
    void Start()
    {
        MRkeyboard = gameObject.AddComponent<MixedRealityKeyboard>();
        //texty = "heyooooooo";
    }

    // Update is called once per frame
    void Update()
    {
        //texty = "hey";
        //Debug.Log(texty);
        if (MRkeyboard.Visible)
        {
            if (msg != null)
            {
                msg.text = "Typing: " + MRkeyboard.Text;
            }
            texty = MRkeyboard.Text;
        }
        else
        {
            MRkeyboard.ClearKeyboardText();
            var keyboardText = MRkeyboard.Text;

            if (string.IsNullOrEmpty(keyboardText))
            {
                if (msg != null)
                {
                    msg.text = "Open keyboard to type text.";
                }
            }
            else
            {
                if (msg != null)
                {
                    msg.text = "Typed: " + keyboardText;
                }
            }
        }
    }


    //Add function for MRkeyboard.ClearKeyboardText();
}
