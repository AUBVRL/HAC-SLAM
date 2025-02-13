using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoordinatesManager : MonoBehaviour
{
    public GameObject ImageTarget, tooltipPrefab;
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
        tooltipText.ToolTipText = "World Coordinates: " + imageTarget.transform.position.ToString() + "\n" +
                          "World Rotation: " + imageTarget.transform.rotation.eulerAngles.y.ToString();
    }
}
