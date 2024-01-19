using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drawner2 : MonoBehaviour
{
    //from declarations script
    float Plane_Width = 20f;
    static int Matrix_Size = 196;//196;//1000
    //int Map_Size = 36864;//9216;
    float Cell_Size = 0.104f;//0.104f; //0.02f
    //from drawner
    Texture2D texture;
    [System.NonSerialized]
    public Texture2D temptexture;
    float temp = 0.5f;
    float[] Occupancy_array;
    int ind = 0;
    Color Black_Color = Color.black, White_Color = Color.white;
    //from HL_map:
    static int Hor_angle_window = 36;
    static int Ver_angle_window = 16;
    static float angle_size = 2f;
    float Hor_angle_min = -((float)Hor_angle_window / 2);
    float Ver_angle_min = -((float)Ver_angle_window / 2);
    Vector3 Hor_Ray_direction;
    Vector3 Ver_Ray_direction;
    Vector3 nearest_pt;
    float hit_distance;
    float min_hit;
    Vector3[] pt = new Vector3[(int)(Hor_angle_window / angle_size)];
    static int size = (int)(Hor_angle_window / angle_size);
    //from drawner.collaboraticeslam
    float[] X = new float[size];
    float[] Y = new float[size];
    float[] Z = new float[size];

    float[] dist_X = new float[size];
    float[] dist_Z = new float[size];
    float[] dist_Y = new float[size];

    int[] X_Coor = new int[size];
    int[] Y_Coor = new int[size];

    float BF_x;
    float BF_y;
    float BF_z;
    float newbfx;
    float newbfy;
    float newbfz;
    float deltax;
    float deltaz;
    int intdeltax;
    int intdeltaz;
    public GameObject marker;
    //new
    [System.NonSerialized]
    public sbyte[] map = new sbyte[Matrix_Size * Matrix_Size];
    PixelEditor pixi;
    
    //public Drawner3 Mergedscript;
    //Texture2D mergedmap = Mergedscript.textureMerger;

    void Start()
    {
        //From Drawner
        BF_x = gameObject.transform.position.x - (Mathf.Abs(gameObject.transform.localScale.x * (10 / 2)));
        BF_y = gameObject.transform.position.y;
        BF_z = gameObject.transform.position.z - (Mathf.Abs(gameObject.transform.localScale.z * (10 / 2)));

        Vector3 BaseFrame = new Vector3(BF_x, BF_y, BF_z);
        Occupancy_array = new float[Matrix_Size * Matrix_Size];
        texture = new Texture2D(Matrix_Size, Matrix_Size,TextureFormat.RGBA32,false);
        temptexture = new Texture2D(Matrix_Size, Matrix_Size);
        texture.filterMode = FilterMode.Point;
        temptexture.filterMode = FilterMode.Point;

        //GetComponent<Renderer>().material.mainTexture = Mergedscript.textureMerger;
        GetComponent<Renderer>().material.mainTexture = texture;
        //Debug.Log(texture.GetPixel(1, 1));
        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                if (x == 1 && y == 1)// || y == 2))
                {
                    texture.SetPixel(x, y, White_Color);
                    temptexture.SetPixel(x, y, White_Color);
                    //map[y * Matrix_Size + x] = 100;
                    map[x * Matrix_Size + (Matrix_Size - y - 1)] = 100;
                }
                else
                {
                    texture.SetPixel(x, y, Black_Color);
                    temptexture.SetPixel(x, y, Black_Color);
                    //map[y * Matrix_Size + x] = 0;
                    map[x * Matrix_Size + (Matrix_Size - y - 1)] = 0;
                }
            }
        }
        //mergedmap.Apply();
        texture.Apply();
        temptexture.Apply();
        //Debug.Log(texture.GetPixel(1, 1));
        pixi = gameObject.GetComponent<PixelEditor>();
        

    }

    void Update()
    {
        //Debug.Log(marker.transform.position.x);
        //From HL_map
        Vector3 Gaze_direction = Camera.main.transform.forward;
        Vector3 Gaze_position = Camera.main.transform.position;

        //Base frame for plane on its corner
        

        //Raycasting from camera and saving results in "pt" array variable
        for (int i = 0; i < (Hor_angle_window / angle_size); i++)
        {
            min_hit = 500;
            nearest_pt = new Vector3(-1, -1, -1);
            pt[i] = new Vector3(-1, -1, -1);
            Hor_Ray_direction = Quaternion.Euler(0, (Hor_angle_min + (angle_size * i)), 0) * Gaze_direction;
            for (int j = 0; j < (int)(Ver_angle_window / angle_size); j++)
            {
                RaycastHit hit;
                bool raycastHit = false;
                Ver_Ray_direction = Quaternion.Euler((Ver_angle_min + (angle_size * j)), 0, 0) * Hor_Ray_direction;
                raycastHit = Physics.Raycast(Gaze_position, Ver_Ray_direction, out hit, 2f);
                if (raycastHit)
                {
                    //Debug.Log(hit.collider.name);

                    hit_distance = Mathf.Sqrt(Mathf.Pow((Gaze_position.x - hit.point.x), 2) + Mathf.Pow((Gaze_position.z - hit.point.z), 2));
                    if ((hit_distance < min_hit))
                    {
                        min_hit = hit_distance;
                        nearest_pt = hit.point;
                    }
                }
            }
            pt[i] = nearest_pt;
        }

        //From Drawner.CollaboraticeSLAM
        for (int i = 0; i < size; i++)
        {
            //position of detected points in "pt" variable
            X[i] = pt[i].x;
            Y[i] = pt[i].y;
            Z[i] = pt[i].z;

            if (X[i] == -1 && Z[i] == -1) { }
            else
            {
                //Calculate distance from base frame to this detected point and round it the cells length
                dist_X[i] = Mathf.Abs(X[i] - BF_x); //pose wrt to BF
                dist_X[i] = (Mathf.Ceil(dist_X[i] / Cell_Size) * Cell_Size); //round to cell's boundary 

                dist_Z[i] = Mathf.Abs(Z[i] - BF_z);
                dist_Z[i] = (Mathf.Ceil(dist_Z[i] / Cell_Size) * Cell_Size);

                // Final Cursor , convert from distances to matrix cells ----- This is the Pixel I'm interested in!
                //Convert from cell distacence to cell number w.r.t the texture base (Which is here diagonally opposite to the base frame)
                X_Coor[i] = Matrix_Size - 1 - (Mathf.RoundToInt((dist_X[i] * (Matrix_Size / Plane_Width)) - 1)); //the minus ones are redundant
                Y_Coor[i] = Matrix_Size - 1 - (Mathf.RoundToInt((dist_Z[i] * (Matrix_Size / Plane_Width)) - 1));
                ind = (texture.height - 1 - X_Coor[i]) * texture.height + Y_Coor[i];
                temp = Occupancy_array[ind];
                if (Y[i] >= (BF_y - 0.2) && Y[i] <= (BF_y + 0.2))
                {
                    //if obstacle is the ground, Set cell as free                    
                    if (Occupancy_array[ind] > 0)
                        Occupancy_array[ind] = temp - 0.01f;

                }
                else
                {
                    if (Occupancy_array[ind] < 1)
                        Occupancy_array[ind] = temp + 0.05f;

                }

                if (Occupancy_array[ind] >= 0.6)
                {
                    texture.SetPixel(X_Coor[i], Y_Coor[i], White_Color);
                    temptexture.SetPixel(X_Coor[i], Y_Coor[i], White_Color);
                    //map[Y_Coor[i] * Matrix_Size + X_Coor[i]] = 100;
                    map[X_Coor[i] * Matrix_Size + Matrix_Size - 1 - Y_Coor[i]] = 100;
                }
                else
                {
                    texture.SetPixel(X_Coor[i], Y_Coor[i], Black_Color);
                    temptexture.SetPixel(X_Coor[i], Y_Coor[i], Black_Color);
                    //map[Y_Coor[i] * Matrix_Size + X_Coor[i]] = 0;
                    map[X_Coor[i] * Matrix_Size + Matrix_Size - 1 - Y_Coor[i]] = 0;
                }

            }

        }
       //mergedmap.Apply();
        texture.Apply();
        temptexture.Apply();
        if ((gameObject.transform.position.x - 10) != BF_x || (gameObject.transform.position.z) - 10 != BF_z)
        {
            PixelShifter();
            //Debug.Log("Nope= "+ gameObject.transform.position.x + "is not equal to " + BF_x);
        }
    }

    public void PixelShifter()
    {
        
        newbfx = gameObject.transform.position.x - (Mathf.Abs(gameObject.transform.localScale.x * (10 / 2)));
        newbfy = gameObject.transform.position.y;
        newbfz = gameObject.transform.position.z - (Mathf.Abs(gameObject.transform.localScale.z * (10 / 2)));
        deltax = newbfx - BF_x;
        deltax = (Mathf.RoundToInt(deltax / Cell_Size) * Cell_Size);
        deltaz = newbfz - BF_z;
        deltaz = (Mathf.RoundToInt(deltaz / Cell_Size) * Cell_Size);
        intdeltax = Mathf.Abs(Mathf.RoundToInt(deltax * Matrix_Size / Plane_Width));
        intdeltaz = Mathf.Abs(Mathf.RoundToInt(deltaz * Matrix_Size / Plane_Width));
        //Debug.Log(intdeltax);
        BF_x = newbfx;
        BF_z = newbfz;
        if (deltax >= 0)
        {
            if (deltaz >= 0)
            {
                for (int x = texture.height -1 ; x >= 0; x--)
                {

                    for (int z = texture.width - 1; z >= 0; z--)
                    {
                        if (x < intdeltax || z < intdeltaz)
                        {
                            texture.SetPixel(x, z, Black_Color);
                            temptexture.SetPixel(x, z, Black_Color);

                            map[x * Matrix_Size + (Matrix_Size - z - 1)] = 0;

                        }
                        else
                        {
                            texture.SetPixel(x, z, temptexture.GetPixel(x - intdeltax, z - intdeltaz));
                            temptexture.SetPixel(x, z, temptexture.GetPixel(x - intdeltax, z - intdeltaz));

                            map[x * Matrix_Size + (Matrix_Size - z - 1)] = temptexture.GetPixel(x,z) == Color.black ? (sbyte) 0: (sbyte)100 ;
                        }
                    }
                }
            }
            else //delta z < 0
            {
                for (int x = texture.height - 1; x >= 0; x--)
                {

                    for (int z = 0; z < texture.width; z++)
                    {
                        if (x < intdeltax || z > (texture.width - intdeltaz))
                        {
                            texture.SetPixel(x, z, Black_Color);
                            temptexture.SetPixel(x, z, Black_Color);

                            map[x * Matrix_Size + (Matrix_Size - z - 1)] = 0;
                        }
                        else
                        {
                            texture.SetPixel(x, z, temptexture.GetPixel(x - intdeltax, z + intdeltaz));
                            temptexture.SetPixel(x, z, temptexture.GetPixel(x - intdeltax, z + intdeltaz));

                            map[x * Matrix_Size + (Matrix_Size - z - 1)] = temptexture.GetPixel(x, z) == Color.black ? (sbyte)0 : (sbyte)100;
                        }
                    }
                }
            }
        }
        else //delta x < 0
        {
            if (deltaz >= 0)
            {
                for (int x = 0; x < texture.height; x++)
                {

                    for (int z = texture.width - 1; z >= 0; z--)
                    {
                        if (x > (texture.height - intdeltax) || z < intdeltaz)
                        {
                            texture.SetPixel(x, z, Black_Color);
                            temptexture.SetPixel(x, z, Black_Color);

                            map[x * Matrix_Size + (Matrix_Size - z - 1)] = 0;
                        }
                        else
                        {
                            texture.SetPixel(x, z, temptexture.GetPixel(x + intdeltax, z - intdeltaz));
                            temptexture.SetPixel(x, z, temptexture.GetPixel(x + intdeltax, z - intdeltaz));

                            map[x * Matrix_Size + (Matrix_Size - z - 1)] = temptexture.GetPixel(x, z) == Color.black ? (sbyte)0 : (sbyte)100;
                        }
                    }
                }
            }
            else //delta z <0
            {
                for (int x = 0; x < texture.height; x++)
                {

                    for (int z = 0; z < texture.width; z++)
                    {
                        if (x > (texture.height - intdeltax) || z > (texture.height - intdeltaz))
                        {
                            texture.SetPixel(x, z, Black_Color);
                            temptexture.SetPixel(x, z, Black_Color);

                            map[x * Matrix_Size + (Matrix_Size - z - 1)] = 0;
                        }
                        else
                        {
                            texture.SetPixel(x, z, temptexture.GetPixel(x + intdeltax, z + intdeltaz));
                            temptexture.SetPixel(x, z, temptexture.GetPixel(x + intdeltax, z + intdeltaz));

                            map[x * Matrix_Size + (Matrix_Size - z - 1)] = temptexture.GetPixel(x, z) == Color.black ? (sbyte)0 : (sbyte)100;
                        }
                    }
                }
            }
        }
        texture.Apply();
        temptexture.Apply();
    }
    public void PlaneAligner()
    {
        //gameObject.transform.position = Camera.main.transform.position + marker.transform.position - new Vector3(0, marker.transform.position.y + Camera.main.transform.position.y + 1.5f, 0);
        gameObject.transform.position = marker.transform.position - new Vector3(0, marker.transform.position.y + 1.5f, 0);
    }
    
    public void klek()
    {
        Vector3 cursorPos = pixi.pointerPosition;

        int changedx = Matrix_Size - Mathf.RoundToInt((Mathf.Ceil(Mathf.Abs(cursorPos.x - BF_x) / Cell_Size) * Cell_Size) * (Matrix_Size / Plane_Width));
        int changedz = Matrix_Size - Mathf.RoundToInt((Mathf.Ceil(Mathf.Abs(cursorPos.z - BF_z) / Cell_Size) * Cell_Size) * (Matrix_Size / Plane_Width));

        if(texture.GetPixel(changedx,changedz) == Color.white)
        {
            texture.SetPixel(changedx, changedz, Color.black);
            temptexture.SetPixel(changedx, changedz, Color.black);
            map[changedx * Matrix_Size + (Matrix_Size - changedz - 1)] = 0;
        }
        else
        {
            texture.SetPixel(changedx, changedz, Color.white);
            temptexture.SetPixel(changedx, changedz, Color.white);
            map[changedx * Matrix_Size + (Matrix_Size - changedz - 1)] = 100;
        }
        texture.Apply();
        temptexture.Apply();
        //PixelShifter();
        Debug.Log("YAs Salam");
    }
}
