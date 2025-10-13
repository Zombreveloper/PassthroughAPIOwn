/* Calls Data for ColorRanges and calculates Color Output
 * TBH this is pretty superfluous by now 
 */

using UnityEngine;
using Colourful;

public class ColorManager : MonoBehaviour
{
    //Placeholder Colors
    //private Color m_backgroundColor = new Color(0.5f, 0.5f, 0.5f);
    //private Color m_targetColor = new Color(0.7f, 0.3f, 0.3f);

    /*private CVDTypeData currentCVD;
    private Vector2 currentFC;
    private Vector2 currentCP;*/

    private void Awake()
    {
        //m_colorVectors = gameObject.AddComponent<ColorVectors>();
    }

    public void GetColorsForVector(CVDTypeData cvdType, out Color target, out Color background) //Vec steht gerade für die currentDeficiency
    {
        var bgVec = cvdType.GetBackgroundColor();
        background = new Color(bgVec.x, bgVec.y, bgVec.z);
        var colorVec = cvdType.GetRGBColor();
        target = new Color(colorVec.x, colorVec.y, colorVec.z);
    }

    //Ab hier Stuff, der seit 12.10. neu ist, um den Color Manager auch mal Colors managen zu lassen. 
    //Damit die CVDTypeData sich nicht mehr damit rumschlagen muss und einfach Data halten darf

    /*public void SetcurrentCVDVectors(CVDTypeData cvdType) //, out Color target, out Color background)
    {
        currentFC = cvdType.FieldChromaticity;
        currentCP = cvdType.gamutCP;
        currentCVD = cvdType;
    }*/

    // PlateManager Schritt 1: einmal Farbwerte ohne Luminanz holen und zwischenspeichern
    public void GetBaseChromas(CVDTypeData cvdType, out Vector2 target, out Vector2 background)
    {
        //Berechnung Target Chroma mit aktueller Desaturierung
        var uvAposthCP = VecxyToVecu_v_(cvdType.gamutCP);
        var colorSatuvAposth = SaturateU_V_(uvAposthCP, cvdType);
        var myuvAposthChroma = Vecu_v_Toxy(colorSatuvAposth);
        target = myuvAposthChroma;

        //Berechnung Background Chroma (braucht nicht saturiert werden)
        background = cvdType.FieldChromaticity;
    }

    // PlateManager Schritt 2: für jeden Punkt einen random Helligkeitswert hinzufügen
    public Vector3 AddRandomLuminance(Vector2 xyChroma)
    {
        var luminance = Random.Range(0.3f, 0.7f);
        return new Vector3(xyChroma.x, xyChroma.y, luminance);
    }

    //Schritt 3: Vector als RGB zum Einfärben ausgeben
    public Color ToColor(Vector3 xyY)
    {
        var linrgb = ColorXYYToLinRGB(xyY);
        var rgbVector = ColorToVector(linrgb);
        //Color outColor = new Color((float)linrgb.R, (float)linrgb.G, (float)linrgb.B);
        var outColor = new Color(rgbVector.x, rgbVector.y, rgbVector.z);
        return outColor;
    }

    #region Color Space Conversions
    private Vector2 VecxyToVecu_v_(Vector2 vec) //I mean the newer version with apostroph but I'm not allowed to write that
    {
        var x = vec.x;
        var y = vec.y;
        var u = 2f * x / (6f * y - x + 1.5f);
        var v = 4.5f * y / (6f * y - x + 1.5f);
        return new Vector2(u, v);
    }

    private Vector2 Vecu_v_Toxy(Vector2 vec)
    {
        var uAposth = vec.x;
        var vAposth = vec.y;
        var x = (9 * uAposth) / (6 * uAposth - 16 * vAposth + 12);
        var y = (4 * vAposth) / (6 * uAposth - 16 * vAposth + 12);
        return new Vector2(x, y);
    }

    public Vector2 SaturateU_V_(Vector2 uvCP, CVDTypeData cvd) //Saturation Vector based on u'v' chromaticity
    {
        var uvFC = VecxyToVecu_v_(cvd.FieldChromaticity);
        var uvChromaPath = uvCP - uvFC;
        var factor = cvd.Staircase();
        var uvDesat = uvFC + uvChromaPath * factor;
        return uvDesat;
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

    private LinearRGBColor ColorXYYToLinRGB(Vector3 xyYVec)
    {
        var xyY = new xyYColor(xyYVec.x, xyYVec.y, xyYVec.z);
        var rgbWorkingSpace = RGBWorkingSpaces.sRGB;

        //to LinearRGB
        var xyYToLinearRGB = new ConverterBuilder().FromxyY(rgbWorkingSpace.WhitePoint).ToLinearRGB(rgbWorkingSpace).Build();
        var outputLinRGB = xyYToLinearRGB.Convert(xyY);
        return outputLinRGB;
    }
    #endregion

    #region Color To Unity-Vector Conversions
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
    #endregion
}
