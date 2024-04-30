using UnityEngine;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine.UI;
using System;

public class FingerPose : MonoBehaviour
{
    Vector3 InitialPose, FinalPose, PrismCenter, Scale_incubes, AssetPose, AssetRot, coliderPose, cubesizeScale;
    Vector3Int InitialPose_incubes, FinalPose_incubes, minbound_inCubes, maxbound_inCubes;
    GameObject Prism, ModelTarget, Selector;

    public GameObject[] Selectors = new GameObject[8];
    public GameObject[] VuforiaTargets = new GameObject[6];
    
    public LabelerFingerPose Labeler;
    public MinecraftBuilder _MinecraftBuilder;
    public RosPublisherExample _RosPublisher;

    Microsoft.MixedReality.Toolkit.Utilities.MixedRealityPose poseLeftIndex, poseLeftThumb; //new
    
    float cubesize, HandAngleThreshold, fingersThreshold;
    bool EditorActivator, selectorInstantiated, trackingLost, fingersClosed, doneInstantiation, testingBool, ConvexityState, DeletingVoxels, AddingAssets, VuforiaEnabled, VuforiaFound, HandAngle, EnablePrism;

    Renderer selectorMesh;

    Collider[] overlaps;
    public GameObject appBar;
    byte AssetLabel, AssetInstance;
 
    
    MeshCollider _meshCollider;
    InputActionHandler _inputActionHandler;
    string AssetName;
    private void Start()
    {
        cubesize = _MinecraftBuilder.cubesize;
        cubesizeScale.Set(cubesize, cubesize, cubesize);

        HandAngleThreshold = 30;
        EnablePrism = false;  //enabled when the user gestures a pinch
        EditorActivator = false; //enabled from the 'Edit Voxels' button
        doneInstantiation = false;
        testingBool = true;
        DeletingVoxels = false;
        AddingAssets = false;
        
        fingersThreshold = 0.04f;
        Prism = Selectors[3];
        _meshCollider = Prism.GetComponent<MeshCollider>();
        _inputActionHandler = gameObject.GetComponent<InputActionHandler>();

    }

    public void Update()
    {
        if (EditorActivator)
        {
            if (!doneInstantiation)
            {
                if (HandJointUtils.TryGetJointPose(Microsoft.MixedReality.Toolkit.Utilities.TrackedHandJoint.IndexTip, Microsoft.MixedReality.Toolkit.Utilities.Handedness.Right, out poseLeftIndex))
                {
                    HandJointUtils.TryGetJointPose(Microsoft.MixedReality.Toolkit.Utilities.TrackedHandJoint.ThumbTip, Microsoft.MixedReality.Toolkit.Utilities.Handedness.Right, out poseLeftThumb);
                    fingersClosed = Vector3.Distance(poseLeftIndex.Position, poseLeftThumb.Position) < fingersThreshold;
                    if (selectorInstantiated)
                    {
                        if (trackingLost)
                        {
                            if (fingersClosed)
                            {
                                //Update size of selector
                                FinalPose = poseLeftIndex.Position;
                                //converting units to cubes
                                FinalPose_incubes.Set(Mathf.RoundToInt(FinalPose.x / cubesize), Mathf.RoundToInt(FinalPose.y / cubesize), Mathf.RoundToInt(FinalPose.z / cubesize));
                                PrismCenter = (InitialPose_incubes + FinalPose_incubes);
                                Scale_incubes.x = Mathf.Max(Mathf.Abs((InitialPose_incubes.x - FinalPose_incubes.x) * cubesize) + cubesize, cubesize);
                                Scale_incubes.y = Mathf.Max(Mathf.Abs((InitialPose_incubes.y - FinalPose_incubes.y) * cubesize) + cubesize, cubesize);
                                Scale_incubes.z = Mathf.Max(Mathf.Abs((InitialPose_incubes.z - FinalPose_incubes.z) * cubesize) + cubesize, cubesize);
                                //transform selector
                                Selector.transform.position = PrismCenter * cubesize / 2;
                                Selector.transform.localScale = Scale_incubes;

                            }
                            else
                            {
                                //Destroy selector
                                Destroy(Selector);
                                selectorInstantiated = false;

                            }
                            trackingLost = false;
                        }
                        else
                        {
                            if (fingersClosed)
                            {
                                //Update size of selector
                                FinalPose = poseLeftIndex.Position;
                                //converting units to cubes
                                FinalPose_incubes.Set(Mathf.RoundToInt(FinalPose.x / cubesize), Mathf.RoundToInt(FinalPose.y / cubesize), Mathf.RoundToInt(FinalPose.z / cubesize));
                                PrismCenter = (InitialPose_incubes + FinalPose_incubes);

                                //without extra cubesize
                                Scale_incubes.x = Mathf.Max(Mathf.Abs((InitialPose_incubes.x - FinalPose_incubes.x) * cubesize), cubesize);
                                Scale_incubes.y = Mathf.Max(Mathf.Abs((InitialPose_incubes.y - FinalPose_incubes.y) * cubesize), cubesize);
                                Scale_incubes.z = Mathf.Max(Mathf.Abs((InitialPose_incubes.z - FinalPose_incubes.z) * cubesize), cubesize);

                                
                                //transform selector
                                Selector.transform.position = PrismCenter * cubesize / 2;
                                Selector.transform.localScale = Scale_incubes;

                            }
                            else   //successful instantiation process
                            {
                                //Set selector script
                                //selectorInstantiated = false; this should happen in the else of doneInstantiation
                                doneInstantiation = true;
                                //selectorInstantiated = false;
                                appBar.SetActive(true);
                            }
                        }

                    }
                    else  //here the instantation happens
                    {
                        HandAngle = Vector3.Angle(Camera.main.transform.forward, (poseLeftIndex.Position - Camera.main.transform.position)) < HandAngleThreshold;
                        if (fingersClosed && HandAngle)
                        {
                            //selector instantiation
                            InitialPose = poseLeftIndex.Position;
                            InitialPose_incubes.Set(Mathf.RoundToInt(InitialPose.x / cubesize), Mathf.RoundToInt(InitialPose.y / cubesize), Mathf.RoundToInt(InitialPose.z / cubesize));
                            Selector = Instantiate(Prism, InitialPose_incubes, Quaternion.identity);
                            Selector.name = "Prism";
                            selectorInstantiated = true;
                        }
                        else
                        {

                            //selectorInstantiated = false; //don't think i need it (the whole else statement)
                        }
                    }
                }
                else
                {
                    //When tracking is off:
                    if (selectorInstantiated)
                    {
                        trackingLost = true;
                    }
                    else
                    {
                        trackingLost = false;
                    }
                }
            }
            else
            {
                /*//Debug.Log("Done"); //app bar should confirm a boolean here. to finalize the editing phase
                selectorMesh = Selector.GetComponent<MeshFilter>();
                vertices = selectorMesh.mesh.vertices;
                foreach (Vector3 vertex in vertices)
                {
                    Debug.Log("Vertex position: " + selectorMesh.transform.TransformPoint(vertex));
                }*/
                //vertexExtractor();
                //testingBool = false;
                selectorInstantiated = false;
            }
            
        }

        if (VuforiaFound)
        {
            Selector.transform.position = ModelTarget.transform.position;
            Selector.transform.rotation = ModelTarget.transform.rotation;
        }

    }

    public void ActivateEditor(bool state)
    {    // Called by the "Add Voxels", "Delete Voxels", and "Home" buttons in the Edit menu.
        EditorActivator = state;
    }

    public void officialVoxelizer()
    {
        // Transforms the selector shape into voxels and saves the center of each voxel as a pointcloud ROS message.
        selectorMesh = Selector.GetComponent<Renderer>();

        //Rounding of the bounds to units of cubes:
        minbound_inCubes.Set(Mathf.RoundToInt(selectorMesh.bounds.min.x / cubesize), 
                             Mathf.RoundToInt(selectorMesh.bounds.min.y / cubesize), 
                             Mathf.RoundToInt(selectorMesh.bounds.min.z / cubesize));
        
        maxbound_inCubes.Set(Mathf.RoundToInt(selectorMesh.bounds.max.x / cubesize),
                             Mathf.RoundToInt(selectorMesh.bounds.max.y / cubesize),
                             Mathf.RoundToInt(selectorMesh.bounds.max.z / cubesize));


        //Loop from min to max bound:
        for(int i = minbound_inCubes.x; i <= maxbound_inCubes.x; i++)
        {
            
            for (int j = minbound_inCubes.y; j <= maxbound_inCubes.y; j++)
            {
                for (int k = minbound_inCubes.z; k <= maxbound_inCubes.z; k++)
                {
                    coliderPose.Set(i, j, k);
                    coliderPose = coliderPose * cubesize;

                    overlaps = Physics.OverlapBox(coliderPose, cubesizeScale / 2);
                    if (overlaps != null)
                    {
                        foreach(Collider overlap in overlaps)
                        {
                            if(overlap.gameObject.name == "Prism")
                            {
                                //coliderPose = coliderPose / cubesize;
                                //coliderPose = coliderPose * 0.0499f;
                                //_MinecraftBuilder.Instantiator(coliderPose, true);
                                if (AddingAssets || VuforiaEnabled)
                                {
                                    _MinecraftBuilder.UserAssetAddition(coliderPose);
                                    _RosPublisher.LabeledPointCloudPopulater(coliderPose, AssetLabel, AssetInstance);
                                }
                                else if (DeletingVoxels)
                                {
                                    _MinecraftBuilder.UserVoxelDeletion(coliderPose);
                                }
                                else
                                {
                                    _MinecraftBuilder.UserVoxelAddition(coliderPose);
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
        // Called by the "Abort" button in the editors app bar. Aborts a created selector and enables the user to create another. TODO: selector should also be aborted when someone exits the editing menu.
        if (Selector != null) Destroy(Selector);
        appBar.SetActive(false);
        doneInstantiation = false;
        if (AddingAssets)
        {
            _inputActionHandler.enabled = true;
        }
        else if (VuforiaEnabled)
        {
            VuforiaFound = false;
            ModelTarget.SetActive(false);
        }
    }

    public void confirmSelector()
    {
        // Called by the "Confirm" button in the app bar. Calls officialVoxelizer(), publishes the saved pointcloud, listens to new user input, and more case specific actions.
        if (AddingAssets)
        {
            _inputActionHandler.enabled = true;
            AssetInstance = Labeler.AssetInstance(AssetLabel);
            Labeler.AssetToolTip(Selector.transform.position, AssetName, AssetLabel, AssetInstance);
            _MinecraftBuilder.AddedVoxelByte.Clear();
            officialVoxelizer();
            _RosPublisher.PublishAddedPointCloudMsg();
            _RosPublisher.LabelPublisher();

        }
        else if (DeletingVoxels)
        {
            _MinecraftBuilder.DeletedVoxelByte.Clear();
            officialVoxelizer();
            _RosPublisher.PublishDeletedPointCloudMsg();
            doneInstantiation = false;

        }
        else if (VuforiaEnabled)
        {
            AssetInstance = Labeler.AssetInstance(AssetLabel);
            Labeler.AssetToolTip(Selector.transform.position, AssetName, AssetLabel, AssetInstance);
            _MinecraftBuilder.AddedVoxelByte.Clear();
            officialVoxelizer();
            _RosPublisher.PublishAddedPointCloudMsg();
            _RosPublisher.LabelPublisher();
            VuforiaFound = false;
            ModelTarget.SetActive(false);

        }
        else
        {
            _MinecraftBuilder.AddedVoxelByte.Clear();
            officialVoxelizer();
            _RosPublisher.PublishAddedPointCloudMsg();
            doneInstantiation = false;
        }
        
        Destroy(Selector);
        appBar.SetActive(false);
        
    }

    public void adjustSelector()
    {
        // Called by the "Adjust" button in the app bar. enables the object manipulator script.
        Selector.GetComponent<ObjectManipulator>().enabled = true;
    }

    public void doneSelector()
    {
        // Called by the "Done" button in the app bar. disables the object manipulator script.
        Selector.GetComponent<ObjectManipulator>().enabled = false;
    }

    public void requestSelectorShape(int index)
    {
        // Changes the selector shape based on what the user chose. Called from the shape selector menu by each button.
        Prism = Selectors[index];
        _meshCollider = Selectors[index].GetComponent<MeshCollider>();
        _meshCollider.convex = ConvexityState;
    }

    public void Convexity(bool state)
    {
        // Called by the convex button.
        _meshCollider.convex = state;
        ConvexityState = state;
    }

    public void AssetInstantiator()
    {
        // Instantiates the asset 2 meters infront of the user and 1.5 meters lower than the camers (Head of the user)
        
        if (Selector != null) Destroy(Selector);
        
        AssetPose.x = Camera.main.transform.localPosition.x + 2 * Mathf.Sin(Camera.main.transform.localRotation.eulerAngles.y * Mathf.Deg2Rad);
        AssetPose.y = Camera.main.transform.localPosition.y - 1.5f;
        AssetPose.z = Camera.main.transform.localPosition.z + 2 * Mathf.Cos(Camera.main.transform.localRotation.eulerAngles.y * Mathf.Deg2Rad);
        
        AssetRot.Set(0, Camera.main.transform.localRotation.eulerAngles.y, 0);
        
        Selector = Instantiate(Prism, AssetPose, Quaternion.Euler(AssetRot));
        Selector.name = "Prism";
        appBar.SetActive(true);
        _inputActionHandler.enabled = false;
    }

    public void EnableAssetAddition(bool state)
    {
        // Enabled by the "Add Asset" button. and disabled by the "Back" button.

        AddingAssets = state;
        _inputActionHandler.enabled = state;
    }

    public void AssetLabelNumber(int label)
    {
        // Called when selecting an asset. Specifies the the label value that will be sent in the pointcloud message.
        AssetLabel = (byte)label;
    }

    public void AssetNameForTooltip(string name)
    {
        // Called by selecting an asset. Specfies the name of the asset for the tooltip.
        AssetName = name;
    }

    public void EnableDeletion(bool state)
    {
        // Called by the "Delete Voxels" button. Triggered to false by the back button
        DeletingVoxels = state;
    }

    public void EnableVuforia(bool state)
    {
        //  Called by selecting a vuforia asset shape. Triggered to false by the back button or when user is done with first vuforia run (After confirmation).
        VuforiaEnabled = state;

        if (!state)
        {
            VuforiaFound = false;
            ModelTarget.SetActive(false);
        }
    }

    public void VuforiaTargetRequest(int index)
    {
        // Grabs the model target object from the "Vuforia Targets" game object. Called by  selecting a vuforia asset shape.
        VuforiaTargets[index].SetActive(true);
        ModelTarget = VuforiaTargets[index];
    }

    public void ModelTracked(GameObject Object)
    {
        // Called by vuforia when a target is found. The update loop handles the alignment of the instantiated selector and the tracked physical asset.
        if (!VuforiaFound)
        {
            Prism = Object;
            Selector = Instantiate(Prism, ModelTarget.transform.position, ModelTarget.transform.rotation);
            Selector.name = "Prism";
            appBar.SetActive(true);
            VuforiaFound = true;
        }

    }

}