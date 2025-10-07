/* Scriptable Object, welches eine Liste enthält. Die Liste repräsentiert die Zeile und der Inhalt
 * (repräsentiert in der zweiten Klasse) repräsentiert den Zeilenhinhalt thematisch sortiert.
 */
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TableImportSO", menuName = "Scriptable Objects/TableImportSO")]
public class TableImportSO : ScriptableObject
{
    public List<TableRows> rows;

    //Dictionary able to call table by wavelength
    private Dictionary<float, TableRows> lookupByWavelength;

    void OnEnable()
    {
        // Create Dictionary 
        lookupByWavelength = new Dictionary<float, TableRows>();

        foreach (var r in rows)
        {
            if (!lookupByWavelength.ContainsKey(r.Lambda_nm))
                lookupByWavelength.Add(r.Lambda_nm, r);
        }
    }

    public TableRows GetByWavelength(float wavelength_nm)
    {
        lookupByWavelength.TryGetValue(wavelength_nm, out TableRows row);
        return row;
    }
}

[System.Serializable]
public class TableRows
{
    public int Lambda_nm;
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
}
