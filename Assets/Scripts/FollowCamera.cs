﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public float distanceXZ;
    public float distanceY;
    public Transform followedObject;

    void Awake()
    {
        if (followedObject == null)
        {
            enabled = false;
        }
    }

    void Update()
    {
        Vector3 toObject = followedObject.position - transform.position;
        Vector2 toObjectXZ = new Vector2(toObject.x, toObject.z).normalized;

        Vector3 desiredPosition = followedObject.position - new Vector3(toObjectXZ.x, 0, toObjectXZ.y) * distanceXZ + Vector3.up * distanceY;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, 0.05f * Time.deltaTime * 60f);
        transform.rotation = Quaternion.LookRotation(toObject);


    }
}
