using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using UnityEngine;
using Vuforia;

public class VufoModelTargetToggle : MonoBehaviour
{
    public void ToggleObjectDetection()
    {
        if (GetComponent<VuforiaBehaviour>().enabled == true)
        {
            GetComponent<VuforiaBehaviour>().enabled = false;
        }
        else
        {
            GetComponent<VuforiaBehaviour>().enabled = true;
        }
    }
}
