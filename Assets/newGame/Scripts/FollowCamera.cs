using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public float distanceXZ;
    public float distanceY;
    [SerializeField]
    private Transform followedObject;

    void Awake()
    {
        if (this.followedObject == null)
        {
            this.enabled = false;
        }
    }

    void Update()
    {
        Vector3 toObject = (this.followedObject.position - this.transform.position);
        Vector2 toObjectXZ = new Vector2(toObject.x, toObject.z).normalized;

        Vector3 desiredPosition = this.followedObject.position - new Vector3(toObjectXZ.x, 0, toObjectXZ.y) * this.distanceXZ + Vector3.up * this.distanceY;
        this.transform.position = Vector3.Lerp(this.transform.position, desiredPosition, 0.05f);
        this.transform.rotation = Quaternion.LookRotation(toObject);


    }
}
