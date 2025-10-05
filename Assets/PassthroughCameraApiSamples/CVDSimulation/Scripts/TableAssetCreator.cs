using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json; // JSON.NET für Unity importieren
using UnityEditor;
using UnityEngine;

public class TableAssetCreator
{
    [MenuItem("Tools/Import Table JSON")]
    public static void ImportTable()
    {
        // Dialog für Dateiauswahl
        string path = EditorUtility.OpenFilePanel("Import Table JSON", Application.dataPath, "json");
        if (string.IsNullOrEmpty(path)) return;

        // JSON laden
        string json = File.ReadAllText(path);
        var rows = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(json);

        // Neues ScriptableObject erzeugen
        TableImportSO asset = ScriptableObject.CreateInstance<TableImportSO>();
        asset.rows = new List<TableRows>();

        foreach (var row in rows)
        {
            TableRows tr = new TableRows();

            // Werte sicher konvertieren
            tr.Lambda_nm = TryGetInt(row, "Lambda_nm");
            tr.LogL2 = TryGetFloat(row, "LogL2");
            tr.LogM2 = TryGetFloat(row, "LogM2");
            tr.LogS2 = TryGetFloat(row, "LogS2");
            tr.LogV2 = TryGetFloat(row, "LogV2");
            tr.LogL10 = TryGetFloat(row, "LogL10");
            tr.LogM10 = TryGetFloat(row, "LogM10");
            tr.LogS10 = TryGetFloat(row, "LogS10");
            tr.LoglOD = TryGetFloat(row, "LoglOD");
            tr.LogmOD = TryGetFloat(row, "LogmOD");
            tr.LogsOD = TryGetFloat(row, "LogsOD");
            tr.dLens = TryGetFloat(row, "dLens");
            tr.dMacular2 = TryGetFloat(row, "dMacular2");

            asset.rows.Add(tr);
        }

        // Asset speichern
        string assetPath = "Assets/Data/TableData.asset";
        Directory.CreateDirectory("Assets/Data");
        AssetDatabase.CreateAsset(asset, assetPath);
        AssetDatabase.SaveAssets();

        EditorUtility.DisplayDialog("Import erfolgreich", $"Tabelle importiert nach {assetPath}", "OK");
    }

    private static float TryGetFloat(Dictionary<string, object> dict, string key)
    {
        if (dict.ContainsKey(key) && dict[key] != null)
        {
            float.TryParse(dict[key].ToString(), out float val);
            return val;
        }
        return 0f;
    }

    private static int TryGetInt(Dictionary<string, object> dict, string key)
    {
        if (dict.ContainsKey(key) && dict[key] != null)
        {
            int.TryParse(dict[key].ToString(), out int val);
            return val;
        }
        return 0;
    }
}