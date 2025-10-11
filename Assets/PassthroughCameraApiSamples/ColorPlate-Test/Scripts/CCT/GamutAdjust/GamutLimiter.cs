/* Class that limits the length of the Confusion Line Vectors to the limits of sRGB Colorspace
 * not meant to be used during runtime. Save output Vectors as clamped Vectors in extensions of CVDTypeData
 * make sure to recalculate if considering Output Gamut changes
 */

using Colourful;
using UnityEngine;

public static class GamutLimiter
{
    //Funktion, die von anderen Klassen gerufen werden kann
    //Warum ChatGPT meinte, ich bräuchte auch Limits in negativer Richtung, ist mir noch ein Rätsel
    public static (Vector2 minUV, Vector2 maxUV) ClampConfusionLineToSRGB(
    //public static Vector2 ClampConfusionLineToSRGB(
        Vector2 originUV, Vector2 directionUV,
        float maxDistance = 1f,
        float tolerance = 1e-5f)
    {
        /*var converter = new ConverterBuilder()
            .From<CIExyY>()   // u'v' muss ggf. in xyY oder Lab konvertiert werden
            .To<RGBColor>()
            .Build();*/

        var rgbWorkingSpace = RGBWorkingSpaces.sRGB;
        var converter = new ConverterBuilder().FromxyY(rgbWorkingSpace.WhitePoint).ToRGB().Build();
        //Warum normalisieren? Der Vektor soll erstmal doch exakt der richtige Vektor sein? 
        //Oder damit er garantiert größer wird als der Farbraum?
        directionUV.Normalize();
        Debug.Log("normalisierter Vector bei: " + directionUV);

        // Prüfe Gamut in positiver und negativer Richtung
        float pos = FindMaxDistance(originUV, directionUV, converter, maxDistance, tolerance);
        float neg = FindMaxDistance(originUV, -directionUV, converter, maxDistance, tolerance);

        var minUV = originUV - directionUV * neg;
        var maxUV = originUV + directionUV * pos;

        Debug.Log("GamutLimits für XVektor sind: " + minUV + " und " + maxUV);
        return (minUV, maxUV);
        //return maxUV;
    }

    private static float FindMaxDistance(Vector2 origin, Vector2 dir,
        Colourful.IColorConverter<Colourful.xyYColor, Colourful.RGBColor> converter, float maxDist, float tolerance)
    {
        float low = 0, high = maxDist;
        while (high - low > tolerance)
        {
            float mid = (low + high) / 2f;
            var uv = origin + dir * mid;
            var xy = uv; //UVtoXY(uv); Ich bin dumm. Ich brauche die Umwandlung gar nicht. Die originalvektoren sind schon im xy-Space...  ABER KLEIN xy!
            //Überlegung wert, ob die Luminanz normalisiert werden muss (oder die Lib das selbst macht)
            var rgb = converter.Convert(new xyYColor(xy.x, xy.y, 1f));

            if (IsInGamut(rgb))
                low = mid;
            else
                high = mid;
        }
        return low;
    }

    private static Vector2 UVtoXY(Vector2 uv)
    {
        // CIE 1976 u′v′ → xy
        float denom = (9 * uv.y / (6 * uv.x - 16 * uv.y + 12));
        float x = (9 * uv.x) / (6 * uv.x - 16 * uv.y + 12);
        float y = (4 * uv.y) / (6 * uv.x - 16 * uv.y + 12);
        return new Vector2(x, y);
    }

    private static bool IsInGamut(RGBColor rgb)
        => rgb.R >= 0 && rgb.R <= 1 &&
           rgb.G >= 0 && rgb.G <= 1 &&
           rgb.B >= 0 && rgb.B <= 1;
}
