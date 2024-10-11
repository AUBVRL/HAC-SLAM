using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ChangePositionOnClick : MonoBehaviour
{
    public Vector3 PositionOn;
    public Vector3 PositionOff;
    public GameObject IconAndText;

    public void OnClick(bool isClicked)
    {
        if (isClicked)
        {
            transform.localPosition = PositionOn;
            IconAndText.transform.localPosition  = Vector3.zero;
        }
        else
        {
            transform.localPosition  = PositionOff;
            IconAndText.transform.localPosition = new Vector3(0, 0.006f, 0);
        }    
    }
}
