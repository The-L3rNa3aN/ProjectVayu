/* My attempt at writing the fighter jet movement code in the style of Quake (1997)'s player movement logic. */

using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerFighterJet : MonoBehaviour
{
    private Rigidbody rb;

    [Header("Vector-based jet rotation")]
    private Vector3 v_forwardVisualizer;            // Visualizes the gameobject's forward (Z) vector.
    private Vector3 v_targetDirection;              // The normalized direction between the forward vector and the travel vector.
    private Vector3 v_main;                         // The forward vector with a specified magnitude.
    public Vector3 v_travel;                        // The travel vector that directs the gameobject's main velocity vector.
    private Vector3 v_torqueAxis;

    public float f_travelVectorMagnitude = 10f;
    public float f_torqueMultiplier = 0.05f;
    private float f_fwdToTravelAngle;
    private float f_tempRoll;                       //Temporary variable for rolling. Must be deleted later.

    [Header("Jet throttle")]
    public float f_throttleIncrement = 0.2f;
    public float f_maxThrust = 200f;
    private float f_throttle;

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
        if (cameraPlaceholder && cameraLookAtTarget) CameraUpdate();

        // Throttle controlls.
        if (Input.GetKey(KeyCode.Keypad8)) f_throttle += f_throttleIncrement;
        if (Input.GetKey(KeyCode.Keypad5)) f_throttle -= f_throttleIncrement;
        Mathf.Clamp(f_throttle, 0f, 100f);              // Clamping values below 0f doesn't seem to work. Needs a fix!

        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        //float z = Input.GetAxis("Roll");
        f_tempRoll = Input.GetAxis("Roll");

        Debug.Log(f_tempRoll);

        v_main = transform.forward * 10f;

        /* NOTE FOR LINE 59
         * For some reason, multiplying 'transform.forward' with 'z' doesn't seem to work and sometimes yields unwanted results.
         * Because the main vector for the jet will be the Z axis, it's impossible to have the 'v_travel' vector to direct the
         * former vector in such a way that it can rotate the jet about its local Z axis.
         *
         * A second vector might be required in order to perform this. */
        v_travel = (transform.right * x + transform.up * y + transform.forward).normalized * f_travelVectorMagnitude;

        v_forwardVisualizer = transform.forward * 10f;

        v_targetDirection = (v_travel - v_main).normalized;
        v_torqueAxis = Vector3.Cross(transform.forward, v_targetDirection);
        f_fwdToTravelAngle = Vector3.Angle(transform.forward, v_targetDirection);

        // Output v_travel's magnitude in the editor. Can be commented.
        f_travelMag = v_travel.magnitude;

        // Output the angle between the forward vector and v_travel.
        //f_fwdToTravelAngle = Vector3.Angle(transform.forward, v_travel);

        Debug.DrawRay(transform.position, v_forwardVisualizer, Color.blue);
        Debug.DrawRay(transform.position, v_travel, Color.red);
    }

    private void FixedUpdate()
    {
        // Throttle
        rb.AddForce(f_throttle * f_maxThrust * transform.forward);

        // Rotating the jet along the X and Y axes for now. (TO DO: extra dampening for the torque forces.)
        rb.AddTorque(f_fwdToTravelAngle * f_torqueMultiplier * v_torqueAxis, ForceMode.Acceleration);

        // Rotating the jet on the Z axis to simulate roll.
        rb.AddTorque(f_tempRoll * transform.forward, ForceMode.Acceleration);
    }

    private void CameraUpdate()
    {
        // Lerp camera position to the placeholder's.
        Camera.main.transform.position = Vector3.SmoothDamp(Camera.main.transform.position, cameraPlaceholder.transform.position, ref camRefVelocity, smoothTime);

        // Lerp the camera's rotation on the player.
        Camera.main.transform.LookAt(cameraLookAtTarget);

        Camera.main.transform.rotation = cameraPlaceholder.transform.rotation;
    }
}