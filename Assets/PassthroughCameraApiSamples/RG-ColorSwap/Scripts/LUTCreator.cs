using UnityEngine;
using Meta.XR.Samples;

public class LUTCreator : MonoBehaviour
{
    private OVRPassthroughLayer m_passthroughLayer;
    private OVRPassthroughColorLut m_lutRGSwap;
    private OVRPassthroughColorLut m_lutRBSwap;

    //Temporary placeholder colors
    private Color m_red = new Color(1f, 0f, 0f);
    private Color m_green = new Color(0f, 1f, 0f);
    private Color m_blue = new Color(0f, 0f, 1f);

    #region Main Functions
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        //initialize placeholders
        //m_red = Color.red;
        m_green = Color.green;
        m_blue = Color.blue;

        var passthrough = GameObject.Find("[BuildingBlock] Passthrough");
        m_passthroughLayer = passthrough.GetComponent<OVRPassthroughLayer>();

    }

    // Update is called once per frame
    private void Update()
    {
        if (OVRInput.GetUp(OVRInput.Button.Two))
        {
            BuildLut("RedBlue");
            //m_lutRBSwap = new OVRPassthroughColorLut(new Color[] = BuildLut("RedBlue"), OVRPassthroughColorLut.ColorChannels.Rgb);
        }
    }
    #endregion

    #region LUT Builders

    //private Color[] rbLut = [m_red, m_green, m_blue ];
    private void BuildLut(string switchtype, int resolution = 16)
    {
        Color[] lut = new Color[resolution*resolution*resolution];
        //Debug.Log("the non filled Color lut is" , lut[0]);
        for(int blue = 0; blue < resolution; blue++)        
        {
            for (int green = 0; green < resolution; green++)
            {
                for (int red = 0; red < resolution; red++)
                {
                    int index = red + green * resolution + blue * resolution * resolution;
                    // Intensitätswerte (0–255)
                    float rVal = red / (float)(resolution - 1);
                    float gVal = green / (float)(resolution - 1);
                    float bVal = blue / (float)(resolution - 1);

                    //rot-blau tauschen und Aufbau im Array
                    lut[index] = new Color(rVal, gVal, bVal);
                }
            }
        }
        var colorLut = new OVRPassthroughColorLut(lut, OVRPassthroughColorLut.ColorChannels.Rgb);
        m_passthroughLayer.SetColorLut(colorLut);
    }


}
#endregion