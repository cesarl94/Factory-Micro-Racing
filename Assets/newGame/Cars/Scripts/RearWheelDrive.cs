using UnityEngine;
using System.Collections;

public enum Direction
{
    Up, Down, Left, Right, None
}


public class RearWheelDrive : MonoBehaviour
{

    private WheelCollider[] wheels;



    public float maxAngle = 15;
    public float maxTorque = 300;
    public float skiddingAngle = 0;
    private float lastSkiddingAngle = 0;

    private Rigidbody rb;

    private Vector3 velocity;
    private Vector3 up;
    private float returnVelocity;
    private bool crashRecover;


    // here we find all the WheelColliders down in the hierarchy
    public void Start()
    {
        wheels = GetComponentsInChildren<WheelCollider>();
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = Vector3.zero;
        crashRecover = false;

    }

    private float getAngle()
    {
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.D))
        {
            return maxAngle;
        }
        else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.A))
        {
            return -maxAngle;
        }

        return 0f;
    }

    public void FixedUpdate()
    {
        float angle = maxAngle * Input.GetAxis("Horizontal");
        float torque = maxTorque * Input.GetAxis("Vertical");

        skiddingAngle = -angle / 2f;//Mathf.Abs(angle) < maxAngle * 0.5f ? 0 : maxAngle * Mathf.Sign(Input.GetAxis("Horizontal"));
        transform.RotateAround(transform.up, (skiddingAngle - lastSkiddingAngle) * Mathf.Deg2Rad);


        lastSkiddingAngle = skiddingAngle;


        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Break();
        }

        // if(!skidding){
        // 	if(Mathf.Abs(angle) > maxAngle * 0.3333f){
        // 		skidding = true;
        // 		transform.RotateAround(transform.up, maxAngle * 0.3333f * Mathf.Deg2Rad * Mathf.Sign(angle));
        // 		foreach (WheelCollider wheel in wheels){
        // 			wheel.transform.RotateAround(transform.up, maxAngle * -0.3333f * Mathf.Deg2Rad * Mathf.Sign(angle));
        // 		}
        // 	}

        // 	//if(Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.D)){
        // 	//}
        // 	//else if(Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.A)){
        // 	//	skidding = true;

        // 		//this.transform.RotateAroundLocal(Vector3.up, angle * Mathf.Deg2Rad);
        // 	//}
        // }
        // else{
        // 	if(Mathf.Abs(angle) < maxAngle * 0.3333f){
        // 		skidding = false;
        // 	}
        // }



        foreach (WheelCollider wheel in wheels)
        {
            // a simple car where front wheels steer while rear ones drive
            //if (wheel.transform.localPosition.z > 0)
            //	wheel.steerAngle = -skiddingAngle + angle;
            //else 
            wheel.steerAngle = -skiddingAngle + (wheel.transform.localPosition.z > 0 ? angle : 0f);


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
    }

    void OnCollisionEnter(Collision collision)
    {
        float velocityMagnitude = velocity.magnitude;
        Vector3 velocityNormalized = velocity / velocityMagnitude;

        float collisionDot = Vector3.Dot(velocityNormalized, collision.contacts[0].normal);
        float movementFactor = 1 - Mathf.Abs(collisionDot);

        Vector3 reflection = Vector3.Reflect(velocityNormalized, collision.contacts[0].normal);
        Vector3 newDirection = Vector3.Slerp(velocityNormalized, reflection, 0.3f * movementFactor);

        rb.position += reflection * 0.15f;
        rb.rotation = Quaternion.LookRotation(newDirection, up);
        rb.velocity = newDirection * velocityMagnitude * movementFactor;
        rb.angularVelocity = Vector3.zero;
    }
}
