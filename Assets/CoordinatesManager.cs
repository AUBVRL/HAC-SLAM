using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoordinatesManager : MonoBehaviour
{
    public GameObject ImageTarget, VectorTarget, tooltipPrefab, vuforiaParent;
    public static GameObject imageTarget;
    public static GameObject originTarget;
    public static GameObject vectorTarget;
    void Start()
    {
        originTarget = new GameObject("originTarget");
        vectorTarget = new GameObject("vectorTarget");
        imageTarget = new GameObject("correctAxesImageTarget");
    }

    private void CreateDebugCube(Vector3 position, Color color)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = position;
        cube.transform.localScale = Vector3.one * 0.05f; // Set size to make it more visible
        cube.GetComponent<Renderer>().material.color = color;
    }

    public void displayImageTargetCoordinates()
    {
        //imageTarget.transform.position = ImageTarget.transform.position;
        //imageTarget.transform.rotation = ImageTarget.transform.rotation;
        //imageTarget.transform.up = ImageTarget.transform.forward;
        //imageTarget.transform.right = -ImageTarget.transform.right;
        //imageTarget.transform.forward = ImageTarget.transform.up;
        //Vector3 transformedPosition = new Vector3(imageTarget.transform.position.x, imageTarget.transform.position.z, imageTarget.transform.position.y);
        CreateDebugCube(imageTarget.transform.position + imageTarget.transform.forward * 0.1f, Color.blue);  // Forward (Z)
        CreateDebugCube(imageTarget.transform.position + imageTarget.transform.up * 0.1f, Color.green);      // Up (Y)
        CreateDebugCube(imageTarget.transform.position + imageTarget.transform.right * 0.1f, Color.red);     // Right (X)
        //GameObject tooltip = Instantiate(tooltipPrefab, imageTarget.transform.position + imageTarget.transform.up * 0.2f, Quaternion.identity);
        //ToolTip tooltipText = tooltip.GetComponent<ToolTip>();
        //tooltipText.ToolTipText = "World Coordinates: " + transformedPosition.ToString() + "\n" +
        //                  "World Rotation: " + (-imageTarget.transform.rotation.eulerAngles.y).ToString();
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
        CreateDebugCube(robot.transform.position + robot.transform.forward * 0.1f, Color.blue);  // Forward (Z)
        CreateDebugCube(robot.transform.position + robot.transform.up * 0.1f, Color.green);      // Up (Y)
        CreateDebugCube(robot.transform.position + robot.transform.right * 0.1f, Color.red);     // Right (X)
        Vector3 globalPosition = robot.transform.position;
        Vector3 localPosition = imageTarget.transform.InverseTransformPoint(globalPosition);
        float y_angle = robot.transform.eulerAngles.y - imageTarget.transform.eulerAngles.y;
        Vector3 transformedGlobalPosition = new Vector3(localPosition.x, localPosition.z, -y_angle);
        GameObject tooltip = Instantiate(tooltipPrefab, robot.transform.position + Vector3.up * 0.4f, Quaternion.identity);
        ToolTip tooltipText = tooltip.GetComponent<ToolTip>();
        tooltipText.ToolTipText = "Coordinates (x, y, theta): " + transformedGlobalPosition.ToString(); //+ "\n" +
                  //"Rotation wrt Image: " + (-y_angle).ToString();
    }

    public void SaveOriginImageTarget()
    {
        originTarget.transform.position = ImageTarget.transform.position;
    }

    public void SaveVectorImageTarget()
    {
        vectorTarget.transform.position = VectorTarget.transform.position;
        Vector3 z_axis = vectorTarget.transform.position - originTarget.transform.position;
        z_axis.Normalize();
        imageTarget.transform.position = originTarget.transform.position;
        imageTarget.transform.forward = z_axis;
        displayImageTargetCoordinates();
    }
}
