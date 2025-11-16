using CCTEvent;
using SimInputs;
using UnityEngine;
using UnityEngine.Events;
using static Oculus.Interaction.Context;

public class SimControls : MonoBehaviour
{

    public static SimControls Instance;
    // Tooltip Members

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetUp(OVRInput.Button.Three)) //The X-Button
        {
            //Toggle Simulation On/Off
            SimInputEventManager.Input.OnXButton.Invoke();
        }
        if (OVRInput.GetUp(OVRInput.Button.Four)) //The Y-Button
        {
            //Toggle active Profile Info visible/hidden


            SimInputEventManager.Input.OnYButton.Invoke();
        }

        /*if (ShowSimulationTooltip)
        {
            UpdateTooltipPosition();

            //Write content into tooltip
            if (SaveManager.Instance.currentUser != null)
            {
                var infotext = m_tooltip.GetComponent<TextMesh>().text;
                infotext = $"Your active Profile is: {SaveManager.Instance.currentUser} \n";
            }
        }*/
    }
}

namespace SimInputs
{
    public static class SimInputEventManager
    {
        public static readonly SimInputEvents Input = new SimInputEvents();
        public class SimInputEvents
        {
            //In meinem Fall will ich folgende Inputs haben: 
            // - Toggle Simulation on/off
            // - Toggle active Profile Info visible/hidden
            public UnityAction OnXButton;
            public UnityAction OnYButton;
        }
    }
}
