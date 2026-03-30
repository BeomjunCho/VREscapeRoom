using UnityEngine;
using UnityEngine.XR;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    [Header("Inputs")]
    // Left controller
    [HideInInspector] public Vector2 leftStick; 
    [HideInInspector] public bool xButton;
    [HideInInspector] public bool yButton;
    [HideInInspector] public float leftTrigger; 
    [HideInInspector] public float leftGrip;
    [HideInInspector] public bool menu;

    // Right controller
    [HideInInspector] public Vector2 rightStick; 
    [HideInInspector] public bool aButton;
    [HideInInspector] public bool bButton;
    [HideInInspector] public float rightTrigger; 
    [HideInInspector] public float rightGrip;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
    }
    private void Update()
    {
        HandleInputs();
    }

    private void HandleInputs()
    {
        var leftController = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        var rightController = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        // Left controller
        leftController.TryGetFeatureValue(CommonUsages.primary2DAxis, out leftStick);
        leftController.TryGetFeatureValue(CommonUsages.primaryButton, out xButton);
        leftController.TryGetFeatureValue(CommonUsages.secondaryButton, out yButton);
        leftController.TryGetFeatureValue(CommonUsages.trigger, out leftTrigger);
        leftController.TryGetFeatureValue(CommonUsages.grip, out leftGrip);
        leftController.TryGetFeatureValue(CommonUsages.menuButton, out menu);

        // Right controller
        rightController.TryGetFeatureValue(CommonUsages.primary2DAxis, out rightStick);
        rightController.TryGetFeatureValue(CommonUsages.primaryButton, out aButton);
        rightController.TryGetFeatureValue(CommonUsages.secondaryButton, out bButton);
        rightController.TryGetFeatureValue(CommonUsages.trigger, out rightTrigger);
        rightController.TryGetFeatureValue(CommonUsages.grip, out rightGrip);
    }


    
}
