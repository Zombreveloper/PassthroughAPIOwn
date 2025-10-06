//using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class SimCalculator : MonoBehaviour
{
    [SerializeField] private TableImportSO m_coneTable;
    private Sensitivities m_Sensitivities;

    private float m_coneshift = 500f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Die Startwellenlänge vom Index 13 ist: " + m_coneTable.rows[13].Lambda_nm);
        var wavenumConeL = ConeAsWavenumber();
        wavenumConeL = ShiftConeByWavenumber(wavenumConeL, m_coneshift);
        ToWavelength(wavenumConeL);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private Sensitivities ConeAsWavenumber()
    {
        List<decimal> wavenumbers = new List<decimal>();
        List<float> coneSensitivities = new List<float>();

        //conversion factor nm to cm^-1
        var centimeter = (decimal)Mathf.Pow(10, 7);

        foreach (var item in m_coneTable.rows)
        {
            wavenumbers.Add(centimeter / item.Lambda_nm);
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
}

public class Sensitivities
{
    public List<decimal> Wavenumbers;
    public List<float> ConeSensitivities;
    public List<float> Wavelengths;

    public Sensitivities(List<decimal> wavenumbers, List<float> coneSensitivities)
    {
        this.Wavenumbers = wavenumbers;
        this.ConeSensitivities= coneSensitivities;
    }
}
