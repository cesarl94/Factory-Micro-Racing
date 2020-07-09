using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneParser : MonoBehaviour
{
	// Start is called before the first frame update
	void Start()
	{
		Transform mapNode = findNode(transform, "Map");
		Transform trackLines = findNode(transform, "TrackLines");
		List<Vector3> ver = new List<Vector3>();
		/*Vector3[] trackVertices = */
		Vector3[] unsortedTrackVertices = trackLines.GetComponent<MeshFilter>().mesh.vertices;

		int[] indexes = trackLines.GetComponent<MeshFilter>().mesh.GetIndices(0);
		Vector3[] trackVertices = new Vector3[unsortedTrackVertices.Length];

		Debug.Log("SUBMESH COUNT: " + trackLines.GetComponent<MeshFilter>().mesh.subMeshCount);

		for (int j = 0; j < trackLines.GetComponent<MeshFilter>().mesh.subMeshCount; j++)
		{
			Debug.Log(j + " INDEX COUNT: " + trackLines.GetComponent<MeshFilter>().mesh.GetIndexCount(j));
			Debug.Log(j + " INDEX START: " + trackLines.GetComponent<MeshFilter>().mesh.GetIndexStart(j));
		}


		Debug.Log("INDEXES_: " + indexes.Length);
		int i = 0;
		foreach (int index in indexes)
		{
			Debug.Log(i + " " + index);
			trackVertices[i++] = unsortedTrackVertices[index];
		}

		i = 0;

		Debug.Log("VERTICES: " + trackLines.GetComponent<MeshFilter>().mesh.vertexCount);
		foreach (Vector3 v in unsortedTrackVertices)
		{
			GameObject go = new GameObject(i++.ToString() + " " + v.ToString("F2"));
			go.transform.position = v;
			Debug.Log(v);
		}
	}

	// Update is called once per frame
	void Update()
	{

	}

	private Transform findNode(Transform parent, string name)
	{
		Transform findedNode = parent.Find(name);
		if (findedNode == null)
		{
			Debug.LogError("Error: \"" + name + "\" node wasn't finded");
			Debug.Break();
		}
		return findedNode;
	}
}
