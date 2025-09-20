/* So, das hier wird jetzt die Hauptklasse, die den CCTCreator möglichst gut in andere Klassen ausgelagert nachstellt
 * Ziel 1: Testplate importieren und auf das Panel setzen. [DONE]
 * Ziel 2: Color Management
 *  Ziel 2.1: Punkte in beliebiger Farbe einfärben. [DONE]
 *  Ziel 2.2: C-Maske in anderer Farbe einfärben.
 *  Ziel 2.3: Luminance Noise für beide Farben adden
 */

using UnityEngine;

public class CCTManager : MonoBehaviour
{
    [SerializeField] private GameObject cctPlate;
    [SerializeField] private RectTransform stimulusContainer;

    //Necessary Components on same Object
    private ColorManager colorManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CollectComponents();
        SetPlate();
        SetColors();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CollectComponents()
    {
        colorManager = GetComponent<ColorManager>();
    }

    #region Positioning
    private void SetPlate()
    {
        RectTransform containerRect = stimulusContainer.GetComponent<RectTransform>();
        var width = containerRect.rect.width;
        var height = containerRect.rect.height;

        //Mittelpunkt des panels finden
        float cx = width / 2;
        float cy = height / 2;
        Vector2 panelCenter = new Vector2(cx, cy);

        cctPlate.SetActive(true);
        

        cctPlate.transform.SetParent(stimulusContainer);
        cctPlate.transform.localPosition = containerRect.position;
        cctPlate.transform.rotation = containerRect.rotation;

        ScalePlate();

    }

    private void ScalePlate()
    {
        // Panel-Größe in Weltkoordinaten
        Vector2 panelSize = stimulusContainer.rect.size * stimulusContainer.lossyScale;

        //Verhältnis Objekt zu Panel berechnen
        var plateData = cctPlate.GetComponent<TestPlate>();
        var objSize = plateData.BoundingBox.size;
        Debug.Log("The Testplate´s Bounding Boxes Size is: " + objSize);
        float scaleX = panelSize.x / objSize.x;
        float scaleY = panelSize.y / objSize.y;
        float finalScale = Mathf.Min(scaleX, scaleY);

        //tatsächliche Skalierung
        cctPlate.transform.localScale *= finalScale;
    }
    #endregion

    void SetColors()
    {
        var baseColor = colorManager.BackgroundColor;

        //Apply Colors to Circles
        var plateData = cctPlate.GetComponent<TestPlate>();

        foreach (var circle in plateData.Circles)
        {
            circle.GetComponentInChildren<SpriteRenderer>().material.color = baseColor;
        }
    }
}
