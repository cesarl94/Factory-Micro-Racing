using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IA : Driver
{
    protected Vector3[] path;
    private int nextPointID;
    private Vector3 direction;
    private Vector2 direction2D;
    private Vector3 nextPoint;

    protected override void Ready()
    {
        Vector3[] trackPoints = LevelParser.instance.trackPoints;
        Vector3[] allTrackPoints = new Vector3[trackPoints.Length + 2];
        allTrackPoints[0] = LevelParser.instance.startingPoints[startingPos].position;
        for (int i = 0; i < trackPoints.Length; i++)
        {
            allTrackPoints[i + 1] = trackPoints[i];
        }
        allTrackPoints[trackPoints.Length + 1] = trackPoints[0];

        path = Utils.getSmoothedTrackPath(allTrackPoints, 0.8f);

        nextPointID = 1;
        nextPoint = path[1];
        direction = (path[1] - path[0]).normalized;
        direction2D = new Vector2(direction.x, direction.z).normalized;

        FollowCamera followCamera = Camera.main.GetComponent<FollowCamera>();
        followCamera.followedObject = transform;
    }

    void Update()
    {

        bool pointPassed = Vector3.Dot(nextPoint - transform.position, direction) < 0;
        while (pointPassed)
        {
            Vector3 previousPoint = nextPoint;
            nextPointID++;
            if (nextPointID >= path.Length)
            {
                nextPointID = 0;
            }
            nextPoint = path[nextPointID];
            direction = (nextPoint - previousPoint).normalized;
            direction2D = new Vector2(direction.x, direction.z).normalized;
            pointPassed = Vector3.Dot(nextPoint - transform.position, direction) < 0;
        }

        float queTanDerechoVoy = Vector3.Dot(transform.forward, direction);
        //if (queTanDerechoVoy < 0.1f) queTanDerechoVoy = 0.1f;

        // Vector2 forward2D = new Vector2(transform.forward.x, transform.forward.z).normalized;
        // float crossProduct = Utils.CrossProduct(direction2D, forward2D);

        // car.drive(1 - Mathf.Abs(crossProduct) - 0.5f, crossProduct);
        Rigidbody rb = car.GetComponent<Rigidbody>();
        rb.velocity = direction * queTanDerechoVoy * 10f;
        car.transform.forward = direction;
    }

    void OnDrawGizmos()
    {
        for (int i = 1; i < path.Length; i++)
        {
            Vector3 previous = path[i - 1];
            Vector3 current = path[i];
            Debug.DrawLine(previous, current, Color.yellow);
        }
    }
}
