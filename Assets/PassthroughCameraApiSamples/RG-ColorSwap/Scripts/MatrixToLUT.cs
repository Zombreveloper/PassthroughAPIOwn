using UnityEngine;
using System.Collections;

public class MatrixToLUT : MonoBehaviour
{
    //public OVRPassthroughLayer passthroughLayer;
    private OVRPassthroughLayer m_passthroughLayer;
    public DeficiencyColorMatrixBase DeficiencyMatrix;
    public DeficiencyList DList;

    private int dlIndex;

    private GameObject m_tooltip;

    // Beispielmatrix: vertauscht Rot und Grün
    private readonly float[,] colorMatrix = new float[3, 3]
    {
        { 0, 1, 0 },  // R' = G
        { 1, 0, 0 },  // G' = R
        { 0, 0, 1 }   // B' = B
    };

    private void Start()
    {
        var passthrough = GameObject.Find("[BuildingBlock] Passthrough");
        m_passthroughLayer = passthrough.GetComponent<OVRPassthroughLayer>();
        dlIndex = 0;

        m_tooltip = GameObject.Find("InputManager/Tooltip");
    }

    private void Update()
    {
        if (OVRInput.GetUp(OVRInput.Button.Two))
        {
            ShowcurrentMatrixName();
            Matrix4x4 currentDef = GetDeficiencyMatrix();
            CreateLUTFromMatrix(currentDef);
            
        }
    }

    void ShowcurrentMatrixName()
    {
        string myname = DList.VisionTypes[dlIndex].name;
        m_tooltip.GetComponent<TextMesh>().text = "You now see " + myname;
    }

    void UpdateListIndex()
    {
        if (dlIndex < DList.VisionTypes.Count - 1) { dlIndex++; }
        else { dlIndex = 0; }
    }

    Matrix4x4 GetDeficiencyMatrix()
    {
        Matrix4x4 currentMatrix = DList.VisionTypes[dlIndex].GetMatrix();
        UpdateListIndex();
        

            return currentMatrix;
    }

    [ContextMenu("Create LUT from Matrix")]
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
        m_passthroughLayer.SetColorLut(lut);
        //m_passthroughLayer.colorMapEditorUse = true; //existiert gar nicht du dummes GPT

        Debug.Log("Matrix-basierte LUT wurde angewendet.");
    }
}
