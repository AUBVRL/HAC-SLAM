using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using Microsoft.MixedReality.Toolkit.UI;
using static System.ComponentModel.Design.ObjectSelectorEditor;
using UnityEngine.Assertions;
using TMPro;

public class EditsManager : MonoBehaviour
{
    public TextMeshPro menuText;
    public GameObject VoxelsMenu, SelectorOptionsMenu;
    public GameObject AssetsMenu;
    bool doneInstantiation, selectorInstantiated, fingersClosed;
    bool additionSelected, deletionSelected, labelingSelected;
    bool assetAdditionSelected;
    Vector3 initialPoseInCubes;
    public static GameObject instantiatedObject;
    Microsoft.MixedReality.Toolkit.Utilities.MixedRealityPose poseRightIndex;
    Microsoft.MixedReality.Toolkit.Utilities.MixedRealityPose poseRightThumb;
    IMixedRealityHandJointService handJointService;
    InputActionHandler inputActionHandler;
    int SelectorLayerMask = 1 << 6;

    void Start()
    {
        doneInstantiation = false;
        selectorInstantiated = false; 
        initialPoseInCubes = new();
        handJointService = CoreServices.GetInputSystemDataProvider<IMixedRealityHandJointService>();
        inputActionHandler = gameObject.GetComponent<InputActionHandler>();
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
            center = initialPoseInCubes * PrefabsManager.voxelSize;
            instantiatedObject = Instantiate(PrefabsManager.Selector, center, Quaternion.identity);
            instantiatedObject.transform.localScale = Vector3.one * PrefabsManager.voxelSize;
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
            VoxelsMenu.SetActive(false);
            SelectorOptionsMenu.SetActive(true);
            menuText.text = "Selector Options Menu";
        }
    }

    List<Vector3> VoxelizeSelector()
    {
        List<Vector3> selectorPoints = new();
        // Get the bounds of the instantiated object
        Bounds bounds = instantiatedObject.GetComponent<MeshRenderer>().bounds;

        Vector3Int minBounds = Vector3Int.FloorToInt(VoxelManager.RoundToVoxel(bounds.min) / PrefabsManager.voxelSize);
        Vector3Int maxBounds = Vector3Int.FloorToInt(VoxelManager.RoundToVoxel(bounds.max) / PrefabsManager.voxelSize);

        Vector3 voxelSizeVector = Vector3.one * PrefabsManager.voxelSize;

        for (int x = minBounds.x; x <= maxBounds.x; x++)
        {
            for (int y = minBounds.y; y <= maxBounds.y; y++)
            {
                for (int z = minBounds.z; z <= maxBounds.z; z++)
                {
                    Vector3 coliderPose = new Vector3(x, y, z) * PrefabsManager.voxelSize;

                    bool checkBoxOverlap = Physics.CheckBox(coliderPose, voxelSizeVector / 2, Quaternion.identity, SelectorLayerMask);
                    Debug.Log(checkBoxOverlap);
                    if (checkBoxOverlap) selectorPoints.Add(coliderPose);
                }
            }
        }

        return selectorPoints;
    }

    public void Confirm()
    {
        List<Vector3> selectorPoints = VoxelizeSelector();
        if (additionSelected || assetAdditionSelected)
        {
            foreach (Vector3 point in selectorPoints)
            {
                VoxelManager.AddVoxel(point, true);
            }
        }
        else if (deletionSelected)
        {
            foreach (Vector3 point in selectorPoints)
            {
                VoxelManager.RemoveVoxel(point, true);
            }
        }
        else if (labelingSelected)
        {
            foreach (Vector3 point in selectorPoints)
            {
                VoxelManager.AddVoxel(point, true);
            }
        }
        Destroy(instantiatedObject);
        instantiatedObject = null;
        selectorInstantiated = false;
        doneInstantiation = false;
    }

    public void Cancel()
    {
        Destroy(instantiatedObject);
        instantiatedObject = null;
        selectorInstantiated = false;
        doneInstantiation = false;
    }

    public void OnAdditionSelected(bool state)
    {
        additionSelected = state;
    }

    public void OnDeletionSelected(bool state)
    {
        deletionSelected = state;
    }

    public void OnLabelingSelected(bool state)
    {
        labelingSelected = state;
    }

    public void OnAssetAdditionSelected(bool state)
    {
        assetAdditionSelected = state;
    }

    public void InstantiateAsset()
    {
        Debug.Log("INSTANTIATING ASSET");
        if (instantiatedObject != null) Destroy(instantiatedObject);
        Vector3 assetPose = new();
        assetPose.x = Camera.main.transform.localPosition.x + 2 * Mathf.Sin(Camera.main.transform.localRotation.eulerAngles.y * Mathf.Deg2Rad);
        assetPose.y = Camera.main.transform.localPosition.y - 1.5f;
        assetPose.z = Camera.main.transform.localPosition.z + 2 * Mathf.Cos(Camera.main.transform.localRotation.eulerAngles.y * Mathf.Deg2Rad);

        Vector3 assetRotation = new();
        assetRotation.Set(0, Camera.main.transform.localRotation.eulerAngles.y, 0);

        instantiatedObject = Instantiate(PrefabsManager.Asset, assetPose, Quaternion.Euler(assetRotation));
        
        AssetsMenu.SetActive(false);
        SelectorOptionsMenu.SetActive(true);
        // inputActionHandler.enabled = false;
    }

    public void ChangeMenu()
    {
        if (additionSelected) {
            VoxelsMenu.SetActive(true);
            menuText.text = "Add Voxels";
        }
        if (deletionSelected)
        {
            VoxelsMenu.SetActive(true);
            menuText.text = "Delete Voxels";
        }
        else if (assetAdditionSelected) AssetsMenu.SetActive(true);
    }



}
