using UnityEngine;
using System.Collections;

public enum Direction
{
    Up, Down, Left, Right, None
}


public class RearWheelDrive : MonoBehaviour
{

    private WheelCollider[] wheels;

    [SerializeField]
    private Vector2[] relationVelocityTorque;
    [SerializeField]
    private Vector2[] relationVelocityMaxAngle;
    [SerializeField]
    private Vector2[] relationVelocitySkidding;

    private Rigidbody rb;
    private Vector3 velocity;
    private Vector3 forward;
    private Vector3 up;


    // here we find all the WheelColliders down in the hierarchy
    public void Start()
    {
        wheels = GetComponentsInChildren<WheelCollider>();
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = Vector3.zero;

        if (relationVelocityMaxAngle.Length == 0)
        {
            Debug.LogError("No hay relación velocidad - angulo en el auto");
            Debug.Break();
        }

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

            // update visual wheels
            Quaternion q;
            Vector3 p;
            wheel.GetWorldPose(out p, out q);

            // assume that the only child of the wheelcollider is the wheel shape
            Transform shapeTransform = wheel.transform.GetChild(0);
            shapeTransform.position = p;
            shapeTransform.rotation = q;
        }

        velocity = rb.velocity;
        up = rb.transform.up;
        forward = rb.transform.forward;
    }


    void OnCollisionEnter(Collision collision)
    {
        this.transform.RotateAround(up, Input.GetAxis("Vertical") * Input.GetAxis("Horizontal") * 0.02f);
        rb.angularVelocity = Vector3.zero;
    }

    void OnCollisionStay(Collision collision)
    {
        OnCollisionEnter(collision);
    }

    private float getMaxAngle(float velocity)
    {
        return Utils.getInterpolatedValueInVectors(relationVelocityMaxAngle, velocity);
    }
}
