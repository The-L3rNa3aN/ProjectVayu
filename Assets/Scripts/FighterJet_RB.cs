using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FighterJet_RB : MonoBehaviour
{
    private Rigidbody rb;

    [Header("Fighter Jet Stats")]
    [SerializeField] private float max_velocity = 2414f;
    [SerializeField] private float kerb_weight = 19700f;

    [Header("Vector-based jet rotation")]
    public Vector3 v_travel;                        // The travel vector that directs the gameobject's main velocity vector.
    public Vector3 v_roll;                          // Second vector that behaves like 'v_travel' but only facilitates rolling along the Z-axis.
    private Vector3 v_main;                         // The forward vector with a specified magnitude.
    private Vector3 v_lateral;                      // The lateral vector with a specified magnitude.
    private Vector3 v_fwdTorqueAxis;
    private Vector3 v_rgtTorqueAxis;

    public float f_travelVectorMagnitude = 10f;
    public float f_torqueMultiplier = 0.05f;
    private float f_fwdToTravelAngle;
    private float f_rgtToTravelAngle;

    [Header("Jet throttle")]
    public float f_throttleIncrement = 0.2f;
    public float f_maxThrust = 200f;
    [SerializeField] private float f_throttle;

    [Header("Chase-camera handling")]
    public GameObject cameraPlaceholder;
    private readonly float smoothTime = 0.1f;
    private Vector3 camRefVelocity = Vector3.zero;

    [Header("DEBUG")]
    [SerializeField] private bool b_visualizeVectors;
    private Vector3 v_forwardVisualizer;            // Visualizes the gameobject's forward (Z) vector.
    private Vector3 v_rightVisualizer;              // Visualizes the gameobject's right (X) vector.


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
        float t = Input.GetAxisRaw("Throttle");

        if (cameraPlaceholder /*&& cameraLookAtTarget*/) CameraUpdate();
        if (b_visualizeVectors) VisualizeVectors();

        // Throttle controls.
        f_throttle += f_throttleIncrement * t;
        if (f_throttle >= 5000f) f_throttle = 5000f;
        if (f_throttle <= 0f) f_throttle = 0f;
        f_throttle *= f_maxThrust;

        v_main = transform.forward * 10f;
        v_lateral = transform.up * 10f;

        v_travel = (transform.right * x + transform.up * y + transform.forward).normalized * f_travelVectorMagnitude;
        v_roll = (transform.right * z + transform.up).normalized * f_travelVectorMagnitude;

        Vector3 targetDir = (v_travel - v_main).normalized;
        v_fwdTorqueAxis = Vector3.Cross(transform.forward, targetDir);
        f_fwdToTravelAngle = Vector3.Angle(transform.forward, targetDir);

        Vector3 rollDir = (v_roll - v_lateral).normalized;
        v_rgtTorqueAxis = Vector3.Cross(transform.up, rollDir);
        f_rgtToTravelAngle = Vector3.Angle(transform.up, rollDir);
    }

    private void FixedUpdate()
    {
        // Throttle
        rb.AddForce(f_throttle * transform.forward);

        // Weight?
        //rb.AddForce(F_GRAVITY * Time.fixedDeltaTime * Time.fixedDeltaTime * Vector3.down, ForceMode.Force);

        // Rotating the jet along the X and Y axes for now. (TO DO: extra dampening for the torque forces.)
        rb.AddTorque(f_fwdToTravelAngle * f_torqueMultiplier * v_fwdTorqueAxis, ForceMode.Acceleration);

        // Rotating the jet on the Z axis to simulate roll.
        rb.AddTorque(f_rgtToTravelAngle * f_torqueMultiplier * v_rgtTorqueAxis, ForceMode.Acceleration);
    }

    private void CameraUpdate()
    {
        // Lerp camera position to the placeholder's.
        Vector3 _pos = Vector3.SmoothDamp(Camera.main.transform.position, cameraPlaceholder.transform.position, ref camRefVelocity, smoothTime);
        Quaternion _rot = cameraPlaceholder.transform.rotation;
        Camera.main.transform.SetPositionAndRotation(_pos, _rot);
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
}
