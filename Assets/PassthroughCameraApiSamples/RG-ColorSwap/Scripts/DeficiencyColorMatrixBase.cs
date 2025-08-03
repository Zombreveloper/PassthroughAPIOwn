using UnityEngine;

[CreateAssetMenu(fileName = "DeficiencyColorMatrixBase", menuName = "Scriptable Objects/DeficiencyColorMatrixBase")]
public class DeficiencyColorMatrixBase : ScriptableObject
{
    [Tooltip("3x3 Farbmatrix (3 Zeilen à 3 Spalten)")]
    public float[] matrix = new float[9]; // row-major (m00, m01, m02, m10,...)

    public Matrix4x4 GetMatrix()
    {
        var m = new Matrix4x4();
        m.m00 = matrix[0]; m.m01 = matrix[1]; m.m02 = matrix[2];
        m.m10 = matrix[3]; m.m11 = matrix[4]; m.m12 = matrix[5];
        m.m20 = matrix[6]; m.m21 = matrix[7]; m.m22 = matrix[8];
        return m;
    }
}
