using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	public Mesh mesh;
	public GameObject prefabSphereLeg;
	Stack spherePool;
	Transform[] legs;
	Transform cube;
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
			if (child.name.Contains("Leg"))
			{
				if (legs[0] == null) legs[0] = child;
				else legs[1] = child;
			}
			else if (child.name.Contains("Cube"))
			{
				cube = child;
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
		foreach (Transform leg0Child in legs[0])
		{
			sphereToPool(leg0Child.gameObject);
		}
		legs[0].DetachChildren();
		foreach (Transform leg1Child in legs[1])
		{
			sphereToPool(leg1Child.gameObject);
		}
		legs[1].DetachChildren();

		SphereCollider[] spheres = legs[0].GetComponents<SphereCollider>();
		foreach (SphereCollider SC in spheres)
		{
			Destroy(SC);
		}
		spheres = legs[1].GetComponents<SphereCollider>();
		foreach (SphereCollider SC in spheres)
		{
			Destroy(SC);
		}

		List<Vector3> spherePositions0 = new List<Vector3>();
		List<Vector3> spherePositions1 = new List<Vector3>();

		GameObject first = inputPositions[0];
		Vector3 firstPosition = first.transform.position;
		int i = 0;
		Vector3 lastPos = firstPosition;
		List<float> magnitudes = new List<float>();
		foreach (GameObject inputPosition in inputPositions)
		{
			if (i++ == 0) continue;
			float mg = Vector3.Magnitude(lastPos - inputPosition.transform.position) / Constants.pixelsPerUnitLegs;
			if (mg < 0.0001f) continue;
			magnitudes.Add(mg);
			Vector3 position = (inputPosition.transform.position - firstPosition) / Constants.pixelsPerUnitLegs;
			GameObject sphere1 = getSphere();

			sphere1.transform.SetParent(legs[0]);
			sphere1.transform.localPosition = position;

			spherePositions0.Add(sphere1.transform.position);
			GameObject sphere2 = getSphere();

			sphere2.transform.SetParent(legs[1]);
			sphere2.transform.localPosition = position;
			spherePositions1.Add(sphere2.transform.position);

			SphereCollider SC1 = legs[0].gameObject.AddComponent<SphereCollider>();
			SC1.radius = Constants.sphereLegRadius;
			SC1.center = position;
			SphereCollider SC2 = legs[1].gameObject.AddComponent<SphereCollider>();
			SC2.radius = Constants.sphereLegRadius;
			SC2.center = position;

			lastPos = inputPosition.transform.position;
		}

		List<RaycastHit> hits0 = new List<RaycastHit>();
		List<RaycastHit> hits1 = new List<RaycastHit>();

		Vector3 previousPos = spherePositions0[0];
		i = 0;
		foreach (Vector3 pos in spherePositions0)
		{
			if (i++ == 0) continue;

			Vector3 dir = pos - previousPos;
			RaycastHit RC;
			float mg = magnitudes[i - 1];
			//if (Physics.SphereCast(previousPos, Constants.sphereLegRadius, dir, out RC, 100f, 1 << 8))
			if (Physics.Raycast(previousPos, dir, out RC, mg, 1 << 8))
			{
				hits0.Add(RC);
			}

			previousPos = pos;
		}

		previousPos = spherePositions1[0];
		i = 0;
		foreach (Vector3 pos in spherePositions1)
		{
			if (i++ == 0) continue;

			Vector3 dir = pos - previousPos;
			RaycastHit RC;
			float mg = magnitudes[i - 1];

			//if (Physics.SphereCast(previousPos, Constants.sphereLegRadius, dir, out RC, 100f, 1 << 8))
			if (Physics.Raycast(previousPos, dir, out RC, mg, 1 << 8))
			{
				hits1.Add(RC);
			}

			previousPos = pos;
		}


		if (hits0.Count > 0)
		{
			if (hits0[0].collider.attachedRigidbody == null)
			{
				if (Input.GetKey(KeyCode.Space))
				{
					//GameObject a0 = new GameObject("a0");
					//a0.transform.position = legs[0].position + difference;
					GameObject a1 = new GameObject("a1");
					a1.transform.position = hits0[0].point;
					GameObject a2 = new GameObject("a2");
					a2.transform.position = spherePositions0[spherePositions0.Count - 1];
					GameObject a3 = new GameObject("a3");
					a3.transform.position = spherePositions0[0];
					GameObject a4 = new GameObject("a4");
					a4.transform.position = legs[0].position;
					Debug.Log("A");
				}
				Vector3 difference = hits0[0].point - spherePositions0[spherePositions0.Count - 1];
				float differenceLength = difference.magnitude;
				Vector3 normalizedDifference = difference / differenceLength;
				legs[0].position += normalizedDifference * (differenceLength + Constants.sphereLegRadius);
				legs[1].position += normalizedDifference * (differenceLength + Constants.sphereLegRadius);
				cube.position += difference;
			}

		}
		if (hits1.Count > 0)
		{
			if (hits1[0].collider.attachedRigidbody == null)
			{
				if (Input.GetKey(KeyCode.Space))
				{
					//GameObject b0 = new GameObject("b0");
					//b0.transform.position = legs[1].position + difference;
					GameObject b1 = new GameObject("b1");
					b1.transform.position = hits1[0].point;
					GameObject b2 = new GameObject("b2");
					b2.transform.position = spherePositions1[spherePositions1.Count - 1];
					GameObject b3 = new GameObject("b3");
					b3.transform.position = spherePositions1[0];
					GameObject b4 = new GameObject("b4");
					b4.transform.position = legs[1].position;
					Debug.Log("B");
				}
				Vector3 difference = hits1[0].point - spherePositions1[spherePositions1.Count - 1];
				float differenceLength = difference.magnitude;
				Vector3 normalizedDifference = difference / differenceLength;
				legs[0].position += normalizedDifference * (differenceLength + Constants.sphereLegRadius);
				legs[1].position += normalizedDifference * (differenceLength + Constants.sphereLegRadius);
				cube.position += difference;
			}

		}

		//if (Input.GetKey(KeyCode.Space)) Debug.Break();
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
