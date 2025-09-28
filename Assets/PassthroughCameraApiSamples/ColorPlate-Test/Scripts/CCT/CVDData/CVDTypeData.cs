using System.Collections.Generic;
using System.Drawing;
using System.Linq;
//using System.Numerics;
using Colourful;
using Unity.XR.CoreUtils;
using UnityEngine;
using static Oculus.Interaction.InteractableColorVisual;

[CreateAssetMenu(fileName = "CVDType", menuName = "Scriptable Objects/CVD/CVDTypeData")]
public abstract class CVDTypeData : ScriptableObject
{
    public abstract string Name { get; }
    public abstract Vector2 CopunctPoint { get; }
    //Test response variables
    public float Threshold;
    public int Reversals;
    public int WrongAtMaxSat;
    public int TotalResponses;
    public int CorrectResponses;

    public bool CurrentResponseCorrect;
    public bool LastResponseCorrect;

    public bool IsActive = true;

    //storage properties for threshold calculation
    public Vector2 UVBeforeReversal;
    public Vector2 UVAfterReversal;
    public Vector2 currentuvAposth;

    //public thresholds = new Dictionary<ColorVector, float>();
    private List<float> thresholds = new List<float>();

    //former ColorVector properties
    public Vector2 FieldChromaticity { get; private set; } = new Vector2(0.413f, 0.360f);

    private static int totalSteps = 60;
    [SerializeField] private int currentStep = 20;

    #region Color Space conversions
    private LuvColor ColorXYToLuv(Vector2 xyVec)
    {
        var xy = new xyChromaticity(xyVec.x, xyVec.y);
        var rgbWorkingSpace = RGBWorkingSpaces.sRGB;

        var xyYToLuv = new ConverterBuilder().Fromxy(rgbWorkingSpace.WhitePoint).ToLuv(rgbWorkingSpace.WhitePoint).Build();
        var outputLuv = xyYToLuv.Convert(xy);
        return outputLuv;
    }

    private LinearRGBColor ColorXYYToLinRGB(Vector3 xyYVec)
    {
        var xyY = new xyYColor(xyYVec.x, xyYVec.y, xyYVec.z);
        var rgbWorkingSpace = RGBWorkingSpaces.sRGB;

        //to LinearRGB
        var xyYToLinearRGB = new ConverterBuilder().FromxyY(rgbWorkingSpace.WhitePoint).ToLinearRGB(rgbWorkingSpace).Build();
        var outputLinRGB = xyYToLinearRGB.Convert(xyY);
        return outputLinRGB;
    }

    private LinearRGBColor ColorXYToLinRGB(Vector2 xyVec)
    {
        var xy = new xyChromaticity(xyVec.x, xyVec.y);
        var rgbWorkingSpace = RGBWorkingSpaces.sRGB;

        //to LinearRGB
        var xyToLinearRGB = new ConverterBuilder().Fromxy(rgbWorkingSpace.WhitePoint).ToLinearRGB(rgbWorkingSpace).Build();
        var outputLinRGB = xyToLinearRGB.Convert(xy);
        return outputLinRGB;
    }

    private LinearRGBColor VecxyToLinRGB(Vector2 xyVec)
    {
        var uvVec76 = VecxyToVecuv1976(xyVec);
        var uv76Desat = SaturateUV1976(uvVec76);
        // Zu Ergebnis Luminanz hinzufügen und zu RGB umwandeln.
        //var uv60Desat = VecxyToVecuv1976(uv76Desat);
        var vecxyDesat = Vecuv1976Toxy(uv76Desat);
        var linRGB = ColorXYToLinRGB(vecxyDesat);
        return linRGB;
        
    }

    private xyYColor ColorLuvToXYY(Vector3 luvVec)
    {
        var luv = new LuvColor(luvVec.x, luvVec.y, luvVec.z);
        var rgbWorkingSpace = RGBWorkingSpaces.sRGB;

        //to xyY
        var luvToxyY = new ConverterBuilder().FromLuv(rgbWorkingSpace.WhitePoint).ToxyY(rgbWorkingSpace.WhitePoint).Build();
        var outputxyY = luvToxyY.Convert(luv);
        return outputxyY;
    }

    private xyChromaticity ColorLuvToXY(Vector3 luvVec)
    {
        var luv = new LuvColor(luvVec.x, luvVec.y, luvVec.z);
        var rgbWorkingSpace = RGBWorkingSpaces.sRGB;

        //to xy
        var luvToxy = new ConverterBuilder().FromLuv(rgbWorkingSpace.WhitePoint).Toxy(rgbWorkingSpace.WhitePoint).Build();
        var outputxy = luvToxy.Convert(luv);
        return outputxy;
    }

    //Berechnung von Seite 13 hieraus: https://www.uni-weimar.de/fileadmin/user/fak/medien/professuren/Computer_Graphics/course_material/Fundamentals_of_Imaging13/3-ima-color-spaces.pdf
    //Leider dort keine Quellen angeben. Selber nochmal nachsuchen für BA
    private Vector2 VecxyToVecuv1976(Vector2 vec) //I mean the newer version with apostroph but I'm not allowed to write that
    {
        var x = vec.x;
        var y = vec.y;
        var u = 2f * x / (6f * y - x + 1.5f); 
        var v = 4.5f * y / (6f * y - x + 1.5f);
        return new Vector2(u, v);
    }

    private Vector2 Vecuv1976ToVecUV1960(Vector2 vec) //I mean the newer version with apostroph but I'm not allowed to write that
    {
        var uAposth = vec.x;
        var vAposth = vec.y;
        var u = uAposth;
        var v = (2 / 3) * vAposth;
        return new Vector2(u, v);
    }

    private Vector2 Vecuv1976Toxy(Vector2 vec)
    {
        var uAposth = vec.x;
        var vAposth = vec.y;
        var x = (9 * uAposth) / (6 * uAposth - 16 * vAposth + 12);
        var y = (4 * vAposth) / (6 * uAposth - 16 * vAposth + 12);
        return new Vector2(x, y);
    }
    #endregion

    public void ResetData()
    {
        currentStep = 15;
        Threshold = 0;
        Reversals = 0;
        WrongAtMaxSat = 0;
        TotalResponses = 0;
        CorrectResponses = 0;
        CurrentResponseCorrect = false;
        LastResponseCorrect = false;
        IsActive = true;
        UVBeforeReversal = Vector2.zero;
        UVAfterReversal = Vector2.zero;
        thresholds.Clear();
    }

    public void OnDestroy()
    {
        ResetData();
    }

    public Vector3 GetBackgroundColor()
    {
        var myColor = ColorXYToLinRGB(FieldChromaticity);
        Vector3 outputVector = ColorToVector(myColor);
        return outputVector;
    }

    public Vector3 GetRGBColor()
    {
        var uvAposthCP = VecxyToVecuv1976(CopunctPoint);
        var colorSatuvAposth = SaturateUV1976(uvAposthCP);
        var myuvAposthChroma = Vecuv1976Toxy(colorSatuvAposth);
        var myColor = ColorXYToLinRGB(myuvAposthChroma);
        Vector3 outputVector = ColorToVector(myColor);
        return outputVector;
    }

    public Vector3 GetRGBColorOld()
    {
        //Vector2 selected = xyVectorForCVDType(current);
        //Debug.Log("I'm currently showing " + selected);
        var colorSat = Saturate(CopunctPoint);
        var myColor = ColorXYYToLinRGB(ColorToVector(colorSat));
        Vector3 outputVector = ColorToVector(myColor);
        return outputVector;
    }


    //dynamische Positionierung auf den Vektoren
    public Vector2 SaturateUV1976(Vector2 uvCP) //Saturation Vector based on u'v' chromaticity
    {
        var uvFC = VecxyToVecuv1976(FieldChromaticity);
        var uvChromaPath = uvCP - uvFC;
        var factor = Staircase();
        var uvDesat = uvFC + uvChromaPath * factor;
        return uvDesat;
    }
    public xyYColor Saturate(Vector2 xy)
    {
        var luvFC = ColorXYToLuv(FieldChromaticity);
        var luvCP = ColorXYToLuv(xy);
        var chromaPathUV = ColorToVector2(luvCP) - ColorToVector2(luvFC);

        //Vektorlänge reduzieren 
        var factor = Staircase();
        var desatLuvVec = ColorToVector2(luvFC) + (chromaPathUV * factor);
        var xyYDesaturated = ColorLuvToXYY(desatLuvVec);
        return xyYDesaturated;
    }

    public xyChromaticity SaturateChroma(Vector2 xy)
    {
        var luvFC = ColorXYToLuv(FieldChromaticity);
        var luvCP = ColorXYToLuv(xy);
        var colorPathlUV = ColorToVector(luvCP) - ColorToVector(luvFC);

        //Vektorlänge reduzieren 
        var factor = Staircase();
        var desatLuvVec = ColorToVector(luvFC) + (colorPathlUV * factor);
        var xyDesaturated = ColorLuvToXY(desatLuvVec);
        return xyDesaturated;
    }

    public void ReduceSaturation()
    { currentStep--; }

    public void IncreaseSaturation()
    { currentStep++; }

    private float Staircase()
    {

        if (currentStep >= totalSteps)
        {
            currentStep = totalSteps;
            WrongAtMaxSat++;
            Debug.Log("Staircase already reached its limit");

        }
        Debug.Log("Current Step liegt gerade bei: " + currentStep);
        float lerpFactor = (float)currentStep / (float)totalSteps;
        Debug.Log("Daraus ergibt sich der Interpolationsfaktor: " + lerpFactor);
        return lerpFactor;
    }

    //Threshold Methods
    public void SetUVNoReversal()
    {
        var uvFC = ColorXYToLuv(FieldChromaticity);
        var uvCP = ColorXYToLuv(CopunctPoint);
        var colorPathUV = ColorToVector(uvCP) - ColorToVector(uvFC);

        //Vektorlänge reduzieren 
        var factor = Staircase();
        UVBeforeReversal = ColorToVector(uvFC) + (colorPathUV * factor);
    }

    public void SetUVReversal()
    {
        var uvAposthFC = VecxyToVecuv1976(FieldChromaticity);
        var uvAposthCP = VecxyToVecuv1976(CopunctPoint);
        var chromaPathUV = uvAposthCP - uvAposthFC;

        //Vektorlänge reduzieren 
        var factor = Staircase();
        var uvReversal = uvAposthFC + (chromaPathUV * factor);
        thresholds.Add(uvReversal.magnitude);
    }

    public float GetThresholdValue(int maxFailures)
    {
        if (WrongAtMaxSat < maxFailures)
        {
            var lastReversals = thresholds.TakeLast(6).ToList();
            var average = lastReversals.Average();
            return average;
        }
        else return 120f; //Artificial MaxValue. Recalculate if Calculation for normal MaxValues changes

    }

    //Conversion helpers
    public Vector3 ColorToVector(LinearRGBColor myColor)
    {
        Vector3 outputVector = new Vector3((float)myColor.R, (float)myColor.G, (float)myColor.B);
        return outputVector;
    }

    public Vector3 ColorToVector(LuvColor myColor)
    {
        Vector3 outputVector = new Vector3((float)myColor.L, (float)myColor.u, (float)myColor.v);
        return outputVector;
    }

    public Vector3 ColorToVector(xyYColor myColor)
    {
        Vector3 outputVector = new Vector3((float)myColor.x, (float)myColor.y, (float)myColor.Luminance);
        return outputVector;
    }

    public Vector3 ColorToVector(xyChromaticity myColor)
    {
        Vector3 outputVector = new Vector3((float)myColor.x, (float)myColor.y, 0.5f);
        return outputVector;
    }

    //Conversion to Vector 2 discards Luminance and only holds Chromaticity values
    public Vector2 ColorToVector2(LuvColor myColor)
    {
        Vector2 outputVector = new Vector2((float)myColor.u, (float)myColor.v);
        return outputVector;
    }

    public Vector2 ColorToVector2(xyYColor myColor)
    {
        Vector2 outputVector = new Vector2((float)myColor.x, (float)myColor.y);
        return outputVector;
    }

    public Vector2 ColorToVector2(xyChromaticity myColor)
    {
        Vector2 outputVector = new Vector2((float)myColor.x, (float)myColor.y);
        return outputVector;
    }
}
