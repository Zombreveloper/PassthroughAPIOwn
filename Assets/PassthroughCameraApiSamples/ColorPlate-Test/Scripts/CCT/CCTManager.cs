/* So, das hier wird jetzt die Hauptklasse, die den CCTCreator möglichst gut in andere Klassen ausgelagert nachstellt
 * Ziel 1: Testplate importieren und auf das Panel setzen. [DONE]
 *  Ziel 1.1: Plate Placement Methoden auf PlateManager schieben
 * Ziel 2: Color Management
 *  Ziel 2.1: Punkte in beliebiger Farbe einfärben. [DONE]
 *  Ziel 2.2: C-Maske in anderer Farbe einfärben. [DONE]
 *  Ziel 2.3: Luminance Noise für beide Farben adden [DONE]
 *  Ziel 2.4: ColorVektoren in eigener Klasse ansprechbar machen [Done]
 *  Ziel 2.5: Farben akkurat entsättigen
 *  
 * Ziel 3: Rest vom Test wieder einfügen Minus die ausgelagerten Methoden
 *  
 *  
 *  Ziel X: Protan, Deutan und Tritan Testprozeduren (Inklusive Anwortspeicherung) seperiert kapseln, um sie im Test randomisiert zu verweben
 *      Ziel X.2: Weg von dem Enum und ganze CVDType Klasse (vllt als Namespace) erstellen. Mit allen Daten spezifisch für diese Deficiency inklusive Vector und Zwischenspeicherung der Testantworten)
 *  
 *  Derzeitiges Hauptproblem: Wo speichere ich den current ColorVector so, dass ich ihn als Argument mitgeben darf?
 */

using System;
using System.Collections.Generic;
using CCT.VectorData;
using Meta.XR.ImmersiveDebugger.UserInterface;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CCTManager : MonoBehaviour
{
    [field: SerializeField] public GameObject CCTPlate;
    [SerializeField] private RectTransform stimulusContainer;

    private ColorVector currentVector;
    [SerializeField] private List<CVDTypeData> types;
    private CVDTypeData currentCVD;

    //Necessary Components on same Object
    private PlateManager plateManager;

    //properties from original Manager. some may get outsourced in the future:
    [Header("UI Components")]
    public Canvas testCanvas;
    public TMP_Text instructionText;
    public Button[] responseButtons; // 4 buttons for gap directions
    public TMP_Text resultsText;

    //Test state variables
    private TestPhase currentPhase;
    private Dictionary<ColorVector, float> thresholds;
    private int currentGapDirection; // 0=right, 1=up, 2=left, 3=down

    //Staircase step variables
    private int initialStep = 15;
    private int currentStep;
    private int staircaseTotalSteps = 20;


    //Structs and Enums
    /*public enum ColorVector
    {
        Protan,   // L-cone (red-green, affects long wavelength)
        Deutan,   // M-cone (red-green, affects medium wavelength)
        Tritan    // S-cone (blue-yellow, affects short wavelength)
    }*/

    public enum TestPhase
    {
        Instructions,
        Testing,
        Results
    }

    //public Array CVDType { get; private set; }
    public string[] cvdType = { "Protan", "Deutan", "Tritan"};


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //RandomCVDType();
        currentVector = ColorVector.Protan;
        var n = UnityEngine.Random.Range(0, types.Count);
        currentCVD = types[n];
        currentGapDirection = UnityEngine.Random.Range(0, 4);
        CollectComponents();
        plateManager.SetPlateToPanel(stimulusContainer);
        //SetPlate();
        plateManager.SetColors(currentVector, currentCVD, currentGapDirection);
        InitializeTest();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CollectComponents()
    {
        plateManager = GetComponent<PlateManager>();
    }

    //Darf gerne alles noch ausgedünnt werden aber hier erstmal alle übrigen (leicht angepassten) Funktionen des Ursprungs-Managers rein

    void InitializeTest()
    {
        //currentPhase = TestPhase.Instructions;
        thresholds = new Dictionary<ColorVector, float>();
        //circles = new List<CircleStimulus>();

        // Setup UI
        /*instructionText.text = "Cambridge Colour Test\n\nSie sehen gleich ein Muster aus Kreisen. " +
                              "Suchen Sie das 'C' (Kreis mit Öffnung) und drücken Sie die entsprechende Taste:\n" +
                              "→ (Rechts), ↑ (Oben), ← (Links), ↓ (Unten)\n\nKlicken Sie 'Start' um zu beginnen.";*/

        // Setup response buttons
        for (int i = 0; i < responseButtons.Length; i++)
        {
            int direction = i;
            responseButtons[i].onClick.AddListener(() => OnResponseButton(direction));
            //responseButtons[i].gameObject.SetActive(false);
        }

        resultsText.gameObject.SetActive(false);
    }

    public void OnResponseButton(int selectedDirection)
    {
        bool correct = selectedDirection == currentGapDirection;
        ProcessResponse(correct);
    }

    void ProcessResponse(bool correct) //Hier kommt noch eine Menge rein zum Prüfen der Antworten, aber erstmal nur nächste Platte
    {
        //total responses up
        //correct responses up if correct
        //relation between these two can be calculated and used somewhere else
        if (correct)
        {
            //decrease saturation by Step Size
            Debug.Log("Deine Antwort war richtig!");
            var cvdData = GetComponent<ColorVectors>();
            //cvdData.ReduceSaturation();
            currentCVD.ReduceSaturation();
        }
        else
        {
            //increasse saturation by Step Size
            Debug.Log("Deine Antwort war flasch!");
            var cvdData = GetComponent<ColorVectors>();
            currentCVD.IncreaseSaturation();
        }


        RandomCVDType();
        currentGapDirection = UnityEngine.Random.Range(0, 4);
        plateManager.SetColors(currentVector, currentCVD, currentGapDirection);
    }

    private void RandomCVDType() //evt zu Type mit Rückgabe ColorVector wechseln. Ist eigentlich aber egal
    {
        var ri = UnityEngine.Random.Range((int)0, cvdType.Length +1);
        if (ri == 0) { currentVector = ColorVector.Protan; }
        else if (ri == 1) { currentVector = ColorVector.Deutan; }
        else { currentVector = ColorVector.Tritan; }
    }

    #region Obsolete Methods but Kept for now because they are guaranteed to work here
    /*
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
    */
    #endregion
}