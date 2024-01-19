using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;

public class TestingKeyboardInject : MonoBehaviour
{

    public GameObject tooltip;
    ToolTip tooltipText;
    public HoloKeyboard holok;
    // Start is called before the first frame update
    void Start()
    {
        tooltipText = tooltip.GetComponent<ToolTip>();
    }

    // Update is called once per frame
    void Update()
    {
        tooltipText.ToolTipText = holok.texty;
    }
}
