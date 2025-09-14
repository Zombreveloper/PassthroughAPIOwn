using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml.Linq;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class SVGCircleReader : MonoBehaviour
{
    public GameObject CircleAsset;
    //public Texture2D CircleAsset;

    private XDocument originalSVG;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "ishiharaBlank.svg");
        if (!File.Exists(path))
        {
            Debug.LogError("No file found at: " + path);
            return;
        }

        originalSVG = XDocument.Load(path);


        List<CircleData> allCircles = ReadValues();
        CreateGO(allCircles);

        
        
        /*foreach (var el in allCircles)
        {
            Debug.Log("Kreis ist in Position: " + el.Position);
        }*/

    }

    // Update is called once per frame
    void Update()
    {
    }

    void CreateGO(List<CircleData> dat)
    {
        foreach (var el in dat)
        {
            GameObject go = Instantiate(CircleAsset, el.Position, Quaternion.identity, this.transform);
            go.transform.localScale = Vector3.one * el.Radius * 2f; // Durchmesser statt Radius
        }
        
    }

    List<CircleData> ReadValues()
    {
        List<CircleData> myCircles = new List<CircleData>();

        foreach (var elem in originalSVG.Descendants("{http://www.w3.org/2000/svg}circle"))
        {
            var x = float.Parse(elem.Attribute("cx").Value, CultureInfo.InvariantCulture);
            var y = float.Parse(elem.Attribute("cy").Value, CultureInfo.InvariantCulture);
            var r = float.Parse(elem.Attribute("r").Value, CultureInfo.InvariantCulture);
            //Debug.Log("this circle is in Xpos: " + x);

            // Das Internet sagt, SVGs positive Y-Werte gehen nach unten und Unitys Y nach oben. Darum invertiert
            var cPos = new Vector3(x, -y, 0);
            CircleData currentCircle = new CircleData(cPos, r);

            myCircles.Add(currentCircle);
        }
        return myCircles;
    }

    struct CircleData
    {
        public Vector3 Position;
        public float Radius;

        public CircleData(Vector3 position, float radius)
        {
            Position = position;
            Radius = radius;
        }
    }
}
