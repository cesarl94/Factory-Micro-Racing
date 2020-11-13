using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
	private Vector3 velocity;
	private Vector3 angularVelocity;

	private Bodywork bodywork;
	//Las ruedas se ordenan según el reloj. FR, BR, BL, FL
	private Wheel[] wheels;

	private float direction;
	private float gravityInfluence;

	void Awake() {
		this.bodywork = this.GetComponentInChildren<Bodywork>();
		this.wheels = new Wheel[4];
		foreach (Transform child in this.transform) {
			string wheelID = Utils.getValueInName(child.name, "Wheel");
			switch(wheelID){
				case "FR": this.wheels[0] = child.GetComponentInChildren<Wheel>(); break;
				case "BR": this.wheels[1] = child.GetComponentInChildren<Wheel>(); break;
				case "BL": this.wheels[2] = child.GetComponentInChildren<Wheel>(); break;
				case "FL": this.wheels[3] = child.GetComponentInChildren<Wheel>(); break;
			}
		}

		this.direction = 30;

		for (int i = 0; i < 4;i++) {
			this.wheels[i].initialize(i);
			
		}
	}

    void FixedUpdate() {

		this.velocity += Physics.gravity * Time.fixedDeltaTime * 0.01f;
		this.transform.position += this.velocity * Time.fixedDeltaTime;

		
		foreach(Wheel wheel in this.wheels) {
			if(wheel.checkGround()){
				//this.velocity *= 0.75f;
			}
		}

		Vector3 frontCenter = (this.wheels[0].getWheelPosition() + this.wheels[3].getWheelPosition()) / 2f;
		Vector3 backCenter = (this.wheels[1].getWheelPosition() + this.wheels[2].getWheelPosition()) / 2f;
		Vector3 toFront = (frontCenter - backCenter).normalized;

		Vector3 frontToRight = (this.wheels[0].getWheelPosition() - frontCenter).normalized;
		Vector3 backToRight = (this.wheels[1].getWheelPosition() - backCenter).normalized;
		Vector3 averageRight = (frontToRight - backToRight) / Mathf.Sqrt(2f);

		Vector3 carUp = Vector3.Cross(toFront, averageRight);
		//this.transform.position = (frontCenter + backCenter) / 2f;
		this.bodywork.transform.rotation = Quaternion.LookRotation(toFront, carUp);

		Vector3 movements = Vector3.zero;

		for(int i=0;i<4;i++){
			Vector3 wheelPosition = this.wheels[i].getWheelPosition();
			Vector3 originPosition = this.wheels[i].getWheelOrigin();

			movements += wheelPosition - originPosition;
		}

		this.transform.position += movements / 4;
		


		//foreach(Wheel wheel in this.wheels){
		//	wheel.setRotation(this.direction);
		//}

		//Quaternion.LookRotation()
    }
}
