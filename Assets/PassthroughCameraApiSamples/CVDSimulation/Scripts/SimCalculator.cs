/* Class that manages all calculations for new RGB-Values for CVD-Simulation
 * At the Moment only for L-Cone. The others follow
 */
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SimCalculator : MonoBehaviour
{
    [SerializeField] private OcularDataset m_oculData;
    private OcularDataset m_shiftedData;
    private Sensitivities m_sensitivities;

    private float m_coneshiftL = 500f;
    private float m_coneshiftM = 500f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //wichtig zum Initiieren und weil viele Daten gleich bleiben und alle andern im Prozess überschrieben werden.
        //m_shiftedData = m_oculData;
        //oder auch nicht. Ich nehme eine leere Klasse und lasse die 5nm-Steps erstmal raus zugunsten genauerer Werte
        m_shiftedData = ScriptableObject.CreateInstance<OcularDataset>();

        Debug.Log("Die Startwellenlänge vom Index 13 ist: " + m_oculData.rows[13].Lambda_nm);
        //var wavenumConeL = ConeAsWavenumber();
        //wavenumConeL = ShiftConeByWavenumber(wavenumConeL, m_coneshiftL);
        //ToWavelength(wavenumConeL);

        //Test: Wird der richtige Wert abgehoben?
        var myLCone = m_oculData.GetValueByWavelength(455f, r => r.LogL2);
        Debug.Log("Wavelength 455 has an LogL Value of: " + myLCone);

        //Test: Kann ich auf diese Art verschobene Zapfen abrufen?
        //var myWavenum = m_oculData.GetValueByWavelength(455f, r => r.Wavenumber_cm_1);
        //var myShift = myWavenum - m_coneshiftL; // (v - delta v) aus Yaguchi Formel (1)
        //var shiftedLCone = m_oculData.GetValueByWavenumber(myShift, r => r.LogL2);
        ShiftCone();
        var testRow = m_shiftedData.rows[13];
        Debug.Log("Shifted LCone has now sensibility of "
            + testRow.LogL2 + " at Wavelength " + testRow.Lambda_nm + " and Wavenumber " + testRow.Wavenumber_cm_1);

    }

    //Hier im nächsten Schritt Argument hinzufügen, um dynamisch jeden Cone abgreifen zu können
    //ODER ich verändere direkt alle drei Cones und schmeiße auch alle drei Shifts rein. Ich glaube, das macht mehr Sinn.
    //Builds new Dataset for shifted Cone sensitivities
    void ShiftCone()
    {
        var shiftrows = m_shiftedData.rows;
        foreach (var row in m_oculData.rows)
        {
            var sensLStandard = row.LogL2;
            var v = row.Wavenumber_cm_1;
            var vShifted = v - m_coneshiftL; // (v - delta v) aus Yaguchi Formel (1)

            var sRow = new SpectralRow
            {
                LogL2 = sensLStandard,
                Wavenumber_cm_1 = vShifted,
                Lambda_nm = m_shiftedData.AsWavelength(vShifted)
            };
            shiftrows.Add(sRow);

        }
    }

    #region legacy Code
    private Sensitivities ConeAsWavenumber()
    {
        List<decimal> wavenumbers = new List<decimal>();
        List<float> coneSensitivities = new List<float>();

        //conversion factor nm to cm^-1
        var centimeter = (decimal)Mathf.Pow(10, 7);

        foreach (var item in m_oculData.rows)
        {
            wavenumbers.Add(centimeter / (decimal)item.Lambda_nm);
            coneSensitivities.Add(item.LogL2);
        }
        Sensitivities sensitivities = new Sensitivities(wavenumbers, coneSensitivities);
        Debug.Log("Die Wavenumber von Index 13 ist: " + wavenumbers[13]);
        return sensitivities;
    }

    private Sensitivities ShiftConeByWavenumber(Sensitivities sensitivity, float shift)
    {
        for (int i = 0; i < sensitivity.Wavenumbers.Count; i++)
        {
            sensitivity.Wavenumbers[i] += (decimal)shift;
        }
        return sensitivity;
    }

    private void ToWavelength(Sensitivities sens)
    {
        List<float> wavelengths = new List<float>();
        var nanometer = (decimal)Mathf.Pow(10, 7);

        if (sens.Wavelengths != null)
        {
            sens.Wavelengths.Clear();
        }

        foreach (var item in sens.Wavenumbers)
        {
            //wavelengths.Add(nanometer / item);
            var value = nanometer / item;
            wavelengths.Add((float)value);
        }
        sens.Wavelengths = wavelengths;

        Debug.Log("Die zurücktransformierte Wellenlänge von Index 13 ist: " + sens.Wavelengths[13]);
    }
    #endregion
}

public class Sensitivities
{
    public List<decimal> Wavenumbers;
    public List<float> WavenumFloats;
    public List<float> ConeSensitivities;
    public List<float> Wavelengths;

    public Sensitivities(List<decimal> wavenumbers, List<float> coneSensitivities)
    {
        this.Wavenumbers = wavenumbers;
        this.ConeSensitivities = coneSensitivities;
    }
}

//Class to temporarily store shifted Cone sensitivities without fitting them into the 5nm Wavelength steps
//Maybe useless bc I should use an empty Ocular Dataset to store additional values
public class SensShifted
{
    public List<float> Wavenumbers = new List<float>();
    public List<float> ConeSensitivities = new List<float>();

    /*public SensShifted(List<float> wavenumbers, List<float> coneSensitivities)
    {
        this.Wavenumbers = wavenumbers;
        this.ConeSensitivities = coneSensitivities;
    }*/
}
