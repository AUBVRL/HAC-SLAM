using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;

public class PixelEditor : MonoBehaviour, IMixedRealityPointerHandler
{
    [System.NonSerialized]
    public Vector3 pointerPosition;
    Drawner2 badnaKlek;
    void Start()
    {
        badnaKlek = gameObject.GetComponent<Drawner2>();
    }
    public void OnPointerClicked(MixedRealityPointerEventData eventData)
    {
        if (eventData.Pointer is ShellHandRayPointer shellHandRayPointer)
        {
            // Get the position of the shellhandraypointer when clicked
            //Vector3 pointerPosition = shellHandRayPointer.Position;
            pointerPosition = shellHandRayPointer.BaseCursor.Position;

            // Do something with the pointer position
            //Debug.Log("ShellHandRayPointer position: " + pointerPosition);
            badnaKlek.klek();
        }
    }

    // Implement the remaining interface methods
    public void OnPointerDown(MixedRealityPointerEventData eventData) { }
    public void OnPointerDragged(MixedRealityPointerEventData eventData) { }
    public void OnPointerUp(MixedRealityPointerEventData eventData) { }
}
