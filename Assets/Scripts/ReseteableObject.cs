using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReseteableObject : MonoBehaviour
{
    Vector3 originalPosition;
    Quaternion originalRotation;

    void Awake()
    {
        enabled = false;
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    public void reset()
    {
        transform.position = originalPosition;
        transform.rotation = originalRotation;

        if (GetComponent<Rigidbody>() is Rigidbody rb)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}
