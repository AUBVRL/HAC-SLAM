using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drawner3 : MonoBehaviour
{
    // Start is called before the first frame update
    Texture2D textureMerger;
    public RosSubscriberExample subsub;
    sbyte[] mapp;
    float widt;
    float heigh;
    float resool;
    public GameObject cam;
    Color pixelColor;
    void Start()
    {
        widt = subsub.wid;
        //Debug.Log(widt);
        heigh = subsub.heig;
        resool = subsub.resol;
        textureMerger = new Texture2D((int)widt, (int)heigh, TextureFormat.RGBA32, false);
        //textureMerger = new Texture2D(100, 100);
        textureMerger.filterMode = FilterMode.Point;
        mapp = subsub.arr;
        GetComponent<Renderer>().material.mainTexture = textureMerger;
        for (int y = 0; y < textureMerger.height; y++)
        {
            for (int x = 0; x < textureMerger.width; x++)
            {
                if (x == 1 && y == 1)// || y == 2))
                {
                    textureMerger.SetPixel(x, y, Color.white);
                    //map[x * Matrix_Size + y] = 100;
                }
                else
                {
                    textureMerger.SetPixel(x, y, Color.black);
                    //map[x * Matrix_Size + y] = 0;
                }
            }
        }
        textureMerger.Apply();
    }

    // Update is called once per frame
    void Update()
    {
        mapp = subsub.arr;
        widt = subsub.wid;
        heigh = subsub.heig;
        resool = subsub.resol;
        for (int i = 0; i < mapp.Length; i++)
        {
            int x = i % (int)widt;
            int y = i / (int)widt;

            sbyte occupancyValue = mapp[i];

            

            if (occupancyValue == -1)
            {
                // Unknown cell
                pixelColor = Color.gray;
            }
            else if (occupancyValue == 0)
            {
                // Free cell
                pixelColor = Color.white;
            }
            else if (occupancyValue == 100)
            {
                // Occupied cell
                pixelColor = Color.black;
            }

            // Set the pixel color in the Texture2D
            textureMerger.SetPixel(x, y, pixelColor);
        }
        textureMerger.Apply();
        //Debug.Log(cam.transform.position.x);
    }
}
