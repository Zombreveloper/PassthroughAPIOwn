using CCTEvent;
using UnityEngine;
using UnityEngine.Events;

public class CCTInputHandler : MonoBehaviour
{
    private bool stickNeutral = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (stickNeutral)
        {
            if (OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).x > 0.75f) //Thumbstick held to the right
            {
                CCTEventManager.Input.OnRStickRight.Invoke(0);
                stickNeutral = false;
            }

            if (OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).x < -0.75f) //Thumbstick held to the left
            {
                CCTEventManager.Input.OnRStickLeft.Invoke(2);
                stickNeutral = false;
            }

            if (OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).y > 0.75f) //Thumbstick held up
            {
                CCTEventManager.Input.OnRStickUp.Invoke(1);
                stickNeutral = false;
            }

            if (OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).y < -0.75f) //Thumbstick down
            {
                CCTEventManager.Input.OnRStickDown.Invoke(3);
                stickNeutral = false;
            }
        }
        StickBeenNeutral();
    }

    private void StickBeenNeutral() //Checks if the Thumbstick has been in Neutral Position between Event Triggers
    {
        if (OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick) == Vector2.zero)
            stickNeutral = true;
    }
}

namespace CCTEvent
{
    public static class CCTEventManager
    {
        public static readonly CCTInputEvents Input = new CCTInputEvents();
        public class CCTInputEvents
        {
            public UnityAction<int> OnRStickRight;
            public UnityAction<int> OnRStickLeft;
            public UnityAction<int> OnRStickUp;
            public UnityAction<int> OnRStickDown;
        }
    }
}
