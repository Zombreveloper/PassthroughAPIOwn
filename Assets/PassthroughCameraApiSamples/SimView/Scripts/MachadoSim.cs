/* maps the Machado matrices based on an interpolation value to a LUT and applies it to the Passthrough Layer
 */
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MachadoSim : MonoBehaviour
{
    private OVRPassthroughLayer m_passthroughLayer;
    public MachadoValues machado;
    public Matrix4x4 MySimMatrix;

    public Slider severSlider;
    public TMP_Dropdown TypeSelect;

    public float activeSeverity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
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
        const int resolution = 16;
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

        var lut = new OVRPassthroughColorLut(lutColors, OVRPassthroughColorLut.ColorChannels.Rgb);
        //lut.SetLut(lutColors);
        m_passthroughLayer.SetColorLut(lut, 1);

        Debug.Log("Matrix-basierte LUT wurde angewendet.");
    }


}
