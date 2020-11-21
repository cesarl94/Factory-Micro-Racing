using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bodywork : MonoBehaviour
{
	private Car car;
	private Quaternion originalLocalRotation;
	private Rigidbody rb;

    void Awake()
    {
		this.car = this.GetComponentInParent<Car>();
		this.originalLocalRotation = this.transform.localRotation;
		rb = GetComponentInParent<Rigidbody>();
		//this.enabled = false;
	}

    // Update is called once per frame
    void Update()
    {
        
    }

	void OnCollisionEnter(Collision collision){
		//rb.velocity
		Debug.Log("VELOCITY: " + rb.velocity.normalized);
		Vector3 reflection = Vector3.Reflect(rb.velocity.normalized, collision.contacts[0].normal);

		rb.position += collision.contacts[0].normal * 0.1f;
		rb.velocity *= 0.5f;
	}

	void OnCollisionStay(Collision collision){
		//rb.position += collision.contacts[0].normal * 0.1f;
		//rb.velocity *= 0.9f;
	}
}
