using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine.Assertions;
using TMPro;

public class EditsManager : MonoBehaviour
{
    public TextMeshPro menuText;
    public GameObject VoxelsMenu,DeleteMenu, SelectorOptionsMenu;
    public GameObject AssetsMenu;
    bool doneInstantiation, selectorInstantiated, fingersClosed, handAngle;
    bool additionSelected, deletionSelected, labelingSelected;
    public static bool assetAdditionSelected;
    Vector3 initialPoseInCubes;
    public static GameObject instantiatedObject;
    Microsoft.MixedReality.Toolkit.Utilities.MixedRealityPose poseRightIndex;
    Microsoft.MixedReality.Toolkit.Utilities.MixedRealityPose poseRightThumb;
    IMixedRealityHandJointService handJointService;
    public InputActionHandler inputActionHandler;
    int SelectorLayerMask = 1 << 6;
    float HandAngleThreshold = 15.0f;

    void Start()
    {
        doneInstantiation = false;
        selectorInstantiated = false; 
        initialPoseInCubes = new();
        handJointService = CoreServices.GetInputSystemDataProvider<IMixedRealityHandJointService>();
        //inputActionHandler = gameObject.GetComponent<InputActionHandler>();
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
            handAngle = Vector3.Angle(Camera.main.transform.forward, (poseRightIndex.Position - Camera.main.transform.position)) < HandAngleThreshold;
        }
    }

    void InstantiateSelector()
    {
        if (fingersClosed && handAngle)
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
            DeleteMenu.SetActive(false);
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
            //foreach (Vector3 point in selectorPoints)
            //{
            //    //VoxelManager.AddVoxel(point, true);
            //    VoxelUtilities.AddVoxelsWithUndo(point, true);
            //}
            var states = new List<bool>(selectorPoints.Count);
            for (int i = 0; i < selectorPoints.Count; ++i) states.Add(true);

            // call the batch API once so Undo/Redo treats the whole selection as one action
            VoxelUtilities.AddVoxelsWithUndo(selectorPoints, states);
        }
        else if (deletionSelected)
        {
            //foreach (Vector3 point in selectorPoints)
            //{
            //    //VoxelManager.DeleteVoxel(point);
            //    VoxelUtilities.DeleteVoxelsWithUndo(point);
            //}
            VoxelUtilities.DeleteVoxelsWithUndo(selectorPoints);
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
        if (assetAdditionSelected)
        {
            inputActionHandler.enabled = true;
        }
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
        PrefabsManager.SelectorOnConvexityToggle(false);
    }

    public void OnDeletionSelected(bool state)
    {
        deletionSelected = state;
        PrefabsManager.SelectorOnConvexityToggle(true);
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

        //Vector3 assetRotation = new();
        //assetRotation.Set(0, Camera.main.transform.localRotation.eulerAngles.y, 0);

        instantiatedObject = Instantiate(PrefabsManager.Asset, assetPose, Quaternion.identity);
        
        AssetsMenu.SetActive(false);
        SelectorOptionsMenu.SetActive(true);
        inputActionHandler.enabled = false;
    }

    public void ChangeMenu()
    {
        if (additionSelected) {
            VoxelsMenu.SetActive(true);
            menuText.text = "Add Voxels";
        }
        if (deletionSelected)
        {
            DeleteMenu.SetActive(true);
            menuText.text = "Delete Voxels";
        }
        else if (assetAdditionSelected) AssetsMenu.SetActive(true);
    }

    public void UndoAction()
    {
        UndoRedoManager2.Undo();
    }

    public void RedoAction()
    {
        UndoRedoManager2.Redo();
    }

}
