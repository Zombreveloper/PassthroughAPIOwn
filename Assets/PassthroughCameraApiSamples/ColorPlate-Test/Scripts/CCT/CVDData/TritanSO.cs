using UnityEngine;

[CreateAssetMenu(fileName = "TritanSO", menuName = "Scriptable Objects/CVD/TritanSO")]
public class TritanSO : CVDTypeData
{
    public override string Name => "Tritan";

    public override Vector2 CopunctPoint => new Vector2(0.171f, 0f);
}
