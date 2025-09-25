using UnityEngine;

[CreateAssetMenu(fileName = "ProtanSO", menuName = "Scriptable Objects/CVD/ProtanSO")]
public class ProtanSO : CVDTypeData
{
    public override string Name => "Protan";

    public override Vector2 CopunctPoint => new Vector2(0.747f, 0.253f);
    
}
