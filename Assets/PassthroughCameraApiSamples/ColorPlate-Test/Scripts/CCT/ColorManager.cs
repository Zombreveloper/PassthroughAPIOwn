/* Stores Data for ColorRanges and calculates Color Output
 */

using UnityEngine;
using CCT.VectorData;

public class ColorManager : MonoBehaviour
{
    private ColorVectors m_colorVectors;

    private Color m_backgroundColor = new Color(0.5f, 0.5f, 0.5f);
    private Color m_targetColor = new Color(0.7f, 0.3f, 0.3f);


    private void Awake()
    {
        m_colorVectors = gameObject.AddComponent<ColorVectors>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Wird jetzt von den ColorVectors geregelt
    public Color BackgroundColor
    {
        get { return m_backgroundColor; }
        set { m_backgroundColor = value; }
    }

    public Color TargetColor
    {
        get { return m_targetColor; }
        set { m_targetColor = value; }
    }

    public void GetColorsForVector(ColorVector vec, out Color target, out Color background) //Vec steht gerade für die currentDeficiency
    {
        //background = m_backgroundColor;
        var bgVec = m_colorVectors.GetBackgroundColor();
        background = new Color(bgVec.x, bgVec.y, bgVec.z);
        var colorVec = m_colorVectors.GetRGBColor(vec);
        target = new Color(colorVec.x, colorVec.y, colorVec.z);
    }

    public void ApplyColorVectors(ColorVector vector, float saturation, out Color target, out Color background)
    {
        background = m_backgroundColor;

        switch (vector)
        {
            case ColorVector.Protan:
                // Protan confusion line (affects L-cones, red-green axis)
                target = new Color(
                    m_backgroundColor.r + saturation * 0.5f,
                    m_backgroundColor.g - saturation * 0.3f,
                    m_backgroundColor.b,
                    1f
                );
                break;

            case ColorVector.Deutan:
                // Deutan confusion line (affects M-cones, red-green axis)
                target = new Color(
                    m_backgroundColor.r + saturation * 0.4f,
                    m_backgroundColor.g - saturation * 0.4f,
                    m_backgroundColor.b,
                    1f
                );
                break;

            case ColorVector.Tritan:
                // Tritan confusion line (affects S-cones, blue-yellow axis)
                target = new Color(
                    m_backgroundColor.r + saturation * 0.2f,
                    m_backgroundColor.g + saturation * 0.3f,
                    m_backgroundColor.b - saturation * 0.5f,
                    1f
                );
                break;

            default:
                target = Color.red;
                break;
        }

        // Clamp colors to valid range
        target = new Color(
            Mathf.Clamp01(target.r),
            Mathf.Clamp01(target.g),
            Mathf.Clamp01(target.b),
            1f
        );
    }
}
