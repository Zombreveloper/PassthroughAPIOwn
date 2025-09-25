using UnityEngine;

[CreateAssetMenu(fileName = "TritanSO", menuName = "Scriptable Objects/CVD/TritanSO")]
public class TritanSO : CVDTypeData
{
    public override string Name => "Deutan";

    public override Vector2 CopunctPoint => new Vector2(1.40f, -0.40f);
}
