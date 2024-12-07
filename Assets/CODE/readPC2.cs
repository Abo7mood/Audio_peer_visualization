using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.IO;


public class readPC2 : MonoBehaviour
{

     // Load FILE PATH
    //private string relativeFilePath = "Assets/Pcx/InfoPC/txtLighterPC1.csv";
    private string relativeFilePath = "Assets/TXT/123.txt";
    //DEFINE LISTS OF COORDINATES
    private List<Vector3> _pointcoords = new List<Vector3>();
     // LIST OF COLORS
     private List<Vector3> _pointcolors= new List<Vector3>();
     //LIST OF SPHERES
     private List<GameObject> _spheresFromPC = new List<GameObject>();

     private void Awake()
     {
          ReadPointCloudData();
     }

     // Start is called before the first frame update
     void Start()
     {
      
     }

     // Update is called once per frame
     void Update()
     {
        
     }

     public void ReadPointCloudData()
     {
          // READ TEXT WITH INFO and STORE into ARRAY and LISTS
          string[] lines = File.ReadAllLines(relativeFilePath);

          int counter = 0;
          foreach (string line in lines)
          {
               counter += 1;
               if (counter > 1)
               {
                    // Array with XYZ and RGB  
                    string[] xyzRGB = line.Split('\n');

                    // get XYZ string
                    string XYZ = xyzRGB[0];
                    // get RGB string
                    //string RGB = xyzRGB[1];
                    Console.WriteLine(XYZ);

                    // split XYZ and make them FLOATS and VECTOR3
                    string[] individualxyz = XYZ.Split('\t');
                //string[] individualRGB = RGB.Replace("\"", String.Empty).Split(',');
                //string[] individualRGB = RGB.Replace("\"", String.Empty).Split(',');
                    float x = float.Parse(individualxyz[0]);
                    float y = float.Parse(individualxyz[1]);
                    float z = float.Parse(individualxyz[2]);
                // remove below three lines if you don't need RGB
                    /*float r = float.Parse(individualxyz[3]);
                    float g = float.Parse(individualxyz[4]);
                    float b = float.Parse(individualxyz[5]);*/
                //
                /*float r = float.Parse(individualRGB[0]);
                float g = float.Parse(individualRGB[1]);
                float b = float.Parse(individualRGB[2]);*/

                // COMPOSE VECTORS
                Vector3 _pointcoord = new Vector3(x,y,z);
                //remove below three lines if you don't need RGB
                //Vector3 _pointcolor = new Vector3(r,g,b);
                //Color _pointRealColor = new Color(r/255, g/255, b/255, 1f);

                    // ADD COORDINATE AND COLOR TO THEIR RESPECTIVE LISTS
                    _pointcoords.Add(_pointcoord);
                //remove below three lines if you don't need RGB
                    //_pointcolors.Add(_pointcolor); //need for RGB

                    // CREATE SPHERES
                    GameObject sphereFromPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    sphereFromPoint.transform.position = _pointcoord;
                    sphereFromPoint.transform.localScale = new Vector3(0.5f,0.5f,0.5f);
                //remove below three lines if you don't need RGB
                //sphereFromPoint.GetComponent<Renderer>().material.color = _pointRealColor; //need for RGB
                    //sphereFromPoint.GetComponent<Renderer>().material.SetColor("_BaseColor", _pointRealColor);
                    //sphereFromPoint.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Renderer.ShadowCastingMode.Off;
                    //sphereFromPoint.GetComponent<Renderer>().LightProbeUsage = UnityEngine.Renderer.LightProbeUsage.Off;

                    // ADD SPHERES INTO LIST
                    _spheresFromPC.Add(sphereFromPoint);

               }
          }

          Debug.Log("The number of points is: " + _pointcoords.Count);
     
    }
}
