using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Microsoft.MixedReality.Toolkit.Experimental.UI;


public class AdjustSelectorManager : MonoBehaviour
{
    public MRTKUGUIInputField xPosition;
    public MRTKUGUIInputField yPosition;
    public MRTKUGUIInputField zPosition;
    public MRTKUGUIInputField xAngle;
    public MRTKUGUIInputField yAngle;
    public MRTKUGUIInputField zAngle;
    public MRTKUGUIInputField xScale;
    public MRTKUGUIInputField yScale;
    public MRTKUGUIInputField zScale;
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
        Vector3 newPosition = Camera.main.transform.position + Camera.main.transform.forward * 1.0f;
        // position
        xPosition.text = EditsManager.instantiatedObject.transform.position.x.ToString();
        yPosition.text = EditsManager.instantiatedObject.transform.position.y.ToString();
        zPosition.text = EditsManager.instantiatedObject.transform.position.z.ToString();
        adjustedPose = EditsManager.instantiatedObject.transform.position;
        xPosition.transform.position = newPosition;
        yPosition.transform.position = newPosition - 0.1f * Camera.main.transform.up;
        zPosition.transform.position = newPosition - 0.2f * Camera.main.transform.up;

        // rotation
        xAngle.text = EditsManager.instantiatedObject.transform.eulerAngles.x.ToString();
        yAngle.text = EditsManager.instantiatedObject.transform.eulerAngles.y.ToString();
        zAngle.text = EditsManager.instantiatedObject.transform.eulerAngles.z.ToString();
        adjustedRotation = EditsManager.instantiatedObject.transform.eulerAngles;
        xAngle.transform.position = newPosition;
        yAngle.transform.position = newPosition - 0.1f * Camera.main.transform.up;
        zAngle.transform.position = newPosition - 0.2f * Camera.main.transform.up;

        // scale
        xScale.text = EditsManager.instantiatedObject.transform.localScale.x.ToString();
        yScale.text = EditsManager.instantiatedObject.transform.localScale.y.ToString();
        zScale.text = EditsManager.instantiatedObject.transform.localScale.z.ToString();
        adjustedScale = EditsManager.instantiatedObject.transform.localScale;
        xScale.transform.position = newPosition;
        yScale.transform.position = newPosition - 0.1f * Camera.main.transform.up;
        zScale.transform.position = newPosition - 0.2f * Camera.main.transform.up;
    }

    void HandleInputEndEdit_xPosition(string inputText)
    {
        adjustedPose.x = float.Parse(inputText);
        EditsManager.instantiatedObject.transform.position = adjustedPose;
    }

    void HandleInputEndEdit_yPosition(string inputText)
    {
        adjustedPose.y = float.Parse(inputText);
        EditsManager.instantiatedObject.transform.position = adjustedPose;
    }

    void HandleInputEndEdit_zPosition(string inputText)
    {
        adjustedPose.z = float.Parse(inputText);
        EditsManager.instantiatedObject.transform.position = adjustedPose;
    }

    void HandleInputEndEdit_xAngle(string inputText)
    {
        adjustedRotation.x = float.Parse(inputText);
        EditsManager.instantiatedObject.transform.eulerAngles = adjustedRotation;
    }

    void HandleInputEndEdit_yAngle(string inputText)
    {
        adjustedRotation.y = float.Parse(inputText);
        EditsManager.instantiatedObject.transform.eulerAngles = adjustedRotation;
    }

    void HandleInputEndEdit_zAngle(string inputText)
    {
        adjustedRotation.z = float.Parse(inputText);
        EditsManager.instantiatedObject.transform.eulerAngles = adjustedRotation;
    }

    void HandleInputEndEdit_xScale(string inputText)
    {
        adjustedScale.x = float.Parse(inputText);
        EditsManager.instantiatedObject.transform.localScale = adjustedScale;
    }

    void HandleInputEndEdit_yScale(string inputText)
    {
        adjustedScale.y = float.Parse(inputText);
        EditsManager.instantiatedObject.transform.localScale = adjustedScale;
    }

    void HandleInputEndEdit_zScale(string inputText)
    {
        adjustedScale.z = float.Parse(inputText);
        EditsManager.instantiatedObject.transform.localScale = adjustedScale;
    }


}