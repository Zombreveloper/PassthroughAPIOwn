/* Datenstruktur, die alle für mich nötigen Werte aus mehreren Tabellen in Abhängigkeit der Wellenlänge speichert
 * Hinweis: evt Wavenumber wegen der hohen Anzahl an Nachkommastellen als double oder decimal speichern!
 * Macht allerdings die restlichen Funktionen bedeutend schwieriger... vllt Rundungsfehler nicht so schlimm.
 * Ansonsten wohl oder übel alle Werte als krassere Typen speichern.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "OcularDataset", menuName = "Data/Ocular Dataset")]
public class OcularDataset : ScriptableObject
{
    public List<SpectralRow> rows = new List<SpectralRow>();

    private void OnEnable()
    {
        CreateWavenumberValues();
    }

    private void CreateWavenumberValues()
    {
        //conversion factor nm to cm^-1
        var centimeter = Mathf.Pow(10, 7);

        foreach (var row in rows)
        {
            row.Wavenumber_cm_1 = centimeter / row.Lambda_nm;
        }
    }

    public float AsWavelength(float wavenum)
    {
        var nanometer = Mathf.Pow(10, 7);
        var value = nanometer / wavenum;
        return value;
    }

    public float GetValueByWavelength(float wavelength, Func<SpectralRow, float> type)
    {
        //if wavelength outside accessible area get values from outmost possible point
        if (wavelength <= rows.First().Lambda_nm)
        { return type(rows.First()); }
        if (wavelength >= rows.Last().Lambda_nm)
        { return type(rows.Last()); }

        //Linear interpolation between table values to make an aproximation of every point on spectrum
        for (int i = 0; i < rows.Count - 1; i++)
        {
            var a = rows[i];
            var b = rows[i + 1];
            if (wavelength >= a.Lambda_nm && wavelength <= b.Lambda_nm)
            {
                float t = (wavelength - a.Lambda_nm) / (b.Lambda_nm - a.Lambda_nm);
                return Mathf.Lerp(type(a), type(b), t);
            }
        }
        //just to shut the compiler up
        Debug.LogWarning("No Value for your wavelength could be obtained. If this message shows up something went srsly wrong!");
        return 0;
    }

    //Same as GetValueByWavelength but with reverse comparison checks bc Length antiproportional to Number
    public float GetValueByWavenumber(float wavenum, Func<SpectralRow, float> type)
    {
        //if wavelength outside accessible area get values from outmost possible point
        if (wavenum >= rows.First().Wavenumber_cm_1)
        { return type(rows.First()); }
        if (wavenum <= rows.Last().Wavenumber_cm_1)
        { return type(rows.Last()); }

        //Linear interpolation between table values to make an aproximation of every point on spectrum
        for (int i = 0; i < rows.Count - 1; i++)
        {
            var a = rows[i];
            var b = rows[i + 1];
            if (wavenum <= a.Wavenumber_cm_1 && wavenum >= b.Wavenumber_cm_1)
            {
                float t = (wavenum - a.Lambda_nm) / (b.Lambda_nm - a.Lambda_nm);
                return Mathf.Lerp(type(a), type(b), t);
            }
        }
        Debug.LogWarning("No Value for your wavenumber could be obtained. If this message shows up something went srsly wrong!");
        return 0;
    }

    //Not needed anymore but kept bc may be relevant later on. 
    //Needs to interpolate set values to fit 5nm wavelength steps
    public void SetValueByWavenumber(float wavenum, Func<SpectralRow, float> type)
    {
        if (wavenum >= rows.First().Wavenumber_cm_1 || wavenum <= rows.Last().Wavenumber_cm_1)
        {
            Debug.LogWarning("The value you are trying to write is out of bounds!");
            return;
        }

        for (int i = 0; i < rows.Count - 1; i++)
        {
            var a = rows[i];
            var b = rows[i + 1];
            if (wavenum <= a.Wavenumber_cm_1 && wavenum >= b.Wavenumber_cm_1)
            {
                float t = (wavenum - a.Lambda_nm) / (b.Lambda_nm - a.Lambda_nm);
                //return Mathf.Lerp(type(a), type(b), t);
            }
        }

    }

}


[System.Serializable]
public class SpectralRow
{
    public float Lambda_nm;
    public float Wavenumber_cm_1;

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