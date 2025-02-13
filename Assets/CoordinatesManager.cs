using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoordinatesManager : MonoBehaviour
{
    public GameObject ImageTarget, tooltipPrefab, vuforiaParent;
    public static GameObject imageTarget;
    void Start()
    {
        imageTarget = new GameObject("correctAxesImageTarget");
    }

    private void CreateDebugCube(Vector3 position, Color color)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = position;
        cube.transform.localScale = Vector3.one * 0.2f; // Set size to make it more visible
        cube.GetComponent<Renderer>().material.color = color;
    }

    public void displayImageTargetCoordinates()
    {
        imageTarget.transform.position = ImageTarget.transform.position;
        imageTarget.transform.up = ImageTarget.transform.forward;
        imageTarget.transform.right = ImageTarget.transform.right;
        imageTarget.transform.forward = -ImageTarget.transform.up;
        CreateDebugCube(imageTarget.transform.position + imageTarget.transform.forward * 1.0f, Color.blue);  // Forward (Z)
        CreateDebugCube(imageTarget.transform.position + imageTarget.transform.up * 1.0f, Color.green);      // Up (Y)
        CreateDebugCube(imageTarget.transform.position + imageTarget.transform.right * 1.0f, Color.red);     // Right (X)
        GameObject tooltip = Instantiate(tooltipPrefab,imageTarget.transform.position + imageTarget.transform.up * 0.2f, Quaternion.identity);
        ToolTip tooltipText = tooltip.GetComponent<ToolTip>();
        tooltipText.ToolTipText = "World Coordinates: (" + (imageTarget.transform.position.x) + ","+ (imageTarget.transform.position.z) + "," + (imageTarget.transform.position.y) + ")" + "\n" +
                          "World Rotation: " + (180-imageTarget.transform.rotation.eulerAngles.y).ToString();
    }

    public void displayActiveModelTargetCoordinates()
    {
        foreach (Transform child in vuforiaParent.transform)
        {
            if (child.gameObject.activeSelf)
            {
                displayModelTargetCoordinates(child.gameObject);
                break;
            }
        }
    }

    public void displayModelTargetCoordinates(GameObject robot)
    {
        Vector3 globalPosition = robot.transform.position;
        Vector3 localPosition = imageTarget.transform.InverseTransformPoint(globalPosition);
        float y_angle = imageTarget.transform.eulerAngles.y - robot.transform.eulerAngles.y;
        GameObject tooltip = Instantiate(tooltipPrefab, robot.transform.position + Vector3.up * 0.2f, Quaternion.identity);
        ToolTip tooltipText = tooltip.GetComponent<ToolTip>();
        tooltipText.ToolTipText = "Coordinates wrt Image Target: (" + (-localPosition.x) + ","+ (-localPosition.z) + "," + (localPosition.y) + ")" + "\n" + //localPosition.ToString() + "\n" +
                          "Rotation wrt Image Target: " + (y_angle - 180).ToString() + "\n" +
                          "World Coordinates: (" + (globalPosition.x) + ","+ (globalPosition.z) + "," + (globalPosition.y) + ")" + "\n" +
                          "World Rotation: " + (robot.transform.rotation.eulerAngles.y);
    }
}
