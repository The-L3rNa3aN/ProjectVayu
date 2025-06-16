/* My attempt at writing the fighter jet movement code in the style of Quake (1997)'s player movement logic. */

using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerFighterJet : MonoBehaviour
{
    private Vector3 v_forwardVisualizer;            // Visualizes the gameobject's forward (Z) vector.
    private Vector3 v_targetDirection;              // The normalized direction between the forward vector and the travel vector.
    private Vector3 v_main;                         // The forward vector with a specified magnitude.
    public Vector3 v_travel;                        // The travel vector that directs the gameobject's main velocity vector.
    private Vector3 v_torqueAxis;

    public float f_travelVectorMagnitude = 10f;
    /*[Range(0f, 1f)]*/ public float f_torqueMultiplier = 0.05f;

    private Rigidbody rb;

    [Header("Chase-camera handling")]
    public GameObject cameraPlaceholder;
    public Transform cameraLookAtTarget;
    public float smoothTime = 0.3f;
    private Vector3 velocity = Vector3.zero;

    [Header("Debug values")]
    [SerializeField] private float f_travelMag;
    [SerializeField] private float f_fwdToTravelAngle;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if(cameraPlaceholder && cameraLookAtTarget) CameraUpdate();

        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        v_main = transform.forward * 10f;
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

        // Lerping the plane's vector with the travel vector. (NOPE)
        //transform.rotation = Quaternion.FromToRotation(transform.forward, v_travel);
    }

    private void FixedUpdate()
    {
        rb.AddTorque(v_torqueAxis * f_fwdToTravelAngle * f_torqueMultiplier, ForceMode.Acceleration);
    }

    private void CameraUpdate()
    {
        // Lerp camera position to the placeholder's.
        Camera.main.transform.position = Vector3.SmoothDamp(Camera.main.transform.position, cameraPlaceholder.transform.position, ref velocity, smoothTime);

        // Lerp the camera's rotation on the player.
        Camera.main.transform.LookAt(cameraLookAtTarget);

        Camera.main.transform.rotation = cameraPlaceholder.transform.rotation;
    }
}