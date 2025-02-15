using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CarController : MonoBehaviour
{
    // Wheel Colliders
    public WheelCollider frontLeftCollider;
    public WheelCollider frontRightCollider;
    public WheelCollider rearLeftCollider;
    public WheelCollider rearRightCollider;

    // Wheel Meshes
    public Transform frontLeftMesh;
    public Transform frontRightMesh;
    public Transform rearLeftMesh;
    public Transform rearRightMesh;

    // Input References
    public Joystick steeringJoystick;
    public Button accelerateButton;
    public Button brakeButton;
    public Button reverseButton;

    // Car parameters
    public float maxSteerAngle = 30f;
    public float motorForce = 1000f;
    public float brakeForce = 3000f;

    // Internal state
    private float currentBrakeForce = 0f;
    private float verticalInput = 0f;
    private bool isAccelerating = false;
    private bool isReversing = false;
    private bool isBraking = false;

    void Start()
    {
        // Setup button click handlers
        SetupButton(accelerateButton, OnAcceleratePressed, OnAccelerateReleased);
        SetupButton(reverseButton, OnReversePressed, OnReverseReleased);
        SetupButton(brakeButton, OnBrakePressed, OnBrakeReleased);
    }

    void Update()
    {
        HandleSteering();
        UpdateWheelMeshes();
    }

    void FixedUpdate()
    {
        HandleMotor();
        ApplyBraking();
    }

    private void SetupButton(Button button, UnityEngine.Events.UnityAction onPress, UnityEngine.Events.UnityAction onRelease)
    {
        // Add event triggers for press and release
        EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();

        // Press event
        var pointerDown = new EventTrigger.Entry();
        pointerDown.eventID = EventTriggerType.PointerDown;
        pointerDown.callback.AddListener((e) => onPress());
        trigger.triggers.Add(pointerDown);

        // Release event
        var pointerUp = new EventTrigger.Entry();
        pointerUp.eventID = EventTriggerType.PointerUp;
        pointerUp.callback.AddListener((e) => onRelease());
        trigger.triggers.Add(pointerUp);
    }

    public void OnAcceleratePressed() => isAccelerating = true;
    public void OnAccelerateReleased() => isAccelerating = false;
    public void OnReversePressed() => isReversing = true;
    public void OnReverseReleased() => isReversing = false;
    public void OnBrakePressed() => isBraking = true;
    public void OnBrakeReleased() => isBraking = false;

    void HandleMotor()
    {
        // Determine vertical input based on buttons
        verticalInput = 0f;
        if (isAccelerating) verticalInput = 1f;
        if (isReversing) verticalInput = -1f;

        // Apply motor force to rear wheels
        rearLeftCollider.motorTorque = verticalInput * motorForce;
        rearRightCollider.motorTorque = verticalInput * motorForce;
    }

    void HandleSteering()
    {
        // Get steering input from joystick
        float horizontalInput = steeringJoystick.Horizontal;
        float steerAngle = horizontalInput * maxSteerAngle;

        // Apply steering to front wheels
        frontLeftCollider.steerAngle = steerAngle;
        frontRightCollider.steerAngle = steerAngle;
    }

    void ApplyBraking()
    {
        // Set brake force based on brake button state
        currentBrakeForce = isBraking ? brakeForce : 0f;

        // Apply brakes to all wheels
        frontLeftCollider.brakeTorque = currentBrakeForce;
        frontRightCollider.brakeTorque = currentBrakeForce;
        rearLeftCollider.brakeTorque = currentBrakeForce;
        rearRightCollider.brakeTorque = currentBrakeForce;
    }

    void UpdateWheelMeshes()
    {
        UpdateWheelPose(frontLeftCollider, frontLeftMesh);
        UpdateWheelPose(frontRightCollider, frontRightMesh);
        UpdateWheelPose(rearLeftCollider, rearLeftMesh);
        UpdateWheelPose(rearRightCollider, rearRightMesh);
    }

    void UpdateWheelPose(WheelCollider collider, Transform mesh)
    {
        Vector3 pos;
        Quaternion rot;
        collider.GetWorldPose(out pos, out rot);
        mesh.position = pos;
        mesh.rotation = rot;
    }
}