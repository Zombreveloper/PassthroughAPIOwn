/* maps the Machado matrices based on an interpolation value to a LUT and applies it to the Passthrough Layer
 * 
 * Code for Safe as PNG provided by: https://discussions.unity.com/t/how-to-save-a-texture2d-into-a-png/184699/4
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Colourful;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class MachadoSim : MonoBehaviour
{
    private OVRPassthroughLayer m_passthroughLayer;
    public MachadoValues machado;
    public Matrix4x4 MySimMatrix;


    public float activeSeverity;
    private string lutPath = "Assets/PassthroughCameraApiSamples/LUTs/DynamicCvdLut.asset";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        var passthrough = GameObject.Find("[BuildingBlock] Passthrough");
        m_passthroughLayer = passthrough.GetComponent<OVRPassthroughLayer>();
    }

    public void ProcessLUT(int typeIndex, float severity)
    {
        var currentType = GetCvdMatrices(typeIndex);
        var finalMatrix = GetMatrixBySeverity(currentType, severity);
        MySimMatrix = finalMatrix.GetMatrix(); //Only for debugging to see values in Inspector
        CreateLUTFromMatrix(finalMatrix.GetMatrix());

    }

    #region Personalized Sim Section
    //Personalized Methods bloats this class up with stuff it shouldn't be handling. 
    //Create additional Translator Class if I find time
    public void ProcessPersonalizedLUT(UserDataCVD user)
    {
        var maxCVDValue = Mathf.Max(user.ProtanScore, user.DeutanScore, user.TritanScore);
        var currentType = GetCvdMatricesByScore(maxCVDValue, user);

        activeSeverity = maxCVDValue;

        //Now divide by 12 to turn the Score with a scale of 120 to a range of 0 to 10 to coincide with my simulation range
        var cvdTableValue = maxCVDValue / 12f;
        var finalMatrix = GetMatrixBySeverity(currentType, cvdTableValue);
        MySimMatrix = finalMatrix.GetMatrix(); //Only for debugging to see values in Inspector
        CreateLUTFromMatrix(finalMatrix.GetMatrix());
    }

    private List<DeficiencyColorMatrixBase> GetCvdMatricesByScore(float highScore, UserDataCVD user)
    {
        if (highScore == user.ProtanScore)
            return machado.ProtanValues;
        else if (highScore == user.DeutanScore)
            return machado.DeutanValues;
        else if (highScore == user.TritanScore)
            return machado.TritanValues;
        else return machado.TritanValues;
    }
    #endregion

    public List<DeficiencyColorMatrixBase> GetCvdMatrices(int index)
    {

        switch (index)
        {
            case 0:
                return machado.ProtanValues;

            case 1:
                return machado.DeutanValues;

            case 2:
                return machado.TritanValues;

            default:
                Debug.LogWarning("Dropdown Menu got invalid values ");
                return null;
        }

    }

    private DeficiencyColorMatrixBase GetMatrixBySeverity(List<DeficiencyColorMatrixBase> cvdTypes, float severity)
    {
        //if severity outside accessible area get values from outmost possible matrix
        if (severity <= 0)
        { return cvdTypes[0]; }
        if (severity >= cvdTypes.Count - 1)
        { return cvdTypes.Last(); }

        //Linear interpolation between table values to make an aproximation of every possible severity
        for (int i = 0; i < cvdTypes.Count - 1; i++)
        {
            var a = cvdTypes[i];
            var b = cvdTypes[i + 1];
            if (severity >= i && severity <= i + 1)
            {
                float t = severity - i; //interpolation factor
                return InterpolateMatrix(a, b, t);
            }
        }
        //just to shut the compiler up
        Debug.LogWarning("No Value for your severity could be obtained. If this message shows up something went srsly wrong!");
        return new DeficiencyColorMatrixBase();
    }

    DeficiencyColorMatrixBase InterpolateMatrix(DeficiencyColorMatrixBase a, DeficiencyColorMatrixBase b, float t)
    {
        float[] resultMatrix = new float[9];

        for (int i = 0; i < a.matrix.Length; i++)
        {
            resultMatrix[i] = Mathf.Lerp(a.matrix[i], b.matrix[i], t);
        }
        var matrixObject = ScriptableObject.CreateInstance<DeficiencyColorMatrixBase>(); //new DeficiencyColorMatrixBase();
        matrixObject.matrix = resultMatrix;
        return matrixObject;
    }

    void CreateLUTFromMatrix(Matrix4x4 matrix)
    {
        const int resolution = 32; //Besser auf 32 setzen für volle 255 value Resolution
        Color[] lutColors = new Color[resolution * resolution * resolution];
        //Matrix4x4 matrix = DeficiencyMatrix.GetMatrix();

        for (int blue = 0; blue < resolution; blue++)
        {
            for (int green = 0; green < resolution; green++)
            {
                for (int red = 0; red < resolution; red++)
                {
                    int index = red + green * resolution + blue * resolution * resolution;

                    // Ursprüngliche Farbwerte im Bereich 0–1
                    float r = red / (float)(resolution - 1);
                    float g = green / (float)(resolution - 1);
                    float b = blue / (float)(resolution - 1);

                    /*
                    // Farbmatrix anwenden (aber nur die Beispielmatrix aus dieser Klasse)
                    float rOut = colorMatrix[0, 0] * r + colorMatrix[0, 1] * g + colorMatrix[0, 2] * b;
                    float gOut = colorMatrix[1, 0] * r + colorMatrix[1, 1] * g + colorMatrix[1, 2] * b;
                    float bOut = colorMatrix[2, 0] * r + colorMatrix[2, 1] * g + colorMatrix[2, 2] * b;
                    */

                    //DeficiencyMatrix anwenden
                    float rOut = matrix.m00 * r + matrix.m01 * g + matrix.m02 * b;
                    float gOut = matrix.m10 * r + matrix.m11 * g + matrix.m12 * b;
                    float bOut = matrix.m20 * r + matrix.m21 * g + matrix.m22 * b;

                    // Clampen auf 0–1
                    rOut = Mathf.Clamp01(rOut);
                    gOut = Mathf.Clamp01(gOut);
                    bOut = Mathf.Clamp01(bOut);

                    lutColors[index] = new Color(rOut, gOut, bOut);
                }
            }
        }

        CreateLutAsset(lutColors, resolution);
        var lut = new OVRPassthroughColorLut(lutColors, OVRPassthroughColorLut.ColorChannels.Rgb);
        m_passthroughLayer.SetColorLut(lut, 1);

        Debug.Log("Matrix-basierte LUT wurde angewendet.");
    }

    private void CreateLutAsset(Color[] colors, int res)
    {
        //var lut = new OVRPassthroughColorLut(colors, OVRPassthroughColorLut.ColorChannels.Rgb);
        Texture2D lutTex = new Texture2D(res * res, res, TextureFormat.RGB24, false, true);
        lutTex.wrapMode = TextureWrapMode.Clamp;
        lutTex.filterMode = FilterMode.Point;


        //Coloring the new Texture
        Debug.Log($"Die Dimensionen der Texture2D liegen bei Höhe: {lutTex.height} und Breite: {lutTex.width}. Das macht {lutTex.height * lutTex.width} Pixel.");
        Debug.Log($"Das ColorArray beinhaltet {colors.Length} Einträge.");
        //Debug.Log($"Jetzt gerade zählt der Index bis höchstens: {(lutTex.width - 1) + (lutTex.height - 1) * (lutTex.height - 1)}");

        int i = 0;
        for (int z = 0; z < res; z++)
        {
            for (int y = 0; y < lutTex.height; y++)
            {
                for (int x = 0; x < lutTex.width / res; x++)
                {
                    Color applyColor = colors[y + x * res];
                    lutTex.SetPixel(x + z * res, y, colors[i++]);
                }
            }
        }
        lutTex.Apply();

        SaveTextureAsPNG(lutTex);
        //AssetDatabase.CreateAsset(lutTex, lutPath);
    }

    private void SaveTextureAsPNG(Texture2D texture)
    {
        byte[] bytes = texture.EncodeToPNG();
        var dirPath = Application.dataPath + "/PassthroughCameraApiSamples/LUTs";
        if (!System.IO.Directory.Exists(dirPath))
        {
            System.IO.Directory.CreateDirectory(dirPath);
        }
        System.IO.File.WriteAllBytes(dirPath + "/ProfileCvdLut" + ".png", bytes); //Hier bald durch Profilnamen individualisieren!
        Debug.Log(bytes.Length / 1024 + "Kb was saved as: " + dirPath);

#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
    }
}
