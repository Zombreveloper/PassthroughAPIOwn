/* Used to toggle between LUT enabled and disabled.
 */

//using Meta.XR.BuildingBlocks;
//using Meta.XR.Samples;
using PassthroughCameraSamples.StartScene;
using UnityEngine;
//using static Oculus.Interaction.Samples.MRPassthrough;
//using ColorMapType = OVRPlugin.InsightPassthroughColorMapType;

public class LUTToggle : MonoBehaviour
{
    public bool ShowStartButtonTooltip = true;
    //[SerializeField] private GameObject m_ptLayer;
    private OVRPassthroughLayer m_passthroughLayer;
    private OVRPassthroughColorLut m_currentLut;

    [SerializeField] private GameObject m_tooltip;
    private const float FORWARDTOOLTIPOFFSET = -0.05f;
    private const float UPWARDTOOLTIPOFFSET = -0.003f;

    private void Start()
    {
        GameObject passthrough = GameObject.Find("[BuildingBlock] Passthrough");
        m_passthroughLayer = passthrough.GetComponent<OVRPassthroughLayer>();
        //m_currentLut = m_passthroughLayer.  maybe I don't need this anymore
    }
    private void Update()
    {
        if (OVRInput.GetUp(OVRInput.Button.One))
        {
            if (m_passthroughLayer == null)
            {
                Debug.LogError("OVRCameraRig does not contain an OVRPassthroughLayer component");
                return;
            }

            if (m_passthroughLayer.colorMapEditorType == OVRPassthroughLayer.ColorMapEditorType.ColorLut)
            {
                m_passthroughLayer.DisableColorMap();
            }
            else
            {
                m_passthroughLayer.colorMapEditorType = OVRPassthroughLayer.ColorMapEditorType.ColorLut;
            }

        }
        //Ab hier nur noch cooler Code um den Tooltip anzuzeigen. Hat nichts mit der eigentlichen Funktion zu tun. Und sollte hier vllt auch eigentlich gar nicht hin. Aber erstmal egal
        if (ShowStartButtonTooltip)
        {
            var finalRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch) *
                                Quaternion.Euler(45, 0, 0);
            var forwardOffsetPosition = finalRotation * Vector3.forward * FORWARDTOOLTIPOFFSET;
            var upwardOffsetPosition = finalRotation * Vector3.up * UPWARDTOOLTIPOFFSET;
            var finalPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch) +
                                forwardOffsetPosition + upwardOffsetPosition;
            m_tooltip.transform.rotation = finalRotation;
            m_tooltip.transform.position = finalPosition;
        }
    }

    private void ToggleStartButtonTooltip(bool shouldShowTooltip)
    {
        ShowStartButtonTooltip = shouldShowTooltip;
        m_tooltip.SetActive(ShowStartButtonTooltip);
    }
}

/* Original Code I derived from
// Copyright (c) Meta Platforms, Inc. and affiliates.
// Original Source code from Oculus Starter Samples (https://github.com/oculus-samples/Unity-StarterSamples)

using Meta.XR.Samples;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PassthroughCameraSamples.StartScene
{
    [MetaCodeSample("PassthroughCameraApiSamples-StartScene")]
    public class ReturnToStartScene : MonoBehaviour
    {
        public bool ShowStartButtonTooltip = true;
        private static ReturnToStartScene s_instance;
        [SerializeField] private GameObject m_tooltip;
        private const float FORWARDTOOLTIPOFFSET = -0.05f;
        private const float UPWARDTOOLTIPOFFSET = -0.003f;

        private void Awake()
        {
            if (s_instance == null)
            {
                s_instance = this;
                m_tooltip.SetActive(ShowStartButtonTooltip);
                DontDestroyOnLoad(gameObject);
            }
            else if (s_instance != this)
            {
                s_instance.ToggleStartButtonTooltip(
                    ShowStartButtonTooltip); // copy the setting from the new loaded scene
                Destroy(gameObject);
            }
        }

        private void Update()
        {
            // Since this is the Start scene, we can assume it's the first index in build settings
            if (OVRInput.GetUp(OVRInput.Button.Start) && SceneManager.GetActiveScene().buildIndex != 0)
            {
                SceneManager.LoadScene(0);
            }

            if (ShowStartButtonTooltip)
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
        }

        private void ToggleStartButtonTooltip(bool shouldShowTooltip)
        {
            ShowStartButtonTooltip = shouldShowTooltip;
            m_tooltip.SetActive(ShowStartButtonTooltip);
        }
    }
}
*/