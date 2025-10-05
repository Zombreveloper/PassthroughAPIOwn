//using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class SimCalculator : MonoBehaviour
{
    [SerializeField] private TableImportSO m_coneTable;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var wavenumConeL = ConeAsWavenumber();
        wavenumConeL = ShiftConeByWavenumber(wavenumConeL, -500f);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private Sensitivities ConeAsWavenumber()
    {
        List<float> wavenumbers = new List<float>();
        List<float> coneSensitivities = new List<float>();

        //conversion factor nm to cm^-1
        var centimeter = Mathf.Pow(10, 7);

        foreach (var item in m_coneTable.rows)
        {
            wavenumbers.Add(centimeter / item.Lambda_nm);
            coneSensitivities.Add(item.LogL2);
        }
        Sensitivities sensitivities = new Sensitivities(wavenumbers, coneSensitivities);
        return sensitivities;
    }

    private Sensitivities ShiftConeByWavenumber(Sensitivities sensitivity, float shift)
    {
        for (int i = 0; i < sensitivity.Wavenumbers.Count; i++)
        {
            sensitivity.Wavenumbers[i] -= shift;
        }
        return sensitivity;
    }
}

public class Sensitivities
{
    public List<float> Wavenumbers;
    public List<float> ConeSensitivities;

    public Sensitivities(List<float> wavenumbers, List<float> coneSensitivities)
    {
        this.Wavenumbers = wavenumbers;
        this.ConeSensitivities= coneSensitivities;
    }
}
