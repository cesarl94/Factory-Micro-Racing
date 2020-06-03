using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	public Mesh mesh;
	public GameObject prefabSphereLeg;
	Stack spherePool;
	Transform[] legs;
	// Start is called before the first frame update
	void Start()
	{
		if (mesh == null || prefabSphereLeg == null)
		{
			Debug.LogError("Te olvidaste un public param en Player");
			Debug.Break();
		}

		spherePool = new Stack();

		prefabSphereLeg.GetComponent<SphereCollider>().radius = Constants.sphereLegRadius;
		prefabSphereLeg.GetComponent<Renderer>().sharedMaterial.SetFloat("_Scale", Constants.sphereLegRadius * 2 * Constants.legSphereImageScale);
		prefabSphereLeg.GetComponent<Renderer>().sharedMaterial.SetColor("_Color", Constants.legsColor);

		legs = new Transform[2];
		foreach (Transform child in transform)
		{
			if (child.transform.name.Contains("Leg"))
			{
				if (legs[0] == null) legs[0] = child;
				else legs[1] = child;
			}
		}

	}

	// Update is called once per frame
	//void Update()
	//{

	//}

	public void updateLegs(List<GameObject> inputPositions)
	{

		GameObject first = inputPositions[0];
		Vector3 firstPosition = first.transform.position;
		int i = 0;
		foreach (GameObject inputPosition in inputPositions)
		{
			if (i++ == 0) continue;
			Vector3 position = (inputPosition.transform.position - firstPosition) / Constants.pixelsPerUnitLegs;
			GameObject sphere1 = getSphere();
			Debug.Log("legs[0]: " + legs[0]);
			sphere1.transform.SetParent(legs[0]);
			sphere1.transform.localPosition = position;
			GameObject sphere2 = getSphere();
			Debug.Log("legs[1]: " + legs[1]);
			sphere2.transform.SetParent(legs[1]);
			sphere2.transform.localPosition = position;
			//SphereCollider sphereCollider = gameObject.AddComponent<SphereCollider>();
			//sphereCollider.center = position;
			//sphereCollider.radius = Constants.sphereLegRadius;
		}

		/*GameObject first = inputPositions[0];
		Vector3 firstPosition = first.transform.position;
		int i = 0;
		foreach (GameObject inputPosition in inputPositions)
		{
			if (i++ == 0) continue;
			Vector3 position = (inputPosition.transform.position - firstPosition) / Constants.pixelsPerUnitLegs;
			SphereCollider sphereCollider = gameObject.AddComponent<SphereCollider>();
			sphereCollider.center = position;
			sphereCollider.radius = Constants.sphereLegRadius;
		}*/
	}

	GameObject getSphere()
	{
		if (spherePool.Count > 0)
		{
			GameObject top = spherePool.Pop() as GameObject;
			top.SetActive(true);
			return top;
		}
		GameObject point = Instantiate(prefabSphereLeg);
		return point;
	}

	void sphereToPool(GameObject sphere)
	{
		sphere.SetActive(false);
		spherePool.Push(sphere);
	}

}
