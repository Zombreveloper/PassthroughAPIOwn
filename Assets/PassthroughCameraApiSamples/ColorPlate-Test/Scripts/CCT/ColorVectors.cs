/* Stores the ColorVectors for the different kinds of deficiencies... as soon as there are any.
 * Color Conversions provided by Colourful library https://github.com/tompazourek/Colourful/tree/master?tab=readme-ov-file
 * 
 * Achtung: Prüfen, ob die Quest überhaupt wirklich mit sRGB arbeitet!
 * Ich meine schon, aber könnte mich falsch erinnert haben.
 */
using UnityEngine;
using Colourful;

public class ColorVectors : MonoBehaviour
{
    private Vector2 fieldChromaticity = new Vector2(0.413f, 0.360f);
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

    private void ColorXYToUV(Vector2 xyVec)
    {
        var xyY = new xyYColor(xyVec.x,xyVec.y, 0.5);
        var rgbWorkingSpace = RGBWorkingSpaces.sRGB;

        //Step1: to LinearRGB
        var xyYToLinearRGB = new ConverterBuilder().FromxyY().ToLinearRGB(rgbWorkingSpace).Build();
        var outputLinRGB = xyYToLinearRGB.Convert(xyY);

        var xyYToLuv = new ConverterBuilder().FromxyY().ToLuv().Build();
        var outputLuv = xyYToLuv.Convert(xyY);
    }

    private LinearRGBColor ColorXYToLinRGB(Vector2 xyVec)
    {
        var xyY = new xyYColor(xyVec.x, xyVec.y, 0.5);
        var rgbWorkingSpace = RGBWorkingSpaces.sRGB;

        //Step1: to LinearRGB
        var xyYToLinearRGB = new ConverterBuilder().FromxyY(rgbWorkingSpace.WhitePoint).ToLinearRGB(rgbWorkingSpace).Build();
        var outputLinRGB = xyYToLinearRGB.Convert(xyY);
        return outputLinRGB;
    }

    public Vector3 GetRGBColor()
    {
        var myColor = ColorXYToLinRGB(protanPoint);
        Vector3 outputVector = new Vector3((float) myColor.R, (float) myColor.G, (float) myColor.B);
        return outputVector;
    }
}
