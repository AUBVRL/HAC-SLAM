using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Experimental.UI;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.XR;

public class EditsManager : MonoBehaviour
{
    public static event Action OnObjectInstantiated;

    // Distance to instantiate the prefab in front of the camera
    public float distanceFromCamera = 0.5f;

    public static GameObject instantiatedObject;

    bool doneInstantiaion = false;
    bool additionSelected, deletionSelected, labelingSelected;

    Vector3 initialWorldPosition;

    int UILayerMask = 1 << 6;

    void Update()
    {
        //HandleTouchInput();
        HandleHandGestureInput();
    }

    void HandleTouchInput()
    {
        if (Input.touchCount > 0 && !doneInstantiaion)
        {
            Touch touch = Input.GetTouch(0);

            Ray ray = Camera.main.ScreenPointToRay(touch.position);
            RaycastHit hit;
            bool checkUI = Physics.Raycast(ray, out hit, 1, UILayerMask);

            if (touch.phase == TouchPhase.Began && !checkUI)
            {
                Vector3 spawnPosition = GetSpawnPositionFromTouch(touch.position);
                StartStretching(spawnPosition);
            }

            if (touch.phase == TouchPhase.Moved && instantiatedObject != null)
            {
                Vector3 currentWorldPosition = GetCurrentWorldPositionFromTouch(touch.position);
                StretchObject(currentWorldPosition);
            }

            if (touch.phase == TouchPhase.Ended && instantiatedObject != null)
            {
                //instantiatedObject = null;

                doneInstantiaion = true;
                OnObjectInstantiated?.Invoke();
            }

        }
    }
    
    void HandleHandGestureInput()
    {
        if (!doneInstantiaion)
        {
            if (HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, Handedness.Right, out MixedRealityPose poseLeftIndex))
            {
                HandJointUtils.TryGetJointPose(TrackedHandJoint.ThumbTip, Handedness.Right, out MixedRealityPose poseLeftThumb);
                bool fingersClosed = Vector3.Distance(poseLeftIndex.Position, poseLeftThumb.Position) < 0.04f;
                if (instantiatedObject != null)
                {
                    if (fingersClosed) //equivelant to touch phase moved
                    {
                        //Vector3 currentWorldPosition = GetCurrentWorldPositionFromTouch(poseLeftIndex.Position);
                        StretchObject(poseLeftIndex.Position);
                    }
                    else   //successful instantiation process and equivelant to touch phase ended
                    {
                        doneInstantiaion = true;
                        OnObjectInstantiated?.Invoke();   
                    }
                }
                else  //here the instantation happens and equicalant to touch phase began
                {
                    if (fingersClosed)
                    {
                        Debug.Log("Instantiating");
                    //Vector3 spawnPosition = GetSpawnPositionFromTouch(poseLeftIndex.Position);
                        StartStretching(poseLeftIndex.Position);
                    }
                    
                }
            }   
        }    
    }
    // HandleMouseInput();

    void StartStretching(Vector3 spawnPosition)
    {
        instantiatedObject = Instantiate(PrefabsManager.SelectorPrefab, spawnPosition, Quaternion.identity);
        initialWorldPosition = spawnPosition;
        instantiatedObject.transform.localScale = Vector3.zero; // Reset scale to start from zero
    }

    // Function to stretch the object between the initial and current positions
    void StretchObject(Vector3 currentWorldPosition)
    {
        Vector3 cornerDifference = currentWorldPosition - initialWorldPosition;

        Vector3 newScale = new Vector3(Mathf.Abs(cornerDifference.x), Mathf.Abs(cornerDifference.y), Mathf.Abs(cornerDifference.z));
        instantiatedObject.transform.localScale = newScale;

        instantiatedObject.transform.position = initialWorldPosition + cornerDifference / 2.0f;
    }

    // Get spawn position from the touch point (use this logic for other inputs as needed)
    Vector3 GetSpawnPositionFromTouch(Vector2 screenPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        RaycastHit hit;
        Vector3 spawnPosition;

        if (Physics.Raycast(ray, out hit, distanceFromCamera))
        {
            spawnPosition = hit.point - Camera.main.transform.forward * 0.1f;
        }
        else
        {
            spawnPosition = ray.origin + (ray.direction * distanceFromCamera);
        }

        return spawnPosition;
    }

    // Get current world position based on the touch movement (use this for other inputs as well)
    Vector3 GetCurrentWorldPositionFromTouch(Vector2 screenPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        return ray.origin + (ray.direction * distanceFromCamera);
    }

    List<Vector3> VoxelizeSelector()
    {
        List<Vector3> selectorPoints = new();
        // Get the bounds of the instantiated object
        Bounds bounds = instantiatedObject.GetComponent<MeshRenderer>().bounds;
        
        Vector3Int minBounds = Vector3Int.FloorToInt(VoxelManager.RoundToVoxel(bounds.min) / PrefabsManager.voxelSize);
        Vector3Int maxBounds = Vector3Int.FloorToInt(VoxelManager.RoundToVoxel(bounds.max) / PrefabsManager.voxelSize);
        
        Vector3 voxelSizeVector = Vector3.one * PrefabsManager.voxelSize;

        for (int x = minBounds.x; x <= maxBounds.x; x ++)
        {
            for (int y = minBounds.y; y <= maxBounds.y; y ++)
            {
                for (int z = minBounds.z; z <= maxBounds.z; z ++)
                {
                    Vector3 coliderPose = new Vector3(x, y, z) * PrefabsManager.voxelSize;
                    
                    bool checkBoxOverlap = Physics.CheckBox(coliderPose, voxelSizeVector / 2, Quaternion.identity, UILayerMask);
                    
                    if (checkBoxOverlap) selectorPoints.Add(coliderPose);
                }
            }
        }

        return selectorPoints;
    }
    
    public void Confirm()
    {
        List<Vector3> selectorPoints = VoxelizeSelector();
        if (additionSelected)
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
        doneInstantiaion = false;
    }

    public void Cancel()
    {
        Destroy(instantiatedObject);
        instantiatedObject = null;
        doneInstantiaion = false;
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
}
