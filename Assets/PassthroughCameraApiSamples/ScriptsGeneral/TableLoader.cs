/* My first (and ChatGPT driven) attempt to import JSON Table Values. 
 * deprecated because this doesn't save the data as asset so the values would be read and created as tables everytime.
 * TableImportSO saves table as SO Asset instead.
 */

/*using System.Collections.Generic;
using UnityEngine;

// 1. Datenmodell
[System.Serializable]
public class TableRow
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

[System.Serializable]
public class TableWrapper
{
    public List<TableRow> rows;
}

// 2. Loader-Script für Unity
public class TableLoader : MonoBehaviour
{
    public TextAsset jsonFile;

    void Start()
    {
        // Da wir ein Array exportieren, packen wir es kurz in ein Wrapper-Objekt
        string wrappedJson = "{\"rows\":" + jsonFile.text + "}";
        TableWrapper table = JsonUtility.FromJson<TableWrapper>(wrappedJson);

        Debug.Log("Erste Zeile Index = " + table.rows[0].Lambda_nm);
        Debug.Log("LogL2 in Zeile 1 = " + table.rows[0].LogL2);
        Debug.Log("LoglOD für Wellenlänge" + table.rows[29].Lambda_nm + " = " + table.rows[29].LoglOD);
    }
} */