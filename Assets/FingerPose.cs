using UnityEngine;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine.UI;
using System;
using TMPro;

public class FingerPose : MonoBehaviour
{
    public GameObject IndicatorPrefab, Indicator;
    public GameObject AddVoxelsMenu, DeleteVoxelsMenu, AddAssetsMenu, AdjustConfirmAbortMenu;
    bool instantiatedIndicator;
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
    public GameObject Selector;
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

        Debug.Log("start");

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

    public void setAxes(bool state)
    {
        Selector.GetComponent<AxisDrag>().enabled = state;
        if (Selector.GetComponent<AxisDrag>().xArrow != null)
        {
            Selector.GetComponent<AxisDrag>().xArrow.SetActive(state);
            Selector.GetComponent<AxisDrag>().yArrow.SetActive(state);
            Selector.GetComponent<AxisDrag>().zArrow.SetActive(state);
        }
    }

    public void allowMotionLocalAxes(bool state)
    {
        Selector.GetComponent<AxisDrag>().allowedMotion = state;
    }

    public void setAxesGlobal(bool state)
    {
        Selector.GetComponent<AxisDragGlobal>().enabled = state;
        if (Selector.GetComponent<AxisDragGlobal>().xArrow != null)
        {
            Selector.GetComponent<AxisDragGlobal>().xArrow.SetActive(state);
            Selector.GetComponent<AxisDragGlobal>().yArrow.SetActive(state);
            Selector.GetComponent<AxisDragGlobal>().zArrow.SetActive(state);
        }

    }

    public void allowMotionGloballAxes(bool state)
    {
        Selector.GetComponent<AxisDragGlobal>().allowedMotion = state;
    }

    public void setScaleAxes(bool state)
    {
        Selector.GetComponent<ScaleWithAxes>().enabled = state;
        if (Selector.GetComponent<ScaleWithAxes>().xArrow != null)
        {
            Selector.GetComponent<ScaleWithAxes>().xArrow.SetActive(state);
            Selector.GetComponent<ScaleWithAxes>().yArrow.SetActive(state);
            Selector.GetComponent<ScaleWithAxes>().zArrow.SetActive(state);
        }
    }

    public void setCircles(bool state)
    {
        Selector.GetComponent<RotateWithCircles>().enabled = state;
        if (Selector.GetComponent<RotateWithCircles>().xCircle != null)
        {
            Selector.GetComponent<RotateWithCircles>().xCircle.SetActive(state);
            Selector.GetComponent<RotateWithCircles>().yCircle.SetActive(state);
            Selector.GetComponent<RotateWithCircles>().zCircle.SetActive(state);
        }
    }

    public void setCirclesGlobal(bool state)
    {
        Selector.GetComponent<RotateGlobal>().enabled = state;
        if (Selector.GetComponent<RotateGlobal>().xCircle != null)
        {
            Selector.GetComponent<RotateGlobal>().xCircle.SetActive(state);
            Selector.GetComponent<RotateGlobal>().yCircle.SetActive(state);
            Selector.GetComponent<RotateGlobal>().zCircle.SetActive(state);
        }
    }

    public void allowMotionGlobalCircles(bool state)
    {
        Selector.GetComponent<RotateGlobal>().allowedMotion = state;
    }

    public void deleteAxes()
    {
        Debug.Log("deleting local axes");
        if (Selector.GetComponent<AxisDrag>().xArrow != null)
        {
            Destroy(Selector.GetComponent<AxisDrag>().xArrow);
            Destroy(Selector.GetComponent<AxisDrag>().yArrow);
            Destroy(Selector.GetComponent<AxisDrag>().zArrow);
        }
        Selector.GetComponent<AxisDrag>().enabled = false;
    }

    public void deleteAxesGlobal()
    {
        Debug.Log("deleting global axes");
        if (Selector.GetComponent<AxisDragGlobal>().xArrow != null)
        {
            Destroy(Selector.GetComponent<AxisDragGlobal>().xArrow);
            Destroy(Selector.GetComponent<AxisDragGlobal>().yArrow);
            Destroy(Selector.GetComponent<AxisDragGlobal>().zArrow);
        }
        Selector.GetComponent<AxisDragGlobal>().enabled = false;
    }

    public void deleteScaleAxes()
    {
        Debug.Log("deleting scale axes");
        if (Selector.GetComponent<ScaleWithAxes>().xArrow != null)
        {
            Destroy(Selector.GetComponent<ScaleWithAxes>().xArrow);
            Destroy(Selector.GetComponent<ScaleWithAxes>().yArrow);
            Destroy(Selector.GetComponent<ScaleWithAxes>().zArrow);
        }
        Selector.GetComponent<ScaleWithAxes>().enabled = false;
    }

    public void deleteCircles()
    {
        Debug.Log("deleting local circles");
        if (Selector.GetComponent<RotateWithCircles>().xCircle != null)
        {
            Destroy(Selector.GetComponent<RotateWithCircles>().xCircle);
            Destroy(Selector.GetComponent<RotateWithCircles>().yCircle);
            Destroy(Selector.GetComponent<RotateWithCircles>().zCircle);
        }
        Selector.GetComponent<RotateWithCircles>().enabled = false;
    }

    public void deleteCirclesGlobal()
    {
        Debug.Log("deleting global circles");
        if (Selector.GetComponent<RotateGlobal>().xCircle != null)
        {
            Destroy(Selector.GetComponent<RotateGlobal>().xCircle);
            Destroy(Selector.GetComponent<RotateGlobal>().yCircle);
            Destroy(Selector.GetComponent<RotateGlobal>().zCircle);
        }
        Selector.GetComponent<RotateGlobal>().enabled = false;
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
        if (AddingAssets)
        {
            AddAssetsMenu.SetActive(true);
            AssetInstance = Labeler.AssetInstance(AssetLabel);
            Labeler.AssetToolTip(Selector.transform.position, AssetName, AssetLabel, AssetInstance);
            _MinecraftBuilder.AddedVoxelByte.Clear();
            officialVoxelizer();
            // _RosPublisher.PublishEditedPointCloudMsg();
            // _RosPublisher.LabelPublisher();
        }

        else if (DeletingVoxels)
        {
            DeleteVoxelsMenu.SetActive(true);
            _MinecraftBuilder.DeletedVoxelByte.Clear();
            officialVoxelizer();
            //  _RosPublisher.PublishDeletedVoxels();
        }

        else
        {
            AddVoxelsMenu.SetActive(true);
            _MinecraftBuilder.AddedVoxelByte.Clear();
            officialVoxelizer();
            //  _RosPublisher.PublishEditedPointCloudMsg();

        }

        Destroy(Selector);
        appBar.SetActive(false);

    }

    public void requestSelectorShape(int index)
    {
        Debug.Log("CHANGING SHAPE");
        Prism = Selectors[index];
        _meshCollider = Selectors[index].GetComponent<MeshCollider>();
        _meshCollider.convex = ConvexityState;
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


}