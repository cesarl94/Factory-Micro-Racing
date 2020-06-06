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

		//remove current childrens
		foreach(Transform leg0Child in legs[0]){
			sphereToPool(leg0Child.gameObject);
		}
		legs[0].DetachChildren();
		foreach(Transform leg1Child in legs[1]){
			sphereToPool(leg1Child.gameObject);
		}
		legs[1].DetachChildren();

		SphereCollider[] spheres = legs[0].GetComponents<SphereCollider>();
		foreach(SphereCollider SC in spheres){
			Destroy(SC);
		}
		spheres = legs[1].GetComponents<SphereCollider>();
		foreach(SphereCollider SC in spheres){
			Destroy(SC);
		}

		GameObject first = inputPositions[0];
		Vector3 firstPosition = first.transform.position;
		int i = 0;
		foreach (GameObject inputPosition in inputPositions)
		{
			if (i++ == 0) continue;
			Vector3 position = (inputPosition.transform.position - firstPosition) / (float)Constants.pixelsPerUnitLegs;
			GameObject sphere1 = getSphere();

			sphere1.transform.SetParent(legs[0]);
			sphere1.transform.localPosition = position;
			GameObject sphere2 = getSphere();

			sphere2.transform.SetParent(legs[1]);
			sphere2.transform.localPosition = position;

			SphereCollider SC1 = legs[0].gameObject.AddComponent<SphereCollider>();
			SC1.radius = Constants.sphereLegRadius;
			SC1.center = position;
			SphereCollider SC2 = legs[1].gameObject.AddComponent<SphereCollider>();
			SC2.radius = Constants.sphereLegRadius;
			SC2.center = position;
			
		}

		transform.Find("Cube").GetComponent<Rigidbody>().velocity = Vector3.zero;
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
