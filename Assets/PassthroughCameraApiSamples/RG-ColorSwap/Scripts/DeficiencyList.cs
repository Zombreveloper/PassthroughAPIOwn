using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DeficiencyList", menuName = "Scriptable Objects/DeficiencyList")]


    public class DeficiencyList : ScriptableObject
    {
        [field: SerializeField] public List<DeficiencyColorMatrixBase> VisionTypes { get; private set; }
    }


