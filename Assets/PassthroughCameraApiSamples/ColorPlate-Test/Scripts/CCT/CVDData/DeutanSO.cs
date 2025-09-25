using UnityEngine;

[CreateAssetMenu(fileName = "DeutanSO", menuName = "Scriptable Objects/CVD/DeutanSO")]
public class DeutanSO : CVDTypeData
{
    public override string Name => "Deutan";

    public override Vector2 CopunctPoint => new Vector2(1.40f, -0.40f);
}
