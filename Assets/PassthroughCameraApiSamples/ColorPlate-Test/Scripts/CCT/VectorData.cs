using System.Security.Cryptography;
using UnityEngine;
using CCT.VectorData;

namespace CCT.VectorData
{
    public enum ColorVector
    {
        Protan,   // L-cone (red-green, affects long wavelength)
        Deutan,   // M-cone (red-green, affects medium wavelength)
        Tritan    // S-cone (blue-yellow, affects short wavelength)
    }

    //Das hat mal gar nicht funktioniert. Wird Durch CVDType Klasse ersetzt werden
    /*public class TestComponents
    {
        //private static readonly Random random = new Random();
        public static ColorVector GetRandomEntry<ColorVector>(ColorVector[] cvdTypes)
        {
            Debug.Log("Anzahl der CVD-Types im ColorVector: " + cvdTypes.Length);
            int randomIndex = Random.Range(0, cvdTypes.Length);
            return cvdTypes[randomIndex];
        }
    }*/

}
