/* SO that contains all precalculated matrices created by machado et al. 
 * values sourced from their website: https://www.inf.ufrgs.br/~oliveira/pubs_files/CVD_Simulation/CVD_Simulation.html
 * Attention: There are some known inaccuracies in their calculations that would need a recalculation to fix. 
 * These matrices are only used to skip implementation of these calculations for the moment.
*/
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MachadoValues", menuName = "Scriptable Objects/MachadoValues")]
public class MachadoValues : ScriptableObject
{
    /*[field: SerializeField] public List<float[,]> ProtanValues = new List<float[,]>();
    [field: SerializeField] public List<float[,]> DeutanValues = new List<float[,]>();
    [field: SerializeField] public List<float[,]> TritanValues = new List<float[,]>();*/

    [field: SerializeField] public List<DeficiencyColorMatrixBase> ProtanValues { get; private set; }
    [field: SerializeField] public List<DeficiencyColorMatrixBase> DeutanValues { get; private set; }
    [field: SerializeField] public List<DeficiencyColorMatrixBase> TritanValues { get; private set; }


    float[,,] values = new float[3, 3, 3];
}
