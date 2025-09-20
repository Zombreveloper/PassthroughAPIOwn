/* Stores Data for ColorRanges and calculates Color Output
 */

using UnityEngine;

public class ColorManager : MonoBehaviour
{
    private Color m_backgroundColor = new Color(0.5f, 0.5f, 0.5f);
    private Color m_foregroundColor = new Color(0.7f, 0.3f, 0.3f);
    public enum ColorVector
    {
        Protan,   // L-cone (red-green, affects long wavelength)
        Deutan,   // M-cone (red-green, affects medium wavelength)
        Tritan    // S-cone (blue-yellow, affects short wavelength)
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Color BackgroundColor
    {
        get { return m_backgroundColor; }
        set { m_backgroundColor = value; }
    }

    public Color ForegroundColor
    {
        get { return m_foregroundColor; }
        set { m_foregroundColor = value; }
    }

    void GetColorsForVector()
    {
        //Will apply the Colorvectors once they are part of this whole thing
    }
}
