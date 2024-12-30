using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

public class ObjectManipulatorManager: MonoBehaviour
{
    private MoveAxisConstraint moveConstraint;
    private AxisFlags combinedAxes;

    void OnEnable()
    {
        EditsManager.instantiatedObject.GetComponent<ObjectManipulator>().enabled = true;
        moveConstraint = EditsManager.instantiatedObject.GetComponent<MoveAxisConstraint>();
    }

    private void OnDisable()
    {
        EditsManager.instantiatedObject.GetComponent<ObjectManipulator>().enabled = false;
    }

    public void ToggleXAxis(bool isOn)
    {
        UpdateAxis(AxisFlags.XAxis, isOn);
    }

    public void ToggleYAxis(bool isOn)
    {
        UpdateAxis(AxisFlags.YAxis, isOn);
    }

    public void ToggleZAxis(bool isOn)
    {
        UpdateAxis(AxisFlags.ZAxis, isOn);
    }

    private void UpdateAxis(AxisFlags axis, bool isOn)
    {
        if (isOn)
        {
            // Add the axis to the combined axes
            combinedAxes |= axis;
        }
        else
        {
            // Remove the axis from the combined axes
            combinedAxes &= ~axis;
        }

        moveConstraint.ConstraintOnMovement = combinedAxes;
        
    }

    public void SetMovementConstraint(bool state)
    {
        moveConstraint.enabled = state;
    }

    public void SetLocalSpace(bool state)
    {
        moveConstraint.UseLocalSpaceForConstraint = state;
    }
}
