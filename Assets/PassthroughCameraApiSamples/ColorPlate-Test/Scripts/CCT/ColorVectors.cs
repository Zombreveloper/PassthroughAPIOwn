/* Stores the ColorVectors for the different kinds of deficiencies... as soon as there are any.
 * Color Conversions provided by Colourful library https://github.com/tompazourek/Colourful/tree/master?tab=readme-ov-file
 * 
 * Achtung: Prüfen, ob die Quest überhaupt wirklich mit sRGB arbeitet!
 * Ich meine schon, aber könnte mich falsch erinnert haben.
 */
using System.Drawing;
using CCT.VectorData;
using Colourful;
using Unity.XR.CoreUtils;
using UnityEngine;
using static Oculus.Interaction.InteractableColorVisual;

public class ColorVectors : MonoBehaviour
{
    public Vector2 FieldChromaticity { get; private set; } = new Vector2(0.413f, 0.360f);
    //Copunctual Points
    private Vector2 protanPoint = new Vector2(0.747f, 0.253f);
    private Vector2 deutanPoint = new Vector2(1.40f, -0.40f);
    private Vector2 tritanPoint = new Vector2(0.171f, 0f);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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

    public Vector3 GetBackgroundColor()
    {
        var myColor = ColorXYToLinRGB(FieldChromaticity);
        Vector3 outputVector = ColorToVector(myColor);
        return outputVector;
    }

    public Vector3 GetRGBColor(ColorVector current)
    {
        Vector2 selected;
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
        }

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

        //Vektorlänge halbieren 
        var desatLuvVec = ColorToVector(uvFC) + (colorPathUV * 0.1f);
        var xyYDesaturated = ColorLuvToXYY(desatLuvVec); //new xyYColor(finalVec.x, finalVec.y, finalVec.z);
        return xyYDesaturated;
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
