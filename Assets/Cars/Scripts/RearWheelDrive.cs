using UnityEngine;
using System.Collections;

#pragma warning disable RCS1222
#pragma warning disable CS0618
#pragma warning disable RCS1018
#pragma warning disable RCS1213
#pragma warning disable IDE0051
#pragma warning disable RCS1110
#pragma warning disable RCS1163
#pragma warning disable IDE0060

public enum Direction
{
    Up, Down, Left, Right, None
}

public class RearWheelDrive : MonoBehaviour
{
    private WheelCollider[] wheels;

    [SerializeField] private Vector2[] relationVelocityTorque;
    [SerializeField] private Vector2[] relationVelocityMaxAngle;
    [SerializeField] private Vector2[] relationVelocitySkidding;

    private Rigidbody rb;
    private Vector3 up;

    [HideInInspector] public int carID;
    [HideInInspector] public int color;
    [HideInInspector] public int lastCheckpoint;
    [HideInInspector] public int laps;
    [HideInInspector] public int racePosition;



    public void Awake()
    {
        wheels = GetComponentsInChildren<WheelCollider>();
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = Vector3.zero;

        if (relationVelocityMaxAngle.Length == 0)
        {
            Debug.LogError("No hay relación velocidad - angulo en el auto");
            Debug.Break();
        }

        laps = 0;
        lastCheckpoint = -1;
    }

    public void FixedUpdate()
    {
        float currentVelocity = rb.velocity.magnitude;

        float angle = Utils.getInterpolatedValueInVectors(relationVelocityMaxAngle, currentVelocity) * Input.GetAxis("Horizontal");
        float torque = Utils.getInterpolatedValueInVectors(relationVelocityTorque, currentVelocity) * Input.GetAxis("Vertical");

        //skiddingAngle = -angle * Utils.getInterpolatedValueInVectors(relationVelocitySkidding, currentVelocity);
        //transform.RotateAround(transform.up, (skiddingAngle - lastSkiddingAngle) * Mathf.Deg2Rad);
        // lastSkiddingAngle = skiddingAngle;

        foreach (WheelCollider wheel in wheels)
        {
            // wheel.steerAngle = -skiddingAngle + (wheel.transform.localPosition.z > 0 ? angle : 0f);
            if (wheel.transform.localPosition.z > 0)
                wheel.steerAngle = angle;

            if (wheel.transform.localPosition.z < 0)
                wheel.motorTorque = torque;

            // modificamos la rotación visual de las ruedas
            wheel.GetWorldPose(out Vector3 p, out Quaternion q);
            Transform shapeTransform = wheel.transform.GetChild(0);
            shapeTransform.position = p;
            shapeTransform.rotation = q;
        }

        up = rb.transform.up;
    }

    void OnCollisionEnter(Collision collision)
    {
        transform.RotateAround(up, Input.GetAxis("Vertical") * Input.GetAxis("Horizontal") * 0.02f);
        rb.angularVelocity = Vector3.zero;
    }

    void OnCollisionStay(Collision collision)
    {
        OnCollisionEnter(collision);
    }
}
