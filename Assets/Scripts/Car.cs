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
#pragma warning disable IDE1006
#pragma warning disable IDE0044

public enum Direction
{
    Up, Down, Left, Right, None
}

public class Car : MonoBehaviour
{
    private WheelCollider[] wheels;
    private Rigidbody rb;
    private Vector3 up;
    private float controlVertical;
    private float controlHorizontal;

    [SerializeField] private Vector2[] relationVelocityTorque;
    [SerializeField] private Vector2[] relationVelocityMaxAngle;
    [SerializeField] private Vector2[] relationVelocitySkidding;

    [HideInInspector] public Driver driver;
    [HideInInspector] public int color;

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
    }

    public void drive(float verticalControl, float horizontalControl)
    {
        controlHorizontal = horizontalControl;
        controlVertical = verticalControl;
    }

    public void FixedUpdate()
    {
        float currentVelocity = rb.velocity.magnitude;

        float angle = Utils.getInterpolatedValueInVectors(relationVelocityMaxAngle, currentVelocity) * controlHorizontal;
        float torque = Utils.getInterpolatedValueInVectors(relationVelocityTorque, currentVelocity) * controlVertical;

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
        transform.RotateAround(up, controlVertical * controlHorizontal * 0.02f);
        rb.angularVelocity = Vector3.zero;
    }

    void OnCollisionStay(Collision collision)
    {
        OnCollisionEnter(collision);
    }
}
