/* Calls Data for ColorRanges and calculates Color Output
 * TBH this is pretty superfluous by now 
 */

using UnityEngine;

public class ColorManager : MonoBehaviour
{
    //private ColorVectors m_colorVectors;

    private Color m_backgroundColor = new Color(0.5f, 0.5f, 0.5f);
    private Color m_targetColor = new Color(0.7f, 0.3f, 0.3f);


    private void Awake()
    {
        //m_colorVectors = gameObject.AddComponent<ColorVectors>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetColorsForVector(CVDTypeData cvdType, out Color target, out Color background) //Vec steht gerade für die currentDeficiency
    {
        var bgVec = cvdType.GetBackgroundColor();
        background = new Color(bgVec.x, bgVec.y, bgVec.z);
        var colorVec = cvdType.GetRGBColor();
        target = new Color(colorVec.x, colorVec.y, colorVec.z);
    }
}
