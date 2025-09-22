/* Klasse, die dem GameObject angehängt wird, welches die Testplate repräsentiert.
 * Enthält zusätzliche Metadaten, die nicht normalerweise im GameObject gespeichert werden.
 * Wird entweder Methoden enthalten, um seine eigenen Metadaten zu ermitteln und zu aktualisieren
 * oder hält nur Properties, die von anderen Klassen gesetzt werden. Könnte in dem Fall zu Struct umgewandelt werden
 * 
 * Achtung: Properties updaten sich nicht automatisch, wenn sich etwas an der Plate ändert... wie erreiche ich das?
 * Allerdings: Skalierung und Repositionierung des Parents nimmt die Bounds mit
 */
using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class TestPlate : MonoBehaviour
{
    public List<GameObject> Circles;
    public Bounds BoundingBox; //Typ Bounds speichert World-Space Koordinaten, nicht local space https://docs.unity3d.com/6000.2/Documentation/ScriptReference/Renderer-bounds.html
    public BoundingSphere RadialBounds;
    /*public struct RadialBounds
    {
        public float Radius;
        public float Center;

        public RadialBounds(float radius, float center)
        {
            Radius = radius;
            Center = center;
        }

    }*/

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        InitializeValues();
    }

    public void InitializeValues()
    {
        UpdateChildren();
        CalculateBounds();
        //SetCenterPoint(); // Unnötig, solange ich nicht ne neue SVG auslesen möchte
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateChildren()
    {
        foreach (Transform child in transform)
        {
            Circles.Add(child.gameObject);
        }
    }

    public void CalculateBounds()
    {
        //List<GameObject> children = new List<GameObject>();
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
        {
            Debug.LogWarning("Keine Renderer in den Children gefunden.");
            return;
        }

        // Bounds der Child-Renderer zusammenfassen
        Bounds bounds = renderers[0].bounds;
        foreach (var rend in renderers)
        {
            bounds.Encapsulate(rend.bounds);
        }
        BoundingBox = bounds;

        //jetzt radiale Bounds ergänzen
        var center = bounds.center;
        var radius = Math.Max(bounds.extents.x, bounds.extents.y);
        //RadialBounds = new BoundingSphere(center, radius);
    }

    public void SetCenterPoint()
    {
        var center = BoundingBox.center;
        foreach (GameObject go in Circles)
        {
            go.transform.position = go.transform.position - center;
        }
        BoundingBox.center -= center; //Ich glaube, diese Zeile ist ein Problem und verschiebt das Center weg vom Kreis. Entweder kanns gelöscht werden oder ich muss im CircleReader nach Lösungsinpiration suchen.
        //RadialBounds.position -= center;
    }

    //visible Bounding Box when Item selected
    public void OnDrawGizmosSelected()
    {
        /*var r = GetComponent<Renderer>();
        if (r == null)
            return;
        var bounds = r.bounds;*/
        Gizmos.matrix = Matrix4x4.identity;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(BoundingBox.center, BoundingBox.extents * 2);
    }
}
