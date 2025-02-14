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
        imageTarget.transform.right = -ImageTarget.transform.right;
        imageTarget.transform.forward = ImageTarget.transform.up;
        Vector3 transformedPosition = new Vector3(imageTarget.transform.position.x, imageTarget.transform.position.z, imageTarget.transform.position.y);
        CreateDebugCube(imageTarget.transform.position + imageTarget.transform.forward * 1.0f, Color.blue);  // Forward (Z)
        CreateDebugCube(imageTarget.transform.position + imageTarget.transform.up * 1.0f, Color.green);      // Up (Y)
        CreateDebugCube(imageTarget.transform.position + imageTarget.transform.right * 1.0f, Color.red);     // Right (X)
        GameObject tooltip = Instantiate(tooltipPrefab, imageTarget.transform.position + imageTarget.transform.up * 0.2f, Quaternion.identity);
        ToolTip tooltipText = tooltip.GetComponent<ToolTip>();
        tooltipText.ToolTipText = "World Coordinates: "  + transformedPosition.ToString() + "\n" +
                          "World Rotation: " + (-imageTarget.transform.rotation.eulerAngles.y).ToString();
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
        CreateDebugCube(robot.transform.position + robot.transform.forward * 1.0f, Color.blue);  // Forward (Z)
        CreateDebugCube(robot.transform.position + robot.transform.up * 1.0f, Color.green);      // Up (Y)
        CreateDebugCube(robot.transform.position + robot.transform.right * 1.0f, Color.red);     // Right (X)
        Vector3 globalPosition = robot.transform.position;
        Vector3 localPosition = imageTarget.transform.InverseTransformPoint(globalPosition);
        Vector3 transformedGlobalPosition = new Vector3(globalPosition.x, globalPosition.z,globalPosition.y);
        float y_angle = robot.transform.eulerAngles.y - imageTarget.transform.eulerAngles.y;
        GameObject tooltip = Instantiate(tooltipPrefab, robot.transform.position + Vector3.up * 0.4f, Quaternion.identity);
        ToolTip tooltipText = tooltip.GetComponent<ToolTip>();
        tooltipText.ToolTipText = "Coordinates wrt Image: " + localPosition.ToString() + "\n" +
                  "Rotation wrt Image: " + (-y_angle).ToString() + "\n" +
                  "World Coordinates: " + transformedGlobalPosition.ToString() + "\n" +
                  "World Rotation: " + (-robot.transform.eulerAngles.y).ToString();  
    }
}
