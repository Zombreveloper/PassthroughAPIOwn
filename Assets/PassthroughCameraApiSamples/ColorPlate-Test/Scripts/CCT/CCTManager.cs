/* So, das hier wird jetzt die Hauptklasse, die den CCTCreator möglichst gut in andere Klassen ausgelagert nachstellt
 * Ziel 1: Testplate importieren und auf das Panel setzen. [DONE]
 *  Ziel 1.1: Plate Placement Methoden auf PlateManager schieben
 * Ziel 2: Color Management
 *  Ziel 2.1: Punkte in beliebiger Farbe einfärben. [DONE]
 *  Ziel 2.2: C-Maske in anderer Farbe einfärben. [DONE]
 *  Ziel 2.3: Luminance Noise für beide Farben adden [DONE]
 *  Ziel 2.4: ColorVektoren in eigener Klasse ansprechbar machen [Done]
 *  Ziel 2.5: Farben akkurat entsättigen [DONE]
 *  Ziel 2.6: Helligkeiten normalisieren
 *  
 * Ziel 3: Rest vom Test wieder einfügen Minus die ausgelagerten Methoden
 * Ziel 4: Endbedingungen festlegen
 * Ziel 5: Ergebnisse ausgeben => Threshold
 *  
 *  
 *  Ziel X: Protan, Deutan und Tritan Testprozeduren (Inklusive Anwortspeicherung) seperiert kapseln, um sie im Test randomisiert zu verweben
 *      Ziel X.2: Weg von dem Enum und ganze CVDType Klasse (vllt als Namespace) erstellen. Mit allen Daten spezifisch für diese Deficiency inklusive Vector und Zwischenspeicherung der Testantworten)
 *  [DONE]
 *  
 */

using System;
using System.Collections.Generic;
using System.Linq;
using Meta.XR.ImmersiveDebugger.UserInterface;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CCTManager : MonoBehaviour
{
    [field: SerializeField] public GameObject CCTPlate;
    [SerializeField] private RectTransform stimulusContainer;

    [SerializeField] private List<CVDTypeData> types;
    private CVDTypeData currentCVD;

    //Necessary Components on same Object
    private PlateManager plateManager;

    //properties from original Manager. some may get outsourced in the future:
    [Header("UI Components")]
    public Canvas testCanvas;
    public TMP_Text instructionText;
    public Button[] responseButtons; // 4 buttons for gap directions

    //Test state variables
    private TestPhase currentPhase;
    //private Dictionary<ColorVector, float> thresholds;
    private int currentGapDirection; // 0=right, 1=up, 2=left, 3=down

    //Test Progress
    private int totalRespones;
    private int correctRespones;
    private Vector3 results;

    //EndRequirements
    [SerializeField] private int maxReversals = 11;
    [SerializeField] private int maxWrongFullSaturation = 5;


    //Structs and Enums

    public enum TestPhase
    {
        Instructions,
        Testing,
        Results
    }


    private void Awake()
    {
        foreach (var type in types) { type.ResetData(); }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ChooseRandomCVD();
        //currentCVD = types[1]; //testing only Protan
        
        currentGapDirection = UnityEngine.Random.Range(0, 4);
        CollectComponents();
        plateManager.SetPlateToPanel(stimulusContainer);
        //SetPlate();
        plateManager.SetColors(currentCVD, currentGapDirection);
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
        currentPhase = TestPhase.Testing;
        //thresholds = new Dictionary<ColorVector, float>();

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

        //resultsText.gameObject.SetActive(false);
    }

    public void OnResponseButton(int selectedDirection)
    {
        bool correct = selectedDirection == currentGapDirection;
        ProcessResponse(correct);
    }

    void ProcessResponse(bool correct)
    {
        //adjust saturation
        if (correct)
        {
            //decrease saturation by Step Size
            Debug.Log("Deine Antwort war richtig!");
            currentCVD.ReduceSaturation();
        }
        else
        {
            //increasse saturation by Step Size
            Debug.Log("Deine Antwort war flasch!");
            currentCVD.IncreaseSaturation();
        }

        bool isReversal = CheckReversal(correct);
        UpdateReversalValues(isReversal);
        SetCVDTypeStatus();

        //count the total and correct Respones of the current CVDType
        //IMPORTANT TO KEEP AFTER PROCESSING AND BEFORE SETTING NEW PLATE
        currentCVD.TotalResponses++;
        if (correct) { currentCVD.CorrectResponses++; }
        //set next testplate

        //Check if test should end or continue
        if (EveryTypeInactive())
        {
            FinishTest();
        }
        else
        {
            ChooseRandomCVD();
            //currentCVD = types[1]; //testing only protan
            currentGapDirection = UnityEngine.Random.Range(0, 4);
            plateManager.SetColors(currentCVD, currentGapDirection);
        }
    }

    private void ChooseRandomCVD()
    {
        var n = UnityEngine.Random.Range(0, types.Count);
        currentCVD = types[n];
        //If CVDType is done, do a reroll. Funktioniert, geht aber ganz sicher ressourcenschonender!
        if (!currentCVD.IsActive)
        {
            ChooseRandomCVD();
            return;
        }
        Debug.Log("We are now testing for: " + currentCVD.Name + ". List Index: " + n);
    }

    private bool CheckReversal(bool currentResponseCorrect)
    {
        Debug.Log("Diese Antwort war wirklich und wahrhaftig: " + currentResponseCorrect);
        currentCVD.CurrentResponseCorrect = currentResponseCorrect;
        var last = currentCVD.LastResponseCorrect;
        var current = currentCVD.CurrentResponseCorrect;

        if (currentCVD.TotalResponses == (int)0)
        {
            //do nothing
            Debug.Log("No Reversal happened because this was your first response");
            return false;
        }
        else if (last != current)
        {
            currentCVD.Reversals++;
            Debug.Log("Reversal happened! Reversals of " + currentCVD.Name + " is now " + currentCVD.Reversals);
            return true;
        }
        else
        {
            //do nothing
            Debug.Log("No Reversal happened!");
            return true;
        }
        currentCVD.LastResponseCorrect = currentResponseCorrect;
        Debug.Log("Last Response now updated to: " + currentCVD.LastResponseCorrect);
    }

    private void UpdateReversalValues(bool reversal)
    {
        if (!reversal)
        {
            //currentCVD.SetUVNoReversal();
            return;
        }
        else
        {
            currentCVD.SetUVReversal();
        }
    }

    private void SetCVDTypeStatus()
    {
        if (currentCVD.Reversals >= maxReversals || currentCVD.WrongAtMaxSat >= maxWrongFullSaturation)
        {
            currentCVD.IsActive = false;
        }
    }

    private bool EveryTypeInactive()
    {
        //responds true if every Type is inactive
        bool noneTrue = types.All(t => !t.IsActive);
        return noneTrue;
    }

    private void FinishTest()
    {
        currentPhase = TestPhase.Results;

        // Hide response buttons
        foreach (var button in responseButtons)
        {
            button.gameObject.SetActive(false);
        }
        plateManager.DisablePlate();

        results = CalculateResults();
        Debug.Log("Test Ende");

        //Show results in text
        string resultText = "You completed the Color Test. Your results: \n";
        resultText += $"Protan Score: {results.x} \n";
        resultText += $"Deutan Score: {results.y} \n";
        resultText += $"Tritan Score: {results.x} \n";
        resultText += $"What does this mean for you? I am not entirely sure yet. I didn't test this through.";
        instructionText.text = resultText;
    }

    private Vector3 CalculateResults() //Vllt stattdessen als Liste oder dict?
    {
        var protanThreshold = types[0].GetThresholdValue(maxWrongFullSaturation);
        var deutanThreshold = types[1].GetThresholdValue(maxWrongFullSaturation);
        var tritanThreshold = types[2].GetThresholdValue(maxWrongFullSaturation);
        Vector3 result = new Vector3(protanThreshold, deutanThreshold, tritanThreshold);
        return result;
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