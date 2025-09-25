using System.Drawing;
using CCT.VectorData;
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

    //former ColorVector properties
    public Vector2 FieldChromaticity { get; private set; } = new Vector2(0.413f, 0.360f);

    //Copunctal Points (Kommen weg sobald ich die Switchcase Unterscheidung woanders eingefügt habe... die evt gar nicht mehr nötig ist?
    private Vector2 protanPoint = new Vector2(0.747f, 0.253f);
    private Vector2 deutanPoint = new Vector2(1.40f, -0.40f);
    private Vector2 tritanPoint = new Vector2(0.171f, 0f);

    private static int totalSteps = 20;
    private int currentStep = 15;

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

    private xyYColor ColorLuvToXYY(Vector3 luvVec)
    {
        var luv = new LuvColor(luvVec.x, luvVec.y, luvVec.z);
        var rgbWorkingSpace = RGBWorkingSpaces.sRGB;

        //to xyY
        var luvToxyY = new ConverterBuilder().FromLuv(rgbWorkingSpace.WhitePoint).ToxyY(rgbWorkingSpace.WhitePoint).Build();
        var outputxyY = luvToxyY.Convert(luv);
        return outputxyY;
    }
    #endregion

    private Vector2 xyVectorForCVDType(ColorVector current)
    {
        switch (current)
        {
            case ColorVector.Protan:
                return protanPoint;

            case ColorVector.Deutan:
                return deutanPoint;

            case ColorVector.Tritan:
                return tritanPoint;

            default:
                return Vector2.zero;
        }
    }

    public Vector3 GetBackgroundColor()
    {
        var myColor = ColorXYToLinRGB(FieldChromaticity);
        Vector3 outputVector = ColorToVector(myColor);
        return outputVector;
    }

    public Vector3 GetRGBColor(ColorVector current)
    {
        /*Vector2 selected;
        switch (current)
        {
            case ColorVector.Protan:
                selected = protanPoint;
                break;

            case ColorVector.Deutan:
                selected = deutanPoint;
                break;

            case ColorVector.Tritan:
                selected = tritanPoint;
                break;

            default: selected = Vector2.zero;
                break;
        }*/
        Vector2 selected = xyVectorForCVDType(current);
        //Debug.Log("I'm currently showing " + selected);
        var colorSat = Saturate(selected);
        var myColor = ColorXYYToLinRGB(ColorToVector(colorSat));
        Vector3 outputVector = ColorToVector(myColor);
        return outputVector;
    }


    //dynamische Positionen auf den Vektoren
    public xyYColor Saturate(Vector2 xy)
    {
        var uvFC = ColorXYToLuv(FieldChromaticity);
        var uvCP = ColorXYToLuv(xy);
        var colorPathUV = ColorToVector(uvCP) - ColorToVector(uvFC);

        //Vektorlänge reduzieren 
        var factor = Staircase();
        var desatLuvVec = ColorToVector(uvFC) + (colorPathUV * factor);
        var xyYDesaturated = ColorLuvToXYY(desatLuvVec); //new xyYColor(finalVec.x, finalVec.y, finalVec.z);
        return xyYDesaturated;
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
            Debug.Log("Staircase already reached its limit");
            //sende Signal zurück an CCT-Manager, dass der Limit-Case eingetreten ist

        }
        Debug.Log("Current Step liegt gerade bei: " + currentStep);
        float lerpFactor = (float)currentStep / (float)totalSteps;
        Debug.Log("Daraus ergibt sich der Interpolationsfaktor: " + lerpFactor);
        return lerpFactor;
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
}
