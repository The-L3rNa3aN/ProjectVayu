using UnityEngine;

public class PlayerFighterJet : MonoBehaviour
{
    [HideInInspector] public Vector3 v_main;
    /*[HideInInspector]*/ public Vector3 v_travel;

    public float f_travelVectorMagnitude = 10f;

    private void Start()
    {
        v_main = transform.forward * 10f;
        v_travel = transform.forward * 10f;
    }

    private void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        //Debug.Log("X: " + x + ", Y: " + z);

        v_travel.Normalize();
        v_travel.x = x * f_travelVectorMagnitude;
        //v_travel.y = y * f_travelVectorMagnitude;
        v_travel.z *= f_travelVectorMagnitude;

        //Debug.DrawRay(transform.position, v_main, Color.blue);
        Debug.DrawRay(transform.position, v_travel, Color.red);

        // Lerping the plane's vector with the travel vector.
        transform.rotation = Quaternion.FromToRotation(transform.forward, v_travel);
    }
}
