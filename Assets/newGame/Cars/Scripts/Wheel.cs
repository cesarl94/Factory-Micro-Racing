using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour {
	private float radius;
	private int wheelID;

	private Transform mesh;
	private Transform car;
	public bool isGrounded;

	public bool isFront {
		get {
			return (this.wheelID + 1) % 4 < 2;
		}
	}

	public bool isRight {
		get {
			return this.wheelID < 2;
		}
	}

    void Awake() {
		this.enabled = false;
		Transform floorNode = this.transform.GetChild(0);
		Transform origin = this.transform.parent;

		this.car = origin.parent;
		this.mesh = this.transform.GetComponentInChildren<MeshFilter>().transform;
		this.radius = Vector3.Magnitude(floorNode.position - this.transform.position);
		this.transform.SetParent(origin.parent);
		this.transform.localPosition = origin.localPosition;
		this.transform.name = origin.name;

		Destroy(floorNode.gameObject);
		Destroy(origin.gameObject);
		

		//this.bodywork = this.transform.parent.GetComponentInChildren<Bodywork>().transform;
		//this.mesh = this.GetComponentInChildren<MeshFilter>().transform;
		//this.origin = this.transform.parent;
		//this.car = this.origin.parent;
	}


	//Devuelve true cuando tocamos el suelo luego de estar en el aire;
	public bool checkGround() {
		RaycastHit raycastResults;
		bool previousIsGrounded = this.isGrounded;
		this.isGrounded = Physics.Raycast(this.transform.position, this.car.transform.up * -1, out raycastResults, this.radius, 1<<LayerMask.NameToLayer("Map"));

		if(this.isGrounded) {
			this.mesh.localPosition = (raycastResults.point + this.car.transform.up * this.radius) - this.transform.position;
		}
		else {
			this.mesh.position = this.transform.position;
		}
		
		return this.isGrounded && !previousIsGrounded;
	}

	public Vector3 getWheelOrigin(){
		return this.transform.position;
	}

	public Vector3 getWheelPosition(){
		return this.mesh.position;
	}

	public void initialize(int wheelID) {
		this.wheelID = wheelID;
	}

	public void setRotation(float direction) {
		this.mesh.rotation = Quaternion.Euler(0, (this.isFront ? direction : 0) + (this.isRight ? 90 : -90), 0);

		
	}

	
    void OnTriggerEnter(Collider collider) {

	}
}
