using UnityEngine;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;

public class FingerPose : MonoBehaviour
{
    Vector3 InitialPose, FinalPose, PrismCenter, Scale_incubes;
    Vector3Int InitialPose_incubes, FinalPose_incubes;
    public GameObject Prism;
    public GameObject Prism1;
    public GameObject Prism2;
    public GameObject Prism3;
    public GameObject sfiro;
    
    public MinecraftBuilder _MinecraftBuilder;
    Microsoft.MixedReality.Toolkit.Utilities.MixedRealityPose poseLeft;
    Microsoft.MixedReality.Toolkit.Utilities.MixedRealityPose poseLeftIndex; //new
    Microsoft.MixedReality.Toolkit.Utilities.MixedRealityPose poseLeftThumb; //new
    IMixedRealityHandJointService handJointService;
    float cubesize;
    float fingersThreshold = 0.04f;
    GameObject Selector;
    bool EditorActivator, EditorActivatorOld, selectorInstantiated, trackingLost, fingersClosed, doneInstantiation, testingBool;

    Renderer selectorMesh;

    Vector3 minbound, maxbound; //delete

    Vector3Int minbound_inCubes, maxbound_inCubes;
    Vector3 coliderPose, cubesizeScale;
    Collider[] overlaps;
    public GameObject appBar;
 
    //MixedRealityInputAction selectAction;
    bool EnablePrism;
    MeshCollider _meshCollider;
    private void Start()
    {
        handJointService = CoreServices.GetInputSystemDataProvider<IMixedRealityHandJointService>();
        cubesize = _MinecraftBuilder.cubesize;
        EnablePrism = false;  //enabled when the user gestures a pinch
        EditorActivator = true; //enabled from the 'Edit Voxels' button
        EditorActivatorOld = false;
        doneInstantiation = false;
        testingBool = true;
        cubesizeScale.Set(cubesize, cubesize, cubesize);
        fingersThreshold = 0.04f;
        _meshCollider = Prism.GetComponent<MeshCollider>();
        //_meshCollider.convex = true;  // We need to make this as a kabse later.
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
        



        /////// here is the old part
        /*if (EnablePrism == true && HandJointUtils.TryGetJointPose(Microsoft.MixedReality.Toolkit.Utilities.TrackedHandJoint.IndexTip, Microsoft.MixedReality.Toolkit.Utilities.Handedness.Right, out poseLeft))
        {

            FinalPose = poseLeft.Position;
            
            //Converting units to cubes:
            FinalPose_incubes.Set(Mathf.RoundToInt(FinalPose.x / cubesize), Mathf.RoundToInt(FinalPose.y / cubesize), Mathf.RoundToInt(FinalPose.z / cubesize));
            PrismCenter = (InitialPose_incubes + FinalPose_incubes);
            Scale_incubes.x = Mathf.Max(Mathf.Abs((InitialPose_incubes.x - FinalPose_incubes.x) * cubesize) + cubesize, cubesize);
            Scale_incubes.y = Mathf.Max(Mathf.Abs((InitialPose_incubes.y - FinalPose_incubes.y) * cubesize) + cubesize, cubesize);
            Scale_incubes.z = Mathf.Max(Mathf.Abs((InitialPose_incubes.z - FinalPose_incubes.z) * cubesize) + cubesize, cubesize);

            Selector.transform.position = PrismCenter * cubesize/2;
            Selector.transform.localScale = Scale_incubes;
            
        }*/
    }

    public void editor3D()  //instantiator
    {
        if (EditorActivatorOld)
        {
            if (HandJointUtils.TryGetJointPose(Microsoft.MixedReality.Toolkit.Utilities.TrackedHandJoint.IndexTip, Microsoft.MixedReality.Toolkit.Utilities.Handedness.Right, out poseLeft))
            {
                EnablePrism = !EnablePrism;
                InitialPose = poseLeft.Position;
                InitialPose_incubes.Set(Mathf.RoundToInt(InitialPose.x / cubesize), Mathf.RoundToInt(InitialPose.y / cubesize), Mathf.RoundToInt(InitialPose.z / cubesize));

                if (EnablePrism == true)
                {
                    Selector = Instantiate(Prism, InitialPose_incubes, Quaternion.identity);
                    Selector.name = "Prism";
                }
            }
        }
        
}

    public void CubeAdder()
    {
        if (EditorActivatorOld)
        {
            _MinecraftBuilder.InstantiateEditor(InitialPose_incubes, FinalPose_incubes);
            Destroy(Selector);
        }
        
    }

    public void CubeRemover()
    {
        if (EditorActivatorOld)
        {
            _MinecraftBuilder.DestroyEditor(InitialPose_incubes, FinalPose_incubes);
            Destroy(Selector);
        }
        
    }
   
    public void ActivateEditor()
    {
        EditorActivator = !EditorActivator;
    }

    public void vertexExtractor()
    {
        if (testingBool)
        {
            selectorMesh = Selector.GetComponent<Renderer>();
            minbound = selectorMesh.bounds.min;
            maxbound = selectorMesh.bounds.max;
            //Instantiate(sfiro, minbound, Quaternion.identity); //watch out for the position it should be transformed
            //Instantiate(sfiro, maxbound, Quaternion.identity);
        }   
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
                                _MinecraftBuilder.Instantiator(coliderPose, true);
                                //Instantiate(sfiro,)
                            }
                        }
                    }
                }
            }
        }



    }

    public void abortSelector()
    {
        Destroy(Selector);
        appBar.SetActive(false);
        doneInstantiation = false;
    }

    public void confirmSelector()
    {
        officialVoxelizer();
        Destroy(Selector);
        appBar.SetActive(false);
        doneInstantiation = false;
    }

    public void adjustSelector()
    {
        Selector.GetComponent<BoxCollider>().enabled = true;
        Selector.GetComponent<BoundsControl>().enabled = true;
    }

    public void doneSelector()
    {
        Selector.GetComponent<BoxCollider>().enabled = false;
        Selector.GetComponent<BoundsControl>().enabled = false;
    }

    public void request1()
    {
        Prism = Prism1;
        _meshCollider = Prism1.GetComponent<MeshCollider>();
    }
    
    public void request2()
    {
        Prism = Prism2;
        _meshCollider = Prism2.GetComponent<MeshCollider>();
    }
    
    public void request3()
    {
        Prism = Prism3;
        _meshCollider = Prism3.GetComponent<MeshCollider>();
    }
}