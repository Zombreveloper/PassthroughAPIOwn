/* Datenstruktur, die alle für mich nötigen Werte aus mehreren Tabellen in Abhängigkeit der Wellenlänge speichert
 */

using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "OcularDataset", menuName = "Data/Ocular Dataset")]
public class OcularDataset : ScriptableObject
{
    public List<SpectralRow> rows = new List<SpectralRow>();
}


[System.Serializable]
public class SpectralRow
{
    public float Lambda_nm;
    //public float Wavenumber_cm_1;

    // Tabelle Stockman & Sharp
    public float LogL2;
    public float LogM2;
    public float LogS2;
    public float LogV2;
    public float LogL10;
    public float LogM10;
    public float LogS10;
    public float LoglOD;
    public float LogmOD;
    public float LogsOD;
    public float dLens;
    public float dMacular2;

    // Tabelle Low Density Spectral Photopigment Absorbances
    public float LogL;
    public float LogM;
    public float LogS;

    // Tabelle Combined Optical Density of Lens and other ocular media
    public float dOcul;
}