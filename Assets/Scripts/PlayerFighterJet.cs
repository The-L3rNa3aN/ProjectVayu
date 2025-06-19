/* My attempt at writing the fighter jet movement code in the style of Quake (1997)'s player movement logic. */

using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerFighterJet : MonoBehaviour
{
    private Rigidbody rb;

    [Header("Vector-based jet rotation")]
    public Vector3 v_travel;                        // The travel vector that directs the gameobject's main velocity vector.
    public Vector3 v_roll;                          // Second vector that behaves like 'v_travel' but only facilitates rolling along the Z-axis.
    private Vector3 v_forwardVisualizer;            // Visualizes the gameobject's forward (Z) vector.
    private Vector3 v_rightVisualizer;              // Visualizes the gameobject's right (X) vector.
    private Vector3 v_targetDirection;              // The normalized direction between the forward vector and the travel vector.
    private Vector3 v_main;                         // The forward vector with a specified magnitude.
    private Vector3 v_lateral;                      // The lateral vector with a specified magnitude.
    private Vector3 v_fwdTorqueAxis;

    public float f_travelVectorMagnitude = 10f;
    public float f_torqueMultiplier = 0.05f;
    private float f_fwdToTravelAngle;

    private Vector3 v_rollDirection;
    private float f_rgtToTravelAngle;
    private Vector3 v_rgtTorqueAxis;

    [Header("Jet throttle")]
    public float f_throttleIncrement = 0.2f;
    public float f_maxThrust = 200f;
    [SerializeField] private float f_throttle;

    [Header("Chase-camera handling")]
    public GameObject cameraPlaceholder;
    public Transform cameraLookAtTarget;
    public float smoothTime = 0.3f;
    private Vector3 camRefVelocity = Vector3.zero;

    [Header("Debug values")]
    [SerializeField] private float f_travelMag;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // Inputs (old input manager for now).
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        float z = Input.GetAxis("Roll");
        float t = Input.GetAxis("Throttle");

        if (cameraPlaceholder && cameraLookAtTarget) CameraUpdate();
        VisualizeVectors();

        // Throttle controls.
        //if (Input.GetKey(KeyCode.Keypad8)) f_throttle += f_throttleIncrement;
        //if (Input.GetKey(KeyCode.Keypad5)) f_throttle -= f_throttleIncrement;
        f_throttle += f_throttleIncrement * t;
        //Mathf.Clamp(f_throttle, 0f, 500f);              // Clamping values below 0f doesn't seem to work. Needs a fix!
        if (f_throttle >= 5000f) f_throttle = 5000f;
        if (f_throttle <= 0f) f_throttle = 0f;

        v_main = transform.forward * 10f;
        v_lateral = transform.up * 10f;

        v_travel = (transform.right * x + transform.up * y + transform.forward).normalized * f_travelVectorMagnitude;
        v_roll = (transform.right * z + transform.up).normalized * f_travelVectorMagnitude;

        v_targetDirection = (v_travel - v_main).normalized;
        v_fwdTorqueAxis = Vector3.Cross(transform.forward, v_targetDirection);
        f_fwdToTravelAngle = Vector3.Angle(transform.forward, v_targetDirection);

        v_rollDirection = (v_roll - v_lateral).normalized;
        v_rgtTorqueAxis = Vector3.Cross(transform.up, v_rollDirection);
        f_rgtToTravelAngle = Vector3.Angle(transform.up, v_rollDirection);

        //f_fwdToTravelAngle = VectorAngle(v_fwdTorqueAxis, v_travel, v_main, transform.forward);

        // Output v_travel's magnitude in the editor. Can be commented.
        f_travelMag = v_travel.magnitude;

        // Output the angle between the forward vector and v_travel.
        //f_fwdToTravelAngle = Vector3.Angle(transform.forward, v_travel);
    }

    private void FixedUpdate()
    {
        // Throttle
        rb.AddForce(f_throttle * f_maxThrust * transform.forward);

        // Rotating the jet along the X and Y axes for now. (TO DO: extra dampening for the torque forces.)
        rb.AddTorque(f_fwdToTravelAngle * f_torqueMultiplier * v_fwdTorqueAxis, ForceMode.Acceleration);

        // Rotating the jet on the Z axis to simulate roll.
        rb.AddTorque(f_rgtToTravelAngle * f_torqueMultiplier * v_rgtTorqueAxis, ForceMode.Acceleration);
    }

    private void CameraUpdate()
    {
        // Lerp camera position to the placeholder's.
        Camera.main.transform.position = Vector3.SmoothDamp(Camera.main.transform.position, cameraPlaceholder.transform.position, ref camRefVelocity, smoothTime);

        // Lerp the camera's rotation on the player.
        Camera.main.transform.LookAt(cameraLookAtTarget);

        Camera.main.transform.rotation = cameraPlaceholder.transform.rotation;
    }

    private void VisualizeVectors()
    {
        v_forwardVisualizer = v_main;
        v_rightVisualizer = v_lateral;

        Debug.DrawRay(transform.position, v_forwardVisualizer, Color.blue);
        Debug.DrawRay(transform.position, v_rightVisualizer, Color.red);
        Debug.DrawRay(transform.position, v_travel, Color.magenta);
        Debug.DrawRay(transform.position, v_roll, Color.cyan);
    }

    private float VectorAngle(Vector3 torqueAxis, Vector3 action, Vector3 main, Vector3 dirAxis)
    {
        Vector3 vectorDir = (action - main).normalized;
        torqueAxis = Vector3.Cross(dirAxis, vectorDir);
        float angle = Vector3.Angle(dirAxis, vectorDir);
        return angle;
    }
}