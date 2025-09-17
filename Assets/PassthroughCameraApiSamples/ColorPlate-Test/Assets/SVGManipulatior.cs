/*
 * Das ist eine erstversion auf Basis des Unity SVG-Package. Die ist eigentlich komplett wertlos und funktioniert nicht ordentlich
 * Liest eine SVG aus den Streamingassets und instanziert das Ergebnis
 */

using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using NUnit.Framework;
using Unity.VectorGraphics;
using UnityEngine;

public class SVGManipulatior : MonoBehaviour
{
    public SVGImage myImage;
    [SerializeField] public SceneNode mySVG;

    public SVGParser.SceneInfo myInfo;

    private SceneNode mySceneNode;
    private Scene myScene;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "ishiharaBlank.svg");
        if (File.Exists(path))
        {
            ParseSVG(path);
            ApplyColorToAll(myScene);
            SetToScene(myScene); //Hab ich richtig scheiße benannt. Ich mein halt einfach nur, dass ich das Ding jetzt endlich als Sprite in der Unity Szene, die aber nicht die SVG Scene ist, anzeigen möchte.
            //XDocument doc = XDocument.Load(path);
            //Debug.Log("SVG erfolgreich geladen!");
        }
        else
        {
            Debug.LogError("Datei nicht gefunden: " + path);
        }

        //var myChildren = new List<SceneNode>();

        /*mySVG = new SceneNode();
        myChildren = mySVG.Children;
        Debug.Log(myChildren.Count);

        foreach (SceneNode child in myChildren)
        {
            Debug.Log(child.Transform);
        }
        */
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetToScene(Scene scene)
    {
        // Scene in ein Sprite umwandeln
       var tessOptions = new VectorUtils.TessellationOptions()
        {
            StepDistance = 1f,
            MaxCordDeviation = 0.5f,
            MaxTanAngleDeviation = 0.1f,
            SamplingStepSize = 0.01f
        };

        Sprite svgSprite = VectorUtils.BuildSprite(
                    VectorUtils.TessellateScene(scene, tessOptions),
                    100.0f, // Pixels per unit
                    VectorUtils.Alignment.Center,
                    Vector2.zero,
                    128, // Gradient resolution
                    true);

        // GameObject mit SpriteRenderer erzeugen
        GameObject go = new GameObject("SVG Display");
        var renderer = go.AddComponent<SpriteRenderer>();
        renderer.sprite = svgSprite;
    }


    private void ParseSVG(string path)
    {
        using var reader = new StreamReader(path);
        myInfo = SVGParser.ImportSVG(reader);
        myScene = myInfo.Scene;
        if (myScene.Root == null)
        {
            Debug.LogError("Datei nicht gefunden: " + path);
        }
        else
        {
            Debug.Log("SVG erfolgreich geparsed! Root-Knoten vorhanden.");

            // Beispiel: Alle Kinder im Baum durchlaufen
            //Traverse(myScene.Root);
        }
    }

    private void ApplyColorToAll(Scene scene)
    {
        if (scene.Root != null)
        {
            var rootNode = scene.Root;
            Color greenFill = Color.green;

            foreach (var child in rootNode.Children) //Achtung, damit überspringe ich die erste Node!
            {
                var myShapes = child.Shapes;
                foreach (var shape in myShapes)
                {
                    var solid = new SolidFill();
                    solid.Color = greenFill;
                    shape.Fill = solid;
                }
            }
        }
        else { Debug.LogWarning("Die SVG Scene besitzt keinen Inhalt!"); }
    }

    //Ich muss den Scheiß hier mal eben komplett selber bauen. Irgendwas ist da ganz komisch und irgendwas wird da an Werten anscheinend weggeschmissen...
    /*void Traverse(SceneNode node, int depth = 0)
    {
        if (node == null)
        {
            Debug.LogWarning("Null-Node übersprungen");
            return;
        }
        string indent = new string(' ', depth * 2);
        //Debug.Log($"{indent}Node: {node.GetType()}, Children: {node.Children.Count}");
        Debug.Log("Hier in Ebene "+ depth + 1 +" ist alles in Ordnung");

        // Falls dieser Node eine Form ist, können wir auf Füllung/Stroke zugreifen
        if (node.Shapes != null)
        {
            foreach (var shape in node.Shapes)
            {
                Debug.Log($"{indent}  Shape mit Fill: {shape.Fill?.ToString()}");
            }
        }

        // Rekursiv durch alle Kinder
        if (node.Children != null)
        {
            //foreach (var child in node.Children)
            for (int i = 0; i < node.Children.Count; i++)
            {
                //if (child == null)
                if (node.Children[i] == null)
                {
                    Debug.LogWarning($"{indent}Kind {i} ist null!");
                    continue; // wichtig!
                }

                //Traverse(child, depth + 1);
                Traverse(node.Children[i], depth + 1);
            }
        }
    }*/
}
