using SimInputs;
using UnityEngine;
using UnityEngine.SceneManagement;


public class TooltipWriter : MonoBehaviour
{
    private MachadoSim sim;

    [SerializeField] private bool showSimulationTooltip;
    [SerializeField] public GameObject m_tooltip;
    private const float FORWARDTOOLTIPOFFSET = -0.05f;
    private const float UPWARDTOOLTIPOFFSET = -0.003f;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SimInputEventManager.Input.OnYButton += ToggleTooltip;

    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SimInputEventManager.Input.OnYButton -= ToggleTooltip;

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        showSimulationTooltip = true;
        sim = GameObject.Find("SimManager").GetComponent<MachadoSim>();
    }

    // Update is called once per frame
    void Update()
    {
        if (showSimulationTooltip == true)
        {
            UpdateTooltipText();
            UpdateTooltipPosition();
        }
    }

    private void ToggleTooltip()
    {
        Debug.LogWarning("Hallo, der Y-Button ist hart gedrückt worden :D");

        if (showSimulationTooltip == true) { showSimulationTooltip = false; }
        else if (showSimulationTooltip == false) { showSimulationTooltip = true; }
        //showSimulationTooltip = show;
        m_tooltip.SetActive(showSimulationTooltip);
    }

    private void UpdateTooltipText()
    {
        Debug.LogWarning("Hallöchen, rufen wir Update Tooltext?");
        if (SaveManager.Instance.currentUser != null)
        {
            Debug.LogWarning("Hallöchen, haben wir einen current User?");
            var infotext = m_tooltip.GetComponent<TextMesh>();
            infotext.text = $"Your active Profile is: {SaveManager.Instance.currentUser.Name} \n";
            if (sim != null)
            {
                // isInC ? targetChroma : bgChroma;
                string state = sim.simActive ? "activated" : "deactivated";
                infotext.text += $"Simulation {state}";

            }
        }
    }

    private void UpdateTooltipPosition()
    {
        var finalRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.LTouch) *
                                Quaternion.Euler(45, 0, 0);
        var forwardOffsetPosition = finalRotation * Vector3.forward * FORWARDTOOLTIPOFFSET;
        var upwardOffsetPosition = finalRotation * Vector3.up * UPWARDTOOLTIPOFFSET;
        var finalPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch) +
                            forwardOffsetPosition + upwardOffsetPosition;
        m_tooltip.transform.rotation = finalRotation;
        m_tooltip.transform.position = finalPosition;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        sim = GameObject.Find("SimManager").GetComponent<MachadoSim>();
    }
}
