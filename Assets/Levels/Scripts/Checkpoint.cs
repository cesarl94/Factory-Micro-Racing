using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    List<Transform> origins;

    void Start()
    {
        origins = new List<Transform>();
        foreach (Transform child in transform)
        {
            if (child.name.Contains("Origin"))
            {
                origins.Add(child);

                Debug.Log(child.name + " forward: " + getRealForward(child));
            }
        }

        enabled = false;
    }

    private Vector3 getRealForward(Transform childTransform)
    {
        // Vector3 eulerRotation = childTransform.rotation.ToEuler();
        // Debug.Log("EULER: " + eulerRotation);
        // Transform identityTransform = new GameObject("aux").transform;

        // identityTransform.RotateAroundLocal(Vector3.right, eulerRotation.x);
        // identityTransform.RotateAroundLocal(Vector3.up, eulerRotation.y);
        // identityTransform.RotateAroundLocal(Vector3.forward, eulerRotation.z);

        // childTransform.transform.localRotation = identityTransform.localRotation;

        // Destroy(identityTransform.gameObject);
        return childTransform.forward;
    }
}
