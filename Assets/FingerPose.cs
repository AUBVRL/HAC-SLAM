using UnityEngine;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class FingerPose : MonoBehaviour
{
    public GameObject IndicatorPrefab, Indicator;
    public GameObject AddVoxelsMenu, DeleteVoxelsMenu, AddAssetsMenu, AdjustConfirmAbortMenu;
    bool instantiatedIndicator, additionSelected;
    float zDepth;
    Vector3 InitialPose, FinalPose, PrismCenter, Scale_incubes, AssetPose, AssetRot;
    Vector3Int InitialPose_incubes, FinalPose_incubes;
    GameObject Prism, ModelTarget;
    public GameObject[] Selectors = new GameObject[8];
    public GameObject[] VuforiaTargets = new GameObject[6];
    public LabelerFingerPose Labeler;
    public MinecraftBuilder _MinecraftBuilder;
    // public RosPublisherExample _RosPublisher;
    float cubesize;
    public static GameObject Selector;
    bool EditorActivator, selectorInstantiated, doneInstantiation, ConvexityState, DeletingVoxels, AddingAssets;

    Renderer selectorMesh;
    Vector3 minbound, maxbound; //delete 

    Vector3Int minbound_inCubes, maxbound_inCubes;
    Vector3 coliderPose, cubesizeScale;
    Collider[] overlaps;
    public GameObject appBar;
    byte AssetLabel, AssetInstance;

    //MixedRealityInputAction selectAction;
    bool EnablePrism;
    MeshCollider _meshCollider;
    InputActionHandler _inputActionHandler;
    string AssetName;
    Vector3 mousePosition;
    int SelectorLayerMask = 1 << 6;

    private void Start()
    {
        instantiatedIndicator = false;
        zDepth = Camera.main.nearClipPlane;
        cubesize = _MinecraftBuilder.cubesize;
        EnablePrism = false;  //enabled when the user gestures a pinch
        EditorActivator = false; //enabled from the 'Edit Voxels' button
        doneInstantiation = false;
        DeletingVoxels = false;
        AddingAssets = false;
        cubesizeScale.Set(cubesize, cubesize, cubesize);
        Prism = Selectors[3];
        _meshCollider = Prism.GetComponent<MeshCollider>();
        _inputActionHandler = gameObject.GetComponent<InputActionHandler>();

        //Debug.Log("start");

        //_meshCollider.convex = true;  // We need to make this as a kabse later.
    }

    public void DisableIndicator()
    {
        Indicator.SetActive(false);
    }

    public void Update()
    {
        if (EditorActivator)
        {
            HandleMouseInput();
        }
    }

    void HandleMouseInput()
    {
        if (!doneInstantiation)
        {

            UpdateIndicatorPosition();
            if (AddingAssets) HandleAssetInstantiation();
            else HandleSelectorInstantiation();
        }
    }

    void HandleSelectorInstantiation()
    {
        if (Input.GetKey(KeyCode.Space) && Input.GetMouseButtonDown(0) && !selectorInstantiated)
        {
            InstantiateSelector();
        }

        else if (selectorInstantiated && !doneInstantiation)
        {
            StretchSelector();

            if (!Input.GetMouseButton(0))
            {
                doneInstantiation = true;
                selectorInstantiated = false;
                instantiatedIndicator = false;
                Destroy(Indicator);
                if (DeletingVoxels) DeleteVoxelsMenu.SetActive(false);
                else AddVoxelsMenu.SetActive(false);
                AdjustConfirmAbortMenu.SetActive(true);
            }
        }
    }

    void UpdateIndicatorPosition()
    {
        zDepth += Input.GetAxis("Mouse ScrollWheel");
        mousePosition = Input.mousePosition;
        mousePosition.z = zDepth;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        if (!instantiatedIndicator)
        {
            Indicator = Instantiate(IndicatorPrefab, mousePosition, Quaternion.identity);
            instantiatedIndicator = true;
        }
        else
        {
            if (!Indicator.activeInHierarchy) Indicator.SetActive(true);
            Indicator.transform.position = mousePosition;
        }
    }

    void InstantiateSelector()
    {
        InitialPose = mousePosition;
        InitialPose_incubes.Set(Mathf.RoundToInt(InitialPose.x / cubesize), Mathf.RoundToInt(InitialPose.y / cubesize), Mathf.RoundToInt(InitialPose.z / cubesize));
        Vector3 center = new Vector3();
        center.Set(InitialPose_incubes.x * cubesize, InitialPose_incubes.y * cubesize, InitialPose_incubes.z * cubesize);
        Selector = Instantiate(Prism, center, Quaternion.identity);
        Selector.name = "Prism";
        selectorInstantiated = true;
    }

    void StretchSelector()
    {
        FinalPose = mousePosition;
        Indicator.transform.position = mousePosition;
        FinalPose_incubes.Set(Mathf.RoundToInt(FinalPose.x / cubesize), Mathf.RoundToInt(FinalPose.y / cubesize), Mathf.RoundToInt(FinalPose.z / cubesize));
        PrismCenter = (InitialPose_incubes + FinalPose_incubes);
        Scale_incubes.x = Mathf.Max(Mathf.Abs((InitialPose_incubes.x - FinalPose_incubes.x) * cubesize), cubesize);
        Scale_incubes.y = Mathf.Max(Mathf.Abs((InitialPose_incubes.y - FinalPose_incubes.y) * cubesize), cubesize);
        Scale_incubes.z = Mathf.Max(Mathf.Abs((InitialPose_incubes.z - FinalPose_incubes.z) * cubesize), cubesize);
        Selector.transform.position = PrismCenter * cubesize / 2;
        Selector.transform.localScale = Scale_incubes;
    }


    void HandleAssetInstantiation()
    {
        if (Input.GetKey(KeyCode.Space) && Input.GetMouseButtonDown(0))
        {
            AssetPose = mousePosition;
            Selector = Instantiate(Prism, AssetPose, Quaternion.identity);
            Selector.name = "Prism";
            instantiatedIndicator = false;
            Destroy(Indicator);
            AddAssetsMenu.SetActive(false);
            AdjustConfirmAbortMenu.SetActive(true);
            doneInstantiation = true;
        }
    }



    public void ActivateEditor(bool state)
    {
        EditorActivator = state;
    }

    public void officialVoxelizer()
    {
        selectorMesh = Selector.GetComponent<Renderer>();

        //Rounding of the bounds to units of cubes:
        minbound_inCubes.Set(Mathf.RoundToInt(selectorMesh.bounds.min.x / cubesize),
                             Mathf.RoundToInt(selectorMesh.bounds.min.y / cubesize),
                             Mathf.RoundToInt(selectorMesh.bounds.min.z / cubesize));

        maxbound_inCubes.Set(Mathf.RoundToInt(selectorMesh.bounds.max.x / cubesize),
                             Mathf.RoundToInt(selectorMesh.bounds.max.y / cubesize),
                             Mathf.RoundToInt(selectorMesh.bounds.max.z / cubesize));


        //Loop from min to max bound:
        for (int i = minbound_inCubes.x; i <= maxbound_inCubes.x; i++)
        {
            //test
            for (int j = minbound_inCubes.y; j <= maxbound_inCubes.y; j++)
            {
                for (int k = minbound_inCubes.z; k <= maxbound_inCubes.z; k++)
                {
                    coliderPose.Set(i, j, k);
                    coliderPose = coliderPose * cubesize;

                    overlaps = Physics.OverlapBox(coliderPose, cubesizeScale / 2);
                    if (overlaps != null)
                    {
                        foreach (Collider overlap in overlaps)
                        {
                            if (overlap.gameObject.name == "Prism")
                            {
                                //coliderPose = coliderPose / cubesize;
                                //coliderPose = coliderPose * 0.0499f;
                                //_MinecraftBuilder.Instantiator(coliderPose, true);
                                if (DeletingVoxels)
                                {
                                    VoxelManager.DeleteVoxel(coliderPose);
                                }
                                else
                                {
                                    VoxelManager.AddVoxel(coliderPose, true);
                                    // if (AddingAssets) _RosPublisher.LabeledPointCloudPopulater(coliderPose, AssetLabel, AssetInstance);
                                }
                                break;
                            }
                        }
                    }
                }
            }
        }
    }


    public void abortSelector()
    {
        if (Selector != null) Destroy(Selector);
        appBar.SetActive(false);
        doneInstantiation = false;
        if (AddingAssets)
        {
            AddAssetsMenu.SetActive(true);
            _inputActionHandler.enabled = true;
        }
        else if (DeletingVoxels)
        {
            DeleteVoxelsMenu.SetActive(true);
        }
        else // Adding Voxels
        {
            AddVoxelsMenu.SetActive(true);
        }
    }

    public void confirmSelector()
    {
        doneInstantiation = false;
        List<Vector3> selectorPoints = VoxelizeSelector();
        //if (AddingAssets)
        //{
        //    AddAssetsMenu.SetActive(true);
        //    AssetInstance = Labeler.AssetInstance(AssetLabel);
        //    Labeler.AssetToolTip(Selector.transform.position, AssetName, AssetLabel, AssetInstance);
        //    _MinecraftBuilder.AddedVoxelByte.Clear();
        //    officialVoxelizer();
        //    // _RosPublisher.PublishEditedPointCloudMsg();
        //    // _RosPublisher.LabelPublisher();
        //}

        //else if (DeletingVoxels)
        //{
        //    DeleteVoxelsMenu.SetActive(true);
        //    _MinecraftBuilder.DeletedVoxelByte.Clear();
        //    officialVoxelizer();
        //    //  _RosPublisher.PublishDeletedVoxels();
        //}

        //else
        //{
        //    AddVoxelsMenu.SetActive(true);
        //    _MinecraftBuilder.AddedVoxelByte.Clear();
        //    officialVoxelizer();
        //    //  _RosPublisher.PublishEditedPointCloudMsg();

        //}

        if (additionSelected)
        {
            AddVoxelsMenu.SetActive(true);
            var states = new List<bool>(selectorPoints.Count);
            for (int i = 0; i < selectorPoints.Count; ++i) states.Add(true);

            // call the batch API once so Undo/Redo treats the whole selection as one action
            VoxelUtilities.AddVoxelsWithUndo(selectorPoints, states);
        }
        else if (DeletingVoxels)
        {
            DeleteVoxelsMenu.SetActive(true);
            VoxelUtilities.DeleteVoxelsWithUndo(selectorPoints);
        }
        else if (AddingAssets)
        {
            AddAssetsMenu.SetActive(true);
            var states = new List<bool>(selectorPoints.Count);
            for (int i = 0; i < selectorPoints.Count; ++i) states.Add(true);

            // call the batch API once so Undo/Redo treats the whole selection as one action
            VoxelUtilities.AddVoxelsWithUndo(selectorPoints, states);
        }

        Destroy(Selector);
        appBar.SetActive(false);

    }

    public void requestSelectorShape(int index)
    {
        Debug.Log("CHANGING SHAPE");
        Prism = Selectors[index];
        _meshCollider = Selectors[index].GetComponent<MeshCollider>();
        if (additionSelected || AddingAssets)
        {
            Convexity(false);
        }
        else
        {
            Convexity(true);
        }
    }

    public void Convexity(bool state)
    {
        _meshCollider.convex = state;
        ConvexityState = state;
    }

    public void EnableAssetAddition(bool state)
    {
        AddingAssets = state;
        // _inputActionHandler.enabled = state;          NO NEED ON DESKTOP APPLICATION!
    }

    public void AssetLabelNumber(int label)
    {
        AssetLabel = (byte)label;
    }

    public void AssetNameForTooltip(string name)
    {
        AssetName = name;
    }

    public void EnableDeletion(bool state)
    {
        DeletingVoxels = state;
    }

    public void EnableAddition(bool state)
    {
        additionSelected = state;
    }

    List<Vector3> VoxelizeSelector()
    {
        List<Vector3> selectorPoints = new();
        // Get the bounds of the instantiated object
        Selector.transform.position = Selector.transform.position + Vector3.one * 0.0001f;
        Bounds bounds = Selector.GetComponent<MeshRenderer>().bounds;

        Vector3Int minBounds = Vector3Int.FloorToInt(VoxelManager.RoundToVoxel(bounds.min) / PrefabsManager.voxelSize);
        Vector3Int maxBounds = Vector3Int.CeilToInt(VoxelManager.RoundToVoxel(bounds.max) / PrefabsManager.voxelSize);

        Vector3 voxelSizeVector = Vector3.one * PrefabsManager.voxelSize;

        for (int x = minBounds.x; x <= maxBounds.x; x++)
        {
            for (int y = minBounds.y; y <= maxBounds.y; y++)
            {
                for (int z = minBounds.z; z <= maxBounds.z; z++)
                {
                    Vector3 coliderPose = new Vector3(x, y, z) * PrefabsManager.voxelSize;

                    bool checkBoxOverlap = Physics.CheckBox(coliderPose, voxelSizeVector / 2, Quaternion.identity, SelectorLayerMask);
                    //Debug.Log(checkBoxOverlap);
                    if (checkBoxOverlap) selectorPoints.Add(coliderPose);
                }
            }
        }

        return selectorPoints;
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