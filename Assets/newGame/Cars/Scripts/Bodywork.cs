using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bodywork : MonoBehaviour
{
	private Car car;
	private Quaternion originalLocalRotation;

    void Awake()
    {
		this.car = this.GetComponentInParent<Car>();
		this.originalLocalRotation = this.transform.localRotation;
		this.enabled = false;
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
