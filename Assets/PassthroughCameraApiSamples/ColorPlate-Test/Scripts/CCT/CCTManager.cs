/* So, das hier wird jetzt die Hauptklasse, die den CCTCreator möglichst gut in andere Klassen ausgelagert nachstellt
 * Ziel 1: Testplate importieren und auf das Panel setzen. [DONE]
 *  Ziel 1.1: Plate Placement Methoden auf PlateManager schieben
 * Ziel 2: Color Management
 *  Ziel 2.1: Punkte in beliebiger Farbe einfärben. [DONE]
 *  Ziel 2.2: C-Maske in anderer Farbe einfärben. [DONE]
 *  Ziel 2.3: Luminance Noise für beide Farben adden [DONE]
 *  Ziel 2.4: ColorVektoren in eigener Klasse ansprechbar machen
 *  
 *  Derzeitiges Hauptproblem: Wo speichere ich den current ColorVector so, dass ich ihn als Argument mitgeben darf?
 */

using Meta.XR.ImmersiveDebugger.UserInterface;
using UnityEngine;

public class CCTManager : MonoBehaviour
{
    [field: SerializeField] public GameObject CCTPlate;
    [SerializeField] private RectTransform stimulusContainer;

    //Necessary Components on same Object
    private PlateManager plateManager;

    //Structs and Enums
    public enum ColorVector
    {
        Protan,   // L-cone (red-green, affects long wavelength)
        Deutan,   // M-cone (red-green, affects medium wavelength)
        Tritan    // S-cone (blue-yellow, affects short wavelength)
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CollectComponents();
        plateManager.SetPlateToPanel(stimulusContainer);
        plateManager.SetColors();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CollectComponents()
    {
        plateManager = GetComponent<PlateManager>();
    }

    



    #region Obsolete Methods but Kept for now because they are guaranteed to work here
    private void SetPlate()
    {
        RectTransform containerRect = stimulusContainer.GetComponent<RectTransform>();
        var width = containerRect.rect.width;
        var height = containerRect.rect.height;

        //Mittelpunkt des panels finden
        float cx = width / 2;
        float cy = height / 2;
        Vector2 panelCenter = new Vector2(cx, cy);

        CCTPlate.SetActive(true);


        CCTPlate.transform.SetParent(stimulusContainer);
        CCTPlate.transform.localPosition = containerRect.position;
        CCTPlate.transform.rotation = containerRect.rotation;

        ScalePlate();

    }

    private void ScalePlate()
    {
        // Panel-Größe in Weltkoordinaten
        Vector2 panelSize = stimulusContainer.rect.size * stimulusContainer.lossyScale;

        //Verhältnis Objekt zu Panel berechnen
        var plateData = CCTPlate.GetComponent<TestPlate>();
        var objSize = plateData.BoundingBox.size;
        Debug.Log("The Testplate´s Bounding Boxes Size is: " + objSize);
        float scaleX = panelSize.x / objSize.x;
        float scaleY = panelSize.y / objSize.y;
        float finalScale = Mathf.Min(scaleX, scaleY);

        //tatsächliche Skalierung
        CCTPlate.transform.localScale *= finalScale;
    }

    void SetColors()
    {
        var baseColor = plateManager.GetBackgroundColor();

        //Apply Colors to Circles
        var plateData = CCTPlate.GetComponent<TestPlate>();

        foreach (var circle in plateData.Circles)
        {
            circle.GetComponentInChildren<SpriteRenderer>().material.color = baseColor;
        }
    }
    #endregion
}
