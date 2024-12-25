using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using Microsoft.MixedReality.Toolkit.UI;
using static System.ComponentModel.Design.ObjectSelectorEditor;

public class EditsManager : MonoBehaviour
{
    bool doneInstantiation, selectorInstantiated, fingersClosed;
    bool deletingVoxels;
    Vector3 initialPoseInCubes;
    public static GameObject instantiatedObject;
    Microsoft.MixedReality.Toolkit.Utilities.MixedRealityPose poseRightIndex;
    Microsoft.MixedReality.Toolkit.Utilities.MixedRealityPose poseRightThumb;
    IMixedRealityHandJointService handJointService;
    void Start()
    {
        doneInstantiation = false;
        selectorInstantiated = false;
        initialPoseInCubes = new();
        handJointService = CoreServices.GetInputSystemDataProvider<IMixedRealityHandJointService>();
    }

    
    void Update()
    {
        HandleHandInput();
    }

    void HandleHandInput()
    {
        if (!doneInstantiation)
        {
            UpdateHandTracking();
            if (!selectorInstantiated) InstantiateSelector();
            else StretchSelector();
        }
    }

    void UpdateHandTracking()
    {
        if (HandJointUtils.TryGetJointPose(Microsoft.MixedReality.Toolkit.Utilities.TrackedHandJoint.IndexTip, Microsoft.MixedReality.Toolkit.Utilities.Handedness.Right, out poseRightIndex))
        {
            HandJointUtils.TryGetJointPose(Microsoft.MixedReality.Toolkit.Utilities.TrackedHandJoint.ThumbTip, Microsoft.MixedReality.Toolkit.Utilities.Handedness.Right, out poseRightThumb);
            fingersClosed = Vector3.Distance(poseRightIndex.Position, poseRightThumb.Position) < 0.04f;
            Debug.Log(fingersClosed);
        }
    }

    void InstantiateSelector()
    {
        if (fingersClosed)
        {
            Vector3 initialPose, center;
            center = new();
            initialPose = poseRightIndex.Position;
            initialPoseInCubes.Set(Mathf.RoundToInt(initialPose.x / PrefabsManager.voxelSize), Mathf.RoundToInt(initialPose.y / PrefabsManager.voxelSize), Mathf.RoundToInt(initialPose.z / PrefabsManager.voxelSize));
            center.Set(initialPoseInCubes.x * PrefabsManager.voxelSize, initialPoseInCubes.y * PrefabsManager.voxelSize, initialPoseInCubes.z * PrefabsManager.voxelSize);
            instantiatedObject = Instantiate(PrefabsManager.Selector, center, Quaternion.identity);
            instantiatedObject.transform.localScale = new Vector3(PrefabsManager.voxelSize, PrefabsManager.voxelSize, PrefabsManager.voxelSize);
            selectorInstantiated = true;
        }
    }

    void StretchSelector()
    {
        if (fingersClosed)
        {
            Vector3 finalPose, finalPoseInCubes, center, scaleInCubes;
            finalPose = new();
            finalPoseInCubes = new();
            center = new();
            scaleInCubes = new();
            finalPose = poseRightIndex.Position;
            finalPoseInCubes.Set(Mathf.RoundToInt(finalPose.x / PrefabsManager.voxelSize), Mathf.RoundToInt(finalPose.y / PrefabsManager.voxelSize), Mathf.RoundToInt(finalPose.z / PrefabsManager.voxelSize));
            center = initialPoseInCubes + finalPoseInCubes;
            scaleInCubes.x = Mathf.Max(Mathf.Abs((initialPoseInCubes.x - finalPoseInCubes.x) * PrefabsManager.voxelSize), PrefabsManager.voxelSize);
            scaleInCubes.y = Mathf.Max(Mathf.Abs((initialPoseInCubes.y - finalPoseInCubes.y) * PrefabsManager.voxelSize), PrefabsManager.voxelSize);
            scaleInCubes.z = Mathf.Max(Mathf.Abs((initialPoseInCubes.z - finalPoseInCubes.z) * PrefabsManager.voxelSize), PrefabsManager.voxelSize);
            instantiatedObject.transform.position = center * PrefabsManager.voxelSize / 2;
            instantiatedObject.transform.localScale = scaleInCubes;
        }
        else
        {
            doneInstantiation = true;
        }
    }


}
