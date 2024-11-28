using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class AdjustSelectorManager : MonoBehaviour
{
    public TMP_InputField xPosition;
    public TMP_InputField yPosition;
    public TMP_InputField zPosition;
    public TMP_InputField xAngle;
    public TMP_InputField yAngle;
    public TMP_InputField zAngle;
    public TMP_InputField xScale;
    public TMP_InputField yScale;
    public TMP_InputField zScale;
    Vector3 adjustedPose;
    Vector3 adjustedRotation;
    Vector3 adjustedScale;

    void Start()
    {
        xPosition.onEndEdit.AddListener(HandleInputEndEdit_xPosition);
        yPosition.onEndEdit.AddListener(HandleInputEndEdit_yPosition);
        zPosition.onEndEdit.AddListener(HandleInputEndEdit_zPosition);
        xAngle.onEndEdit.AddListener(HandleInputEndEdit_xAngle);
        yAngle.onEndEdit.AddListener(HandleInputEndEdit_yAngle);
        zAngle.onEndEdit.AddListener(HandleInputEndEdit_zAngle);
        xScale.onEndEdit.AddListener(HandleInputEndEdit_xScale);
        yScale.onEndEdit.AddListener(HandleInputEndEdit_yScale);
        zScale.onEndEdit.AddListener(HandleInputEndEdit_zScale);
    }

    void OnEnable()
    {
        // position
        xPosition.text = FingerPose.Selector.transform.position.x.ToString();
        yPosition.text = FingerPose.Selector.transform.position.y.ToString();
        zPosition.text = FingerPose.Selector.transform.position.z.ToString();
        adjustedPose = FingerPose.Selector.transform.position;
        // rotation
        xAngle.text = FingerPose.Selector.transform.eulerAngles.x.ToString();
        yAngle.text = FingerPose.Selector.transform.eulerAngles.y.ToString();
        zAngle.text = FingerPose.Selector.transform.eulerAngles.z.ToString();
        adjustedRotation = FingerPose.Selector.transform.eulerAngles;
        // scale
        xScale.text = FingerPose.Selector.transform.localScale.x.ToString();
        yScale.text = FingerPose.Selector.transform.localScale.y.ToString();
        zScale.text = FingerPose.Selector.transform.localScale.z.ToString();
        adjustedScale = FingerPose.Selector.transform.localScale;
    }

    void HandleInputEndEdit_xPosition(string inputText)
    {
        adjustedPose.x = float.Parse(inputText);
        FingerPose.Selector.transform.position = adjustedPose;
    }

    void HandleInputEndEdit_yPosition(string inputText)
    {
        adjustedPose.y = float.Parse(inputText);
        FingerPose.Selector.transform.position = adjustedPose;
    }

    void HandleInputEndEdit_zPosition(string inputText)
    {
        adjustedPose.z = float.Parse(inputText);
        FingerPose.Selector.transform.position = adjustedPose;
    }

    void HandleInputEndEdit_xAngle(string inputText)
    {
        adjustedRotation.x = float.Parse(inputText);
        FingerPose.Selector.transform.eulerAngles = adjustedRotation;
    }

    void HandleInputEndEdit_yAngle(string inputText)
    {
        adjustedRotation.y = float.Parse(inputText);
        FingerPose.Selector.transform.eulerAngles = adjustedRotation;
    }

    void HandleInputEndEdit_zAngle(string inputText)
    {
        adjustedRotation.z = float.Parse(inputText);
        FingerPose.Selector.transform.eulerAngles = adjustedRotation;
    }

    void HandleInputEndEdit_xScale(string inputText)
    {
        adjustedScale.x = float.Parse(inputText);
        FingerPose.Selector.transform.localScale = adjustedScale;
    }

    void HandleInputEndEdit_yScale(string inputText)
    {
        adjustedScale.y = float.Parse(inputText);
        FingerPose.Selector.transform.localScale = adjustedScale;
    }

    void HandleInputEndEdit_zScale(string inputText)
    {
        adjustedScale.z = float.Parse(inputText);
        FingerPose.Selector.transform.localScale = adjustedScale;
    }


}