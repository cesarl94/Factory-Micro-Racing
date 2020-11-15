using UnityEngine;
using System.Collections;

public class RearWheelDrive : MonoBehaviour {

	private WheelCollider[] wheels;

	public float maxAngle = 30;
	public float maxTorque = 300;

	// here we find all the WheelColliders down in the hierarchy
	public void Start() {
		wheels = GetComponentsInChildren<WheelCollider>();
		Rigidbody rb = GetComponent<Rigidbody>();
		rb.centerOfMass = Vector3.zero ;	
	}


	// this is a really simple approach to updating wheels
	// here we simulate a rear wheel drive car and assume that the car is perfectly symmetric at local zero
	// this helps us to figure our which wheels are front ones and which are rear
	public void Update() {
		float angle = maxAngle * Input.GetAxis("Horizontal");
		float torque = maxTorque * Input.GetAxis("Vertical");

		foreach (WheelCollider wheel in wheels) {
			// a simple car where front wheels steer while rear ones drive
			if (wheel.transform.localPosition.z > 0)
				wheel.steerAngle = angle;

			if (wheel.transform.localPosition.z < 0)
				wheel.motorTorque = torque;

			// update visual wheels
			Quaternion q;
			Vector3 p;
			wheel.GetWorldPose (out p, out q);

			// assume that the only child of the wheelcollider is the wheel shape
			Transform shapeTransform = wheel.transform.GetChild (0);
			shapeTransform.position = p;
			shapeTransform.rotation = q;
		}
	}
}
