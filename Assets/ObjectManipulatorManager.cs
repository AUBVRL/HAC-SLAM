using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

public class ObjectManipulatorManager: MonoBehaviour
{
    private MoveAxisConstraint moveConstraint;
    private RotationAxisConstraint rotationConstraint;
    private AxisFlags combinedAxes, combinedRotations;

    void OnEnable()
    {
        EditsManager.instantiatedObject.GetComponent<ObjectManipulator>().enabled = true;
        moveConstraint = EditsManager.instantiatedObject.GetComponent<MoveAxisConstraint>();
        rotationConstraint = EditsManager.instantiatedObject.GetComponent<RotationAxisConstraint>();
        
    }

    private void OnDisable()
    {
        EditsManager.instantiatedObject.GetComponent<ObjectManipulator>().enabled = false;
    }

    public void ToggleXAxis(bool isOn)
    {
        UpdateAxis(AxisFlags.XAxis, isOn);
    }

    public void ToggleXRotation(bool isOn)
    {
        UpdateRotation(AxisFlags.XAxis, isOn);
    }

    public void ToggleYAxis(bool isOn)
    {
        UpdateAxis(AxisFlags.YAxis, isOn);
    }

    public void ToggleYRotation(bool isOn)
    {
        UpdateRotation(AxisFlags.YAxis, isOn);
    }

    public void ToggleZAxis(bool isOn)
    {
        UpdateAxis(AxisFlags.ZAxis, isOn);
    }

    public void ToggleZRotation(bool isOn)
    {
        UpdateRotation(AxisFlags.ZAxis, isOn);
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

    private void UpdateRotation(AxisFlags axis, bool isOn)
    {
        if (isOn)
        {
            // Add the axis to the combined axes
            combinedRotations |= axis;
        }
        else
        {
            // Remove the axis from the combined axes
            combinedRotations &= ~axis;
        }

        rotationConstraint.ConstraintOnRotation = combinedRotations;

    }

    public void SetMovementConstraint(bool state)
    {
        moveConstraint.enabled = state;
    }

    public void SetLocalTranslation(bool state)
    {
        moveConstraint.UseLocalSpaceForConstraint = state;
    }

    public void SetLocalRotation(bool state)
    {
        rotationConstraint.UseLocalSpaceForConstraint = state;
    }


    public void DisableMovement()
    {
        combinedAxes |= AxisFlags.XAxis;
        combinedAxes |= AxisFlags.YAxis;
        combinedAxes |= AxisFlags.ZAxis;
        moveConstraint.ConstraintOnMovement = combinedAxes;
    }

    public void DisableRotation()
    {
        combinedRotations |= AxisFlags.XAxis;
        combinedRotations |= AxisFlags.YAxis;
        combinedRotations |= AxisFlags.ZAxis;
        rotationConstraint.ConstraintOnRotation = combinedRotations;
    }
}
