using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class CambridgeColourTest : MonoBehaviour
{
    [Header("UI Components")]
    public Canvas testCanvas;
    public RectTransform stimulusContainer;
    public Text instructionText;
    public Button[] responseButtons; // 4 buttons for gap directions
    public Text resultsText;

    [Header("Test Configuration")]
    public int circlesCount = 150;
    public float circleMinSize = 0.02f;
    public float circleMaxSize = 0.08f;
    public float gapSize = 0.3f; // Size of the C gap relative to stimulus area
    public float initialSaturation = 0.5f;
    public int reversalsToStop = 6;
    public float stepSizeInitial = 0.1f;
    public float stepSizeFinal = 0.02f;

    [Header("Color Configuration")]
    public Color backgroundColor = new Color(0.5f, 0.5f, 0.5f);
    public float luminanceNoiseRange = 0.3f;

    // Test state variables
    private TestPhase currentPhase;
    private ColorVector currentVector;
    private int currentGapDirection; // 0=right, 1=up, 2=left, 3=down
    private float currentSaturation;
    private List<float> reversalPoints;
    private int correctResponses;
    private int totalResponses;
    private bool waitingForResponse;

    // Results storage
    private Dictionary<ColorVector, float> thresholds;
    private List<CircleStimulus> circles;

    // Enums and data structures
    public enum TestPhase
    {
        Instructions,
        Testing,
        Results
    }

    public enum ColorVector
    {
        Protan,   // L-cone (red-green, affects long wavelength)
        Deutan,   // M-cone (red-green, affects medium wavelength)
        Tritan    // S-cone (blue-yellow, affects short wavelength)
    }

    [System.Serializable]
    public class CircleStimulus
    {
        public GameObject gameObject;
        public Image image;
        public bool isTarget;
        public Vector2 position;
        public float size;

        public CircleStimulus(GameObject go)
        {
            gameObject = go;
            image = go.GetComponent<Image>();
        }
    }

    void Start()
    {
        InitializeTest();
    }

    void InitializeTest()
    {
        currentPhase = TestPhase.Instructions;
        thresholds = new Dictionary<ColorVector, float>();
        circles = new List<CircleStimulus>();

        // Setup UI
        instructionText.text = "Cambridge Colour Test\n\nSie sehen gleich ein Muster aus Kreisen. " +
                              "Suchen Sie das 'C' (Kreis mit Öffnung) und drücken Sie die entsprechende Taste:\n" +
                              "→ (Rechts), ↑ (Oben), ← (Links), ↓ (Unten)\n\nKlicken Sie 'Start' um zu beginnen.";

        // Setup response buttons
        for (int i = 0; i < responseButtons.Length; i++)
        {
            int direction = i; // Capture for closure
            responseButtons[i].onClick.AddListener(() => OnResponseButton(direction));
            responseButtons[i].gameObject.SetActive(false);
        }

        resultsText.gameObject.SetActive(false);

        // Create circle pool
        CreateCirclePool();
    }

    void CreateCirclePool()
    {
        // Create a pool of circle GameObjects
        for (int i = 0; i < circlesCount; i++)
        {
            GameObject circle = new GameObject($"Circle_{i}");
            circle.transform.SetParent(stimulusContainer);

            Image img = circle.AddComponent<Image>();
            img.sprite = CreateCircleSprite();

            CircleStimulus stimulus = new CircleStimulus(circle);
            circles.Add(stimulus);

            circle.SetActive(false);
        }
    }

    Sprite CreateCircleSprite()
    {
        // Create a simple circle sprite
        Texture2D texture = new Texture2D(64, 64);
        Color[] pixels = new Color[64 * 64];

        Vector2 center = new Vector2(32, 32);
        float radius = 30f;

        for (int y = 0; y < 64; y++)
        {
            for (int x = 0; x < 64; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center);
                pixels[y * 64 + x] = distance <= radius ? Color.white : Color.clear;
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();

        return Sprite.Create(texture, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f));
    }

    public void StartTest()
    {
        currentPhase = TestPhase.Testing;
        currentVector = ColorVector.Protan; // Start with Protan

        instructionText.text = $"Testing: {currentVector} Vector\nFinden Sie das 'C' und wählen Sie die Öffnungsrichtung.";

        // Show response buttons
        foreach (var button in responseButtons)
        {
            button.gameObject.SetActive(true);
        }

        // Initialize test parameters
        currentSaturation = initialSaturation;
        reversalPoints = new List<float>();
        correctResponses = 0;
        totalResponses = 0;

        StartCoroutine(RunTestTrial());
    }

    IEnumerator RunTestTrial()
    {
        waitingForResponse = true;

        // Generate new stimulus
        GenerateStimulus();

        // Wait for response or timeout
        float timeoutTime = 10f;
        float elapsedTime = 0f;

        while (waitingForResponse && elapsedTime < timeoutTime)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (waitingForResponse)
        {
            // Timeout - treat as incorrect
            ProcessResponse(false);
        }
    }

    void GenerateStimulus()
    {
        // Random gap direction
        currentGapDirection = Random.Range(0, 4);

        // Calculate stimulus area
        RectTransform containerRect = stimulusContainer.GetComponent<RectTransform>();
        float width = containerRect.rect.width;
        float height = containerRect.rect.height;

        // Generate random circle positions and sizes
        List<Vector2> positions = new List<Vector2>();
        List<float> sizes = new List<float>();

        for (int i = 0; i < circlesCount; i++)
        {
            Vector2 pos = new Vector2(
                Random.Range(-width / 2, width / 2),
                Random.Range(-height / 2, height / 2)
            );
            float size = Random.Range(circleMinSize, circleMaxSize) * Mathf.Min(width, height);

            positions.Add(pos);
            sizes.Add(size);
        }

        // Determine target and background colors
        Color targetColor, backgroundBaseColor;
        GetColorsForVector(currentVector, currentSaturation, out targetColor, out backgroundBaseColor);

        // Create C-shape mask
        Vector2 cCenter = Vector2.zero;
        float cRadius = 0.2f * Mathf.Min(width, height);

        // Apply colors to circles
        for (int i = 0; i < circles.Count; i++)
        {
            var circle = circles[i];
            circle.gameObject.SetActive(true);

            // Position and scale
            circle.gameObject.transform.localPosition = positions[i];
            circle.gameObject.transform.localScale = Vector3.one * (sizes[i] / 64f);

            // Determine if this circle is part of the C
            Vector2 relativePos = positions[i] - cCenter;
            float distance = relativePos.magnitude;
            bool isInC = IsPositionInC(relativePos, cRadius, currentGapDirection);

            // Apply color with luminance noise
            Color baseColor = isInC ? targetColor : backgroundBaseColor;
            float luminanceNoise = Random.Range(-luminanceNoiseRange, luminanceNoiseRange);
            Color finalColor = AdjustLuminance(baseColor, luminanceNoise);

            circle.image.color = finalColor;
            circle.isTarget = isInC;
        }
    }

    bool IsPositionInC(Vector2 position, float radius, int gapDirection)
    {
        float distance = position.magnitude;

        // Not in circle at all
        if (distance > radius) return false;

        // Calculate gap angle based on direction
        float gapAngle = gapDirection * 90f; // 0°, 90°, 180°, 270°
        float gapWidth = 60f; // degrees

        // Convert position to angle
        float angle = Mathf.Atan2(position.y, position.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360f;

        // Check if position is in the gap
        float angleDiff = Mathf.DeltaAngle(angle, gapAngle);
        bool isInGap = Mathf.Abs(angleDiff) < gapWidth / 2f;

        // Also check inner radius for C shape
        float innerRadius = radius * 0.6f;
        bool isInInnerCircle = distance < innerRadius;

        return !isInGap && !isInInnerCircle;
    }

    void GetColorsForVector(ColorVector vector, float saturation, out Color target, out Color background)
    {
        background = backgroundColor;

        switch (vector)
        {
            case ColorVector.Protan:
                // Protan confusion line (affects L-cones, red-green axis)
                target = new Color(
                    backgroundColor.r + saturation * 0.5f,
                    backgroundColor.g - saturation * 0.3f,
                    backgroundColor.b,
                    1f
                );
                break;

            case ColorVector.Deutan:
                // Deutan confusion line (affects M-cones, red-green axis)
                target = new Color(
                    backgroundColor.r + saturation * 0.4f,
                    backgroundColor.g - saturation * 0.4f,
                    backgroundColor.b,
                    1f
                );
                break;

            case ColorVector.Tritan:
                // Tritan confusion line (affects S-cones, blue-yellow axis)
                target = new Color(
                    backgroundColor.r + saturation * 0.2f,
                    backgroundColor.g + saturation * 0.3f,
                    backgroundColor.b - saturation * 0.5f,
                    1f
                );
                break;

            default:
                target = Color.red;
                break;
        }

        // Clamp colors to valid range
        target = new Color(
            Mathf.Clamp01(target.r),
            Mathf.Clamp01(target.g),
            Mathf.Clamp01(target.b),
            1f
        );
    }

    Color AdjustLuminance(Color baseColor, float adjustment)
    {
        // Simple luminance adjustment
        return new Color(
            Mathf.Clamp01(baseColor.r + adjustment),
            Mathf.Clamp01(baseColor.g + adjustment),
            Mathf.Clamp01(baseColor.b + adjustment),
            baseColor.a
        );
    }

    public void OnResponseButton(int selectedDirection)
    {
        if (!waitingForResponse) return;

        waitingForResponse = false;
        bool correct = selectedDirection == currentGapDirection;
        ProcessResponse(correct);
    }

    void ProcessResponse(bool correct)
    {
        totalResponses++;
        if (correct) correctResponses++;

        // Staircase algorithm
        float previousSaturation = currentSaturation;

        if (correct)
        {
            // Decrease saturation (make it harder)
            currentSaturation -= GetCurrentStepSize();
        }
        else
        {
            // Increase saturation (make it easier)
            currentSaturation += GetCurrentStepSize();
        }

        currentSaturation = Mathf.Clamp01(currentSaturation);

        // Check for reversal
        bool isReversal = false;
        if (reversalPoints.Count > 0)
        {
            float lastChange = currentSaturation - previousSaturation;
            float previousChange = previousSaturation - (reversalPoints.Count > 1 ? reversalPoints[reversalPoints.Count - 2] : initialSaturation);

            if (lastChange * previousChange < 0) // Sign change indicates reversal
            {
                isReversal = true;
                reversalPoints.Add(previousSaturation);
            }
        }
        else if (totalResponses > 1)
        {
            // First potential reversal
            reversalPoints.Add(previousSaturation);
        }

        // Update UI
        instructionText.text = $"Testing: {currentVector} Vector\n" +
                              $"Accuracy: {correctResponses}/{totalResponses} " +
                              $"({(correctResponses * 100f / totalResponses):F1}%)\n" +
                              $"Current Saturation: {(currentSaturation * 100):F1}%\n" +
                              $"Reversals: {reversalPoints.Count}";

        // Check if we should continue or move to next vector
        if (reversalPoints.Count >= reversalsToStop)
        {
            // Calculate threshold for this vector
            float threshold = CalculateThreshold();
            thresholds[currentVector] = threshold;

            // Move to next vector or finish
            MoveToNextVector();
        }
        else
        {
            // Continue with next trial
            StartCoroutine(RunTestTrial());
        }
    }

    float GetCurrentStepSize()
    {
        // Reduce step size as we get more reversals
        if (reversalPoints.Count < 2)
            return stepSizeInitial;
        else
            return stepSizeFinal;
    }

    float CalculateThreshold()
    {
        if (reversalPoints.Count < 4) return currentSaturation;

        // Use last 4 reversals for threshold calculation
        var lastReversals = reversalPoints.TakeLast(4).ToList();
        return lastReversals.Average();
    }

    void MoveToNextVector()
    {
        // Hide circles
        foreach (var circle in circles)
        {
            circle.gameObject.SetActive(false);
        }

        // Move to next vector
        switch (currentVector)
        {
            case ColorVector.Protan:
                currentVector = ColorVector.Deutan;
                ResetTestParameters();
                StartCoroutine(RunTestTrial());
                break;

            case ColorVector.Deutan:
                currentVector = ColorVector.Tritan;
                ResetTestParameters();
                StartCoroutine(RunTestTrial());
                break;

            case ColorVector.Tritan:
                FinishTest();
                break;
        }
    }

    void ResetTestParameters()
    {
        currentSaturation = initialSaturation;
        reversalPoints.Clear();
        correctResponses = 0;
        totalResponses = 0;
    }

    void FinishTest()
    {
        currentPhase = TestPhase.Results;

        // Hide response buttons
        foreach (var button in responseButtons)
        {
            button.gameObject.SetActive(false);
        }

        // Show results
        string results = "Cambridge Colour Test - Ergebnisse:\n\n";

        foreach (var kvp in thresholds)
        {
            float threshold = kvp.Value * 100f; // Convert to percentage
            results += $"{kvp.Key}: {threshold:F2}%\n";

            // Basic classification
            string classification = ClassifyResult(kvp.Key, kvp.Value);
            results += $"  → {classification}\n\n";
        }

        results += GetOverallClassification();

        instructionText.text = results;
        resultsText.text = "Test abgeschlossen. Ergebnisse siehe oben.";
        resultsText.gameObject.SetActive(true);
    }

    string ClassifyResult(ColorVector vector, float threshold)
    {
        // Simple classification based on threshold values
        // These values should be calibrated with real data

        if (threshold < 0.05f)
            return "Normal";
        else if (threshold < 0.15f)
            return "Leichte Anomalie";
        else if (threshold < 0.3f)
            return "Mittlere Anomalie";
        else
            return "Starke Anomalie/Dichromatie";
    }

    string GetOverallClassification()
    {
        string overall = "Gesamtbewertung:\n";

        float protanThreshold = thresholds.ContainsKey(ColorVector.Protan) ? thresholds[ColorVector.Protan] : 0f;
        float deutanThreshold = thresholds.ContainsKey(ColorVector.Deutan) ? thresholds[ColorVector.Deutan] : 0f;
        float tritanThreshold = thresholds.ContainsKey(ColorVector.Tritan) ? thresholds[ColorVector.Tritan] : 0f;

        if (protanThreshold < 0.05f && deutanThreshold < 0.05f && tritanThreshold < 0.05f)
        {
            overall += "Normale Farbwahrnehmung";
        }
        else if (protanThreshold > deutanThreshold && protanThreshold > tritanThreshold)
        {
            overall += protanThreshold > 0.3f ? "Protanopie/Protanomalie" : "Protanomalie";
        }
        else if (deutanThreshold > protanThreshold && deutanThreshold > tritanThreshold)
        {
            overall += deutanThreshold > 0.3f ? "Deuteranopie/Deuteranomalie" : "Deuteranomalie";
        }
        else if (tritanThreshold > 0.15f)
        {
            overall += "Tritanomalie/Tritanopie";
        }
        else
        {
            overall += "Gemischte Farbsehstörung";
        }

        return overall;
    }

    // Public method to get results for external use
    public Dictionary<ColorVector, float> GetThresholds()
    {
        return new Dictionary<ColorVector, float>(thresholds);
    }

    // Method to export results as JSON or other format
    public string ExportResults()
    {
        string json = "{\n";
        json += "  \"test_type\": \"Cambridge_Colour_Test\",\n";
        json += "  \"timestamp\": \"" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\",\n";
        json += "  \"thresholds\": {\n";

        var thresholdList = thresholds.ToList();
        for (int i = 0; i < thresholdList.Count; i++)
        {
            json += $"    \"{thresholdList[i].Key}\": {thresholdList[i].Value:F4}";
            if (i < thresholdList.Count - 1) json += ",";
            json += "\n";
        }

        json += "  },\n";
        json += "  \"classification\": \"" + GetOverallClassification().Replace("\n", " ").Replace("\"", "'") + "\"\n";
        json += "}";

        return json;
    }
}