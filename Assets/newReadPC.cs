using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;
using UnityEngine;

public class newReadPC : MonoBehaviour
{

    // POINT CLOUD FILE PATH
    public static string _relativePointCloudFilePath = "C://Users//tharo//OneDrive//Desktop//try.CSV";

    // LIST OF POINTS
    public static List<Vector3> _pixelCoords = new List<Vector3>();
    // LIST OF COLORS
    public static List<Vector3> _pixelColors = new List<Vector3>();
    // LIST OF SPHERES
    public static List<GameObject> _spheresFromPC = new List<GameObject>();
    // INITIALIZE EMPTY ARRAYS
    public int[] arrayZeroR;
    public int[] arrayZeroG;
    public int[] arrayZeroB;
    // GRADE VOLORS BOOLEAN
    public bool gradeColors = false;

    private void Awake()
    {
        ReadLoadPointCloud();

    }

    // Start is called before the first frame update
    void Start()
    {
        // INITIALIZE EMPTY ARRAYS
        arrayZeroR = new int[_spheresFromPC.Count];
        arrayZeroG = new int[_spheresFromPC.Count];
        arrayZeroB = new int[_spheresFromPC.Count];

        Debug.Log("Sum of R is: " + arrayZeroR.Sum());
        Debug.Log("Sum of G is: " + arrayZeroG.Sum());
        Debug.Log("Sum of B is: " + arrayZeroB.Sum());
    }

    // Update is called once per frame
    void Update()
    {

        // CHECK IF GRADE COLORS IS TRUE
        if (gradeColors)
        {
            // GRADE COLORS TILL THEY BECOME BLACK ------------------------------------------------------------

            // CHECK SUMS OF ARRAYS
            int sumR = arrayZeroR.Sum();
            int sumG = arrayZeroG.Sum();
            int sumB = arrayZeroB.Sum();

            // CHECK IF WE NEED TO GRADE MORE
            if (sumR != _spheresFromPC.Count || sumG != _spheresFromPC.Count || sumB != _spheresFromPC.Count)
            {
                GradeColors();
            }
            else
            {
                Debug.Log("ALL Spheres are now Colored Black");
                gradeColors = false;
                foreach (GameObject sphere in _spheresFromPC)
                {
                    sphere.GetComponent<Renderer>().material.SetColor("_BaseColor", new Color(1f, 1f, 1f, 1f));
                }
            }
        }
    }

    private void GradeColors()
    {
        throw new NotImplementedException();
    }

    public void ReadLoadPointCloud()
    {
        // READ TXT WITH INFO and STORE into LISTS
        string[] lines = File.ReadAllLines(_relativePointCloudFilePath);

        int lineCounter = 0;
        foreach (string line in lines)
        {
            lineCounter += 1;
            if (lineCounter > 1)
            {
                //Debug.Log("TxtLine is: " + line);
                string[] arrayLine = line.Split('\t'); // 0: string xyz | 1: string RGB
                string[] arrayXYZ = arrayLine[0].Split(' ');
                //Debug.Log("arrayXYZ is: " + arrayXYZ[0] + " " + arrayXYZ[1] + " " + arrayXYZ[2]);
                string[] arrayRGB = arrayLine[1].Replace("\"", String.Empty).Split(',');
                //Debug.Log("arrayRGB is: " + arrayRGB[0] + " " + arrayRGB[1] + " " + arrayRGB[2]);
                float xCoord = float.Parse(arrayXYZ[0]);
                float yCoord = float.Parse(arrayXYZ[1]);
                float zCoord = float.Parse(arrayXYZ[2]);
                float rColor = float.Parse(arrayRGB[0]);
                float gColor = float.Parse(arrayRGB[1]);
                float bColor = float.Parse(arrayRGB[2]);
                Vector3 lineCoord = new Vector3(xCoord, zCoord, yCoord);
                Vector3 lineColor = new Vector3(rColor, gColor, bColor);

                // Append Items to List
                _pixelCoords.Add(lineCoord);
                _pixelColors.Add(lineColor);

                // Draw, Append, and Color Spheres
                GameObject sphereFromPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphereFromPoint.transform.position = lineCoord;
                sphereFromPoint.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                sphereFromPoint.GetComponent<Renderer>().material.SetColor("_BaseColor", new Color(rColor / 255f, gColor / 255f, bColor / 255f, 1f));
                sphereFromPoint.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                sphereFromPoint.GetComponent<Renderer>().lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
                // Append Sphere to List
                _spheresFromPC.Add(sphereFromPoint);
            }
        }

        Debug.Log("Length of Coords is: " + _pixelCoords.Count);
        Debug.Log("Length of Colors is: " + _pixelColors.Count);
    }

}

