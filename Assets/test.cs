using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
	// Start is called before the first frame update
	void Start()
	{
		Utils.getValueInName("amortiguador", "tigua");
		Utils.getValueInName("amortiguador", "caca");
	}

	// Update is called once per frame
	void Update()
	{
		GetComponent<Rigidbody>().AddRelativeTorque(new Vector3(0, 0, 10f * Time.deltaTime));
	}
}
