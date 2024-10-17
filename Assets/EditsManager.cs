using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Experimental.UI;
using UnityEngine;
using UnityEngine.Networking;

public class EditsManager : MonoBehaviour
{
    public static event Action OnObjectInstantiated;

    // Distance to instantiate the prefab in front of the camera
    public float distanceFromCamera = 0.5f;

    GameObject instantiatedObject;

    bool doneInstantiaion = false;

    Vector3 initialWorldPosition;

    int UILayerMask;
    // Start is called before the first frame update
    void Start()
    {
        UILayerMask = 1 << 6;
    }

    // Update is called once per frame
    void Update()
    {
        HandleTouchInput();
    }

    public void HandleTouchInput()
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
                OnObjectInstantiated?.Invoke(); //The problem is that this is being triggered when I select confirm or cancel
                Debug.Log("Instantiated");
            }

        }
    }
   
    // Add other input handling functions later:
    // HandleHandGestureInput();
    // HandleMouseInput();

    public void StartStretching(Vector3 spawnPosition)
    {
        instantiatedObject = Instantiate(SelectorManager.SelectorPrefab, spawnPosition, Quaternion.identity);
        initialWorldPosition = spawnPosition;
        instantiatedObject.transform.localScale = Vector3.zero; // Reset scale to start from zero
    }

    // Function to stretch the object between the initial and current positions
    public void StretchObject(Vector3 currentWorldPosition)
    {
        Vector3 cornerDifference = currentWorldPosition - initialWorldPosition;

        Vector3 newScale = new Vector3(Mathf.Abs(cornerDifference.x), Mathf.Abs(cornerDifference.y), Mathf.Abs(cornerDifference.z));
        instantiatedObject.transform.localScale = newScale;

        instantiatedObject.transform.position = initialWorldPosition + cornerDifference / 2.0f;
    }

    // Get spawn position from the touch point (use this logic for other inputs as needed)
    public Vector3 GetSpawnPositionFromTouch(Vector2 screenPosition)
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
    public Vector3 GetCurrentWorldPositionFromTouch(Vector2 screenPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        return ray.origin + (ray.direction * distanceFromCamera);
    }

    public void Confirm()
    {
        VoxelizeSelector();
        Destroy(instantiatedObject);
        instantiatedObject = null;
        if (instantiatedObject == null)
        {
            Debug.Log("Destroyed");
        } 
        else
        {
            Debug.Log("Festroyed");
        }
        doneInstantiaion = false;
    }

    public void Cancel()
    {
        Destroy(instantiatedObject);
        instantiatedObject = null;
        if (instantiatedObject != null)
        {
            Debug.Log("Destroyed");
        }else
        {
            Debug.Log("Festroyed");
        }
        doneInstantiaion = false;
    }

    void VoxelizeSelector()
    {
        // Get the bounds of the instantiated object
        Bounds bounds = instantiatedObject.GetComponent<MeshRenderer>().bounds;

        // Get the size of the bounds
        Vector3 size = bounds.size;

        

    }
}
