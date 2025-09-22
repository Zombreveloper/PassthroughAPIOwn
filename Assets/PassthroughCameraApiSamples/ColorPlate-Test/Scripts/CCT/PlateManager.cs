/* Klasse, die sich um das gesamte Management der CCT Testplate kümmert.
 * Das beinhaltet: Positionierung, Skalierung, Einfärbung, C-Shape
 * Bekommt alle Befehle und Anweisungen vom CCTManager. Inklusive Daten, wenn er sie selbst hat.
 */
using CCT.VectorData;
using System.ComponentModel;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class PlateManager : MonoBehaviour
{
    private ColorManager colorManager;
    private CShape cShape;
    private GameObject testPlate;

    [SerializeField] public float luminanceNoiseRange = 0.3f;

    private void Awake()
    {
        CollectComponents();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void CollectComponents()
    {
        testPlate = GetComponent<CCTManager>().CCTPlate;
        //colorManager = GetComponent<ColorManager>();
        colorManager = gameObject.AddComponent<ColorManager>();
        cShape = GetComponent<CShape>();
    }

    //Plate positioning
    public void SetPlateToPanel(RectTransform panel)
    {
        RectTransform containerRect = panel.GetComponent<RectTransform>();
        var width = containerRect.rect.width;
        var height = containerRect.rect.height;

        //Mittelpunkt des panels finden
        float cx = width / 2;
        float cy = height / 2;
        Vector2 panelCenter = new Vector2(cx, cy);

        if (!testPlate)
        {
            Debug.Log("PlateManager currently does not own a CCTPlate");
            return;
        }
        testPlate.SetActive(true);


        testPlate.transform.SetParent(panel);
        testPlate.transform.localPosition = containerRect.position;
        testPlate.transform.rotation = containerRect.rotation;

        ScalePlate(panel);
    }

    private void ScalePlate(RectTransform panel)
    {
        // Panel-Größe in Weltkoordinaten
        Vector2 panelSize = panel.rect.size * panel.lossyScale;

        //Verhältnis Objekt zu Panel berechnen
        var plateData = testPlate.GetComponent<TestPlate>();
        var objSize = plateData.BoundingBox.size;
        Debug.Log("The Testplate´s Bounding Boxes Size is: " + objSize);
        float scaleX = panelSize.x / objSize.x;
        float scaleY = panelSize.y / objSize.y;
        float finalScale = Mathf.Min(scaleX, scaleY);

        //tatsächliche Skalierung
        testPlate.transform.localScale *= finalScale;
    }

    //Coloring
    public Color GetBackgroundColor() //Könnte bald Argumente wie Current Vector annehmen. Und sowohl target als auch bg Color ausgeben
    {
        return colorManager.BackgroundColor;
    }

    public Color GetTargetColor()
    {
        return colorManager.TargetColor;
    }

    public void SetColors(ColorVector vec)
    {
        //var bgColor = GetBackgroundColor();
        var bgColor = colorManager.BackgroundColor;
        var targetColor = GetTargetColor();
        //colorManager.ApplyColorVectors(vec, 0.5f, out targetColor, out bgColor);
        colorManager.GetColorsForVector(out targetColor, out bgColor);
        var plateData = testPlate.GetComponent<TestPlate>();
        SetCShape();

        //Apply Colors to Circles
        foreach (var circle in plateData.Circles)
        {
            bool isInC = cShape.IsPositionInside(circle.transform.localPosition);
            Color baseColor = isInC ? targetColor : bgColor;
            var finalColor = AdjustLuminance(baseColor);
            circle.GetComponentInChildren<SpriteRenderer>().material.color = finalColor;
        }
    }

    Color AdjustLuminance(Color baseColor)
    {
        // Simple luminance adjustment
        float luminanceNoise = Random.Range(-luminanceNoiseRange, luminanceNoiseRange);
        return new Color(
            Mathf.Clamp01(baseColor.r + luminanceNoise),
            Mathf.Clamp01(baseColor.g + luminanceNoise),
            Mathf.Clamp01(baseColor.b + luminanceNoise),
            baseColor.a
        );
    }

    public void SetCShape() //oder CreateCShape?
    {
        //hier stattdessen ein CShape.Create(alle wichtigen Variablen) einfügen. Den rest macht die CShape Klasse!

        Vector3 plateExtents = testPlate.GetComponent<TestPlate>().BoundingBox.extents;
        float cRadius = 0.7f * Mathf.Min(plateExtents.x, plateExtents.y); //Größe des C in Relation zur Testplate
        int gap = 1; //Platzhalter
        cShape.CreateShape(cRadius, gap);
    }
}
