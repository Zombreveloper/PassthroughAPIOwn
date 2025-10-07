/* Im Unity-Editor in der oberen Menüzeile unter "Tools -> Build Ocular Dataset" verwenden.
 * 
 * Baut aus den drei Tabellen von Stockman & Sharp, sowie aus CIE 170-1 einen gemeinsamen Datencontainer auf Basis der Wellenlänge.
 * Leider nicht dynamisch sondern exakt auf meine drei Tabellen zugeschnitten.
 * Tabellen liegen in: CVDSimulation/StaticData
 * Almost completely written by ChatGPT because JSON Parsing is killing me and I need to save time :(
 */

using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Globalization;
using Newtonsoft.Json.Linq;

public class OcularDatasetBuilder : EditorWindow
{
    [MenuItem("Tools/Build Ocular Dataset")]
    public static void BuildDataset()
    {
        // 🔹 Pfade zu deinen drei JSON-Dateien
        string table1Path = "Assets/PassthroughCameraApiSamples/CVDSimulation/StaticData/StockmanSharpeConeResponseTable.json";
        string table2Path = "Assets/PassthroughCameraApiSamples/CVDSimulation/StaticData/LowDensityAbsorbances.json";
        string table3Path = "Assets/PassthroughCameraApiSamples/CVDSimulation/StaticData/OpticalDensityOcul.json";

        // 🔹 JSON-Dateien laden
        var table1 = JArray.Parse(File.ReadAllText(table1Path));
        var table2 = JArray.Parse(File.ReadAllText(table2Path));
        var table3 = JArray.Parse(File.ReadAllText(table3Path));

        var dataset = ScriptableObject.CreateInstance<OcularDataset>();

        foreach (var entry1 in table1)
        {
            float? lambda1 = SafeGetFloat(entry1, "Lambda_nm");
            if (!lambda1.HasValue)
                continue;

            float lambda = lambda1.Value;
            var row = new SpectralRow();
            row.Lambda_nm = lambda;

            // ---------- Tabelle 1 Werte ----------
            row.LogL2 = SafeGetFloat(entry1, "LogL2") ?? 0f;
            row.LogM2 = SafeGetFloat(entry1, "LogM2") ?? 0f;
            row.LogS2 = SafeGetFloat(entry1, "LogS2") ?? 0f;
            row.LogV2 = SafeGetFloat(entry1, "LogV2") ?? 0f;
            row.LogL10 = SafeGetFloat(entry1, "LogL10") ?? 0f;
            row.LogM10 = SafeGetFloat(entry1, "LogM10") ?? 0f;
            row.LogS10 = SafeGetFloat(entry1, "LogS10") ?? 0f;
            row.LoglOD = SafeGetFloat(entry1, "LoglOD") ?? 0f;
            row.LogmOD = SafeGetFloat(entry1, "LogmOD") ?? 0f;
            row.LogsOD = SafeGetFloat(entry1, "LogsOD") ?? 0f;
            row.dLens = SafeGetFloat(entry1, "dLens") ?? 0f;
            row.dMacular2 = SafeGetFloat(entry1, "dMacular2") ?? 0f;

            // ---------- Tabelle 2: Werte mit Lambda-Abgleich ----------
            foreach (var entry2 in table2)
            {
                float? lambda2 = SafeGetFloat(entry2, "Lambda_nm");
                if (lambda2.HasValue && Mathf.Abs(lambda2.Value - lambda) < 0.01f)
                {
                    row.LogL = SafeGetFloat(entry2, "LogL") ?? 0f;
                    row.LogM = SafeGetFloat(entry2, "LogM") ?? 0f;
                    row.LogS = SafeGetFloat(entry2, "LogS") ?? 0f;
                    break;
                }
            }

            // ---------- Tabelle 3: Werte mit Lambda-Abgleich ----------
            foreach (var entry3 in table3)
            {
                float? lambda3 = SafeGetFloat(entry3, "Lambda_nm");
                if (lambda3.HasValue && Mathf.Abs(lambda3.Value - lambda) < 0.01f)
                {
                    row.dOcul = SafeGetFloat(entry3, "dOcul") ?? 0f;
                    break;
                }
            }

            dataset.rows.Add(row);
        }

        // 🔹 Asset speichern
        string outputPath = "Assets/Data/CombinedSpectrum.asset";
        AssetDatabase.CreateAsset(dataset, outputPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"✅ Combined spectral dataset created: {outputPath}");
    }

    // -------- Hilfsfunktion: sicheres Float-Parsing --------
    private static float? SafeGetFloat(JToken token, string key)
    {
        if (token == null)
            return null;

        var valueToken = token[key];
        if (valueToken == null || valueToken.Type == JTokenType.Null)
            return null;

        string raw = valueToken.ToString().Trim();
        if (string.IsNullOrWhiteSpace(raw) || raw == "-" || raw.Equals("null", StringComparison.OrdinalIgnoreCase))
            return null;

        // Ersetze Komma durch Punkt
        raw = raw.Replace(",", ".");

        if (float.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out float result))
            return result;

        return null;
    }
}



//Diese Version funktioniert beinahe, hat aber gerade Probleme bei LogS wegen fehlender Werte.
/*
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

public class OcularDatasetBuilder : EditorWindow
{
    private TextAsset jsonFile1;
    private TextAsset jsonFile2;
    private TextAsset jsonFile3;

    [MenuItem("Tools/Build Ocular Dataset")]
    public static void ShowWindow()
    {
        GetWindow<OcularDatasetBuilder>("Ocular Dataset Builder");
    }

    private void OnGUI()
    {
        GUILayout.Label("JSON-Dateien auswählen", EditorStyles.boldLabel);
        jsonFile1 = (TextAsset)EditorGUILayout.ObjectField("Tabelle Stockman", jsonFile1, typeof(TextAsset), false);
        jsonFile2 = (TextAsset)EditorGUILayout.ObjectField("Tabelle Low Density Absorbances", jsonFile2, typeof(TextAsset), false);
        jsonFile3 = (TextAsset)EditorGUILayout.ObjectField("Tabelle Optical Density Ocul", jsonFile3, typeof(TextAsset), false);

        if (GUILayout.Button("Spectral Dataset erstellen"))
        {
            if (jsonFile1 == null)
            {
                EditorUtility.DisplayDialog("Fehler", "Bitte mindestens Datei 1 auswählen.", "OK");
                return;
            }

            BuildDataset();
        }
    }

    private void BuildDataset()
    {
        var dataset = ScriptableObject.CreateInstance<OcularDataset>();
        dataset.rows = new List<SpectralRow>();

        // Hilfsmethode für sicheres Parsen
        float ParseValue(string s)
        {
            if (string.IsNullOrWhiteSpace(s) || s == "-" || s.ToLower() == "null") return float.NaN;
            s = s.Replace(",", "."); // Dezimaltrennzeichen korrigieren
            if (float.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out float val))
                return val;
            return float.NaN;
        }

        // JSON laden
        var table1 = JArray.Parse(jsonFile1.text);
        var table2 = jsonFile2 ? JArray.Parse(jsonFile2.text) : new JArray();
        var table3 = jsonFile3 ? JArray.Parse(jsonFile3.text) : new JArray();

        foreach (var entry1 in table1)
        {
            float lambda = entry1["Lambda_nm"]?.Value<float>() ?? float.NaN;
            if (float.IsNaN(lambda)) continue;

            var row = new SpectralRow();
            row.Lambda_nm = lambda;
            //row.Wavenumber_cm_1 = 1e7f / lambda;

            // Tabelle 1 Werte
            row.LogL2 = entry1["LogL2"]?.Value<float>() ?? float.NaN;
            row.LogM2 = entry1["LogM2"]?.Value<float>() ?? float.NaN;
            row.LogS2 = entry1["LogS2"]?.Value<float>() ?? float.NaN;
            row.LogV2 = entry1["LogV2"]?.Value<float>() ?? float.NaN;
            row.LogL10 = entry1["LogL10"]?.Value<float>() ?? float.NaN;
            row.LogM10 = entry1["LogM10"]?.Value<float>() ?? float.NaN;
            row.LogS10 = entry1["LogS10"]?.Value<float>() ?? float.NaN;
            row.LoglOD = entry1["LoglOD"]?.Value<float>() ?? float.NaN;
            row.LogmOD = entry1["LogmOD"]?.Value<float>() ?? float.NaN;
            row.LogsOD = entry1["LogsOD"]?.Value<float>() ?? float.NaN;
            row.dLens = entry1["dLens"]?.Value<float>() ?? float.NaN;
            row.dMacular2 = entry1["dMacular2"]?.Value<float>() ?? float.NaN;

            // Tabelle 2 Werte (gleiche Lambda suchen)
            foreach (var entry2 in table2)
            {
                if (Mathf.Abs(entry2["Lambda_nm"]?.Value<float>() ?? 0 - lambda) < 0.01f)
                {
                    row.LogL = ParseValue(entry2["LogL"]?.ToString());
                    row.LogM = ParseValue(entry2["LogM"]?.ToString());
                    row.LogS = ParseValue(entry2["LogS"]?.ToString());
                    break;
                }
            }

            // Tabelle 3 Werte
            foreach (var entry3 in table3)
            {

                float? lambda3 = SafeGetFloat(entry3, "Lambda_nm");
                if (lambda3.HasValue && Mathf.Abs(lambda3.Value - lambda) < 0.01f)
                {
                    var dOculValue = SafeGetFloat(entry3, "dOcul");
                    if (dOculValue.HasValue)
                        row.dOcul = dOculValue.Value;  // explizite Entnahme des float-Werts
                }
            }

            dataset.rows.Add(row);
        }

        string path = EditorUtility.SaveFilePanelInProject(
            "Speichere Ocular Dataset",
            "CombinedSpectrum",
            "asset",
            "Speicherort für das kombinierte okulare Spektrum auswählen"
        );

        if (!string.IsNullOrEmpty(path))
        {
            AssetDatabase.CreateAsset(dataset, path);
            AssetDatabase.SaveAssets();
            EditorUtility.DisplayDialog("Erfolg", "OcularDataset erfolgreich erstellt!", "OK");
        }
    }

    private static float? SafeGetFloat(JToken token, string key)
    {
        if (token == null)
            return null;

        var valueToken = token[key];
        if (valueToken == null || valueToken.Type == JTokenType.Null)
            return null;

        string raw = valueToken.ToString().Trim();
        if (string.IsNullOrWhiteSpace(raw) || raw == "-" || raw.Equals("null", StringComparison.OrdinalIgnoreCase))
            return null;

        // Ersetze Komma durch Punkt
        raw = raw.Replace(",", ".");

        if (float.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out float result))
            return result;

        return null;
    }
}*/
