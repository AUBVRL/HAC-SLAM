using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using UnityEngine.InputSystem;
using Microsoft.MixedReality.Toolkit.Examples.Demos;

public class LabelerFingerPose : MonoBehaviour
{
    bool labelerOn, doneInstantiation, trackingLost, selectorInstantiated, fingersClosed;
    Microsoft.MixedReality.Toolkit.Utilities.MixedRealityPose poseLeft;
    Microsoft.MixedReality.Toolkit.Utilities.MixedRealityPose poseLeftIndex;
    Microsoft.MixedReality.Toolkit.Utilities.MixedRealityPose poseLeftThumb;
    float fingersThreshold, cubesize;
    Vector3 InitialPose, FinalPose, PrismCenter, Scale_incubes, coliderPose, cubesizeScale, ToolTipAnchor;
    Vector3Int InitialPose_incubes, FinalPose_incubes, minbound_inCubes, maxbound_inCubes;
    public MinecraftBuilder _minecraftbuilder;
    public RosPublisherExample Pub;
    GameObject Selector, tool;
    public GameObject appBar, Prism, tooltip;
    MeshCollider _meshCollider;
    Renderer selectorMesh;
    Collider[] overlaps;
    ToolTip tooltipText; //just now
    //ToolTipConnector tooltipconnector;  //just now
    int counterForVoxels;
    public Material SelectedMaterial;
    MeshRenderer VoxelMeshRenderer;
    SystemKeyboardExample key;
    public HoloKeyboard holoKey;

    // Start is called before the first frame update
    void Start()
    {
        fingersThreshold = 0.04f;
        cubesize = _minecraftbuilder.cubesize;
        cubesizeScale.Set(cubesize - 0.001f, cubesize - 0.001f, cubesize - 0.001f);
        ToolTipAnchor = Vector3.zero;
        _meshCollider = Prism.GetComponent<MeshCollider>();
        _meshCollider.convex = true;
        counterForVoxels = 0;
        labelerOn = true;
        doneInstantiation = false;
        //tooltipText = tooltip.GetComponent<ToolTip>();  //just now
        //tooltipconnector = tooltip.GetComponent<ToolTipConnector>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (labelerOn)
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

                                //with extra cubesize
                                /*Scale_incubes.x = Mathf.Max(Mathf.Abs((InitialPose_incubes.x - FinalPose_incubes.x) * cubesize) + cubesize, cubesize);
                                Scale_incubes.y = Mathf.Max(Mathf.Abs((InitialPose_incubes.y - FinalPose_incubes.y) * cubesize) + cubesize, cubesize);
                                Scale_incubes.z = Mathf.Max(Mathf.Abs((InitialPose_incubes.z - FinalPose_incubes.z) * cubesize) + cubesize, cubesize);*/
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
                        if (fingersClosed)
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
                selectorInstantiated = false;
            }
        }
    }

    public void labelVoxelizer()
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
                                
                                foreach (Collider overlap2 in overlaps)
                                {
                                    if (overlap2.gameObject.name == "Voxel")
                                    {
                                        VoxelMeshRenderer = overlap2.gameObject.GetComponent<MeshRenderer>();
                                        VoxelMeshRenderer.material = SelectedMaterial;
                                        overlap2.gameObject.name = "Labeled";
                                        //ToolTipAnchor += overlap2.gameObject.transform.position;
                                        //counterForVoxels++;
                                        //Pub.LabeledPointCloudPopulater(overlap2.gameObject.transform.position, 77 , 66);
                                        break;
                                        //Destroy(overlap2.gameObject);   //this works
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /////ToolTipAnchor = ToolTipAnchor / counterForVoxels; //center of mass for the selcted voxels to become the anchor for the tooltip
        //key.debugMessage.text = tooltipText.ToolTipText;
        //tooltipText.ToolTipText = "Chair";
        //tooltipconnector.Target = Selector; //The ToolTipAchor value should go here
        tool = Instantiate(tooltip, Selector.transform.position + new Vector3(0, (Selector.transform.localScale.y)/2,0), Quaternion.identity);
        tooltipText = tool.GetComponent<ToolTip>();
        //Destroy(Selector);
        //Pub.LabelPublisher();

    }

    public void abortSelector()
    {
        Destroy(Selector);
        appBar.SetActive(false);
        doneInstantiation = false;
    }

    public void confirmSelector()
    {
        //labelVoxelizer(); commented this for testing
        //Destroy(Selector);
        tooltipText.ToolTipText = holoKey.texty;
        appBar.SetActive(false);
        doneInstantiation = false;
    }

    public void adjustSelector()
    {
        Selector.GetComponent<ObjectManipulator>().enabled = true;
        Selector.GetComponent<BoxCollider>().enabled = true;
        Selector.GetComponent<BoundsControl>().enabled = true;
    }

    public void doneSelector()
    {
        Selector.GetComponent<BoxCollider>().enabled = false;
        Selector.GetComponent<BoundsControl>().enabled = false;
    }
    public void labelSelector()
    {
        labelVoxelizer();
        Destroy(Selector);
        holoKey.OpenKeyboard();
    }
}
