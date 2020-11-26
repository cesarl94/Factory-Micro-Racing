using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IA : Driver
{
    private Arrow[] path;
    private int nextPointID;

    protected override void Ready()
    {
        nextPointID = 0;
        tracePath();

        FollowCamera followCamera = Camera.main.GetComponent<FollowCamera>();
        followCamera.followedObject = transform;
    }

    void Update()
    {
        float aditionalDistance = 0.4f * car.velocity;
        float traveledDistance = getTraveledDistanceToClosestPoint(transform.position);
        Arrow? traveledPoint = getTraveledPointInPath(traveledDistance + aditionalDistance);

        while (!traveledPoint.HasValue)
        {
            nextPointID = (nextPointID + 1) % LevelParser.instance.trackPoints.Length;
            tracePath();
            traveledDistance = getTraveledDistanceToClosestPoint(transform.position);
            traveledPoint = getTraveledPointInPath(traveledDistance + aditionalDistance);
        }

        Arrow simulatedPoint = traveledPoint.Value;

        float queTanDerechoVoy = Vector3.Dot(transform.forward, simulatedPoint.forward);
        if (queTanDerechoVoy < 0.1f) queTanDerechoVoy = 0.1f;

        Vector2 forward2D = new Vector2(transform.forward.x, transform.forward.z).normalized;
        Vector2 toSimulatedPoint2D = new Vector2(simulatedPoint.origin.x - transform.position.x, simulatedPoint.origin.z - transform.position.z).normalized;

        //Vector2 forward2D = new Vector2(transform.forward.x, transform.forward.z).normalized;
        float crossProduct = Utils.CrossProduct(toSimulatedPoint2D, forward2D);

        car.drive(/*1 - Mathf.Abs(crossProduct) - 0.5f*/ queTanDerechoVoy, crossProduct);

        // Arrow elPuntoMasCercanoAlPathQueEstoy = getFirstClosestPointOnPath(transform.position, queTanAdelanteDeboIr);


        // puntoMasCercano.transform.position = elPuntoMasCercanoAlPathQueEstoy.origin;
        // puntoMasCercano.transform.forward = elPuntoMasCercanoAlPathQueEstoy.forward;

        // float caminoRecorrido = 0f;
        // for (int i = 0; i < pathArrow.Length; i++)
        // {
        //     if (pathArrow[i].forward.Equals(elPuntoMasCercanoAlPathQueEstoy))
        //     {
        //         caminoRecorrido += Vector3.Distance(elPuntoMasCercanoAlPathQueEstoy.origin, pathArrow[i].position);
        //     }
        //     else
        //     {
        //         caminoRecorrido += pathArrow[i].magnitude;
        //     }
        // }

        // float caminoPorRecorrer = caminoRecorrido + queTanAdelanteDeboIr;
        // Arrow puntoAdelantado = getTraveledPointInPath(caminoPorRecorrer);
        // puntoAdelantadoGO.transform.position = puntoAdelantado.origin;
        // puntoAdelantadoGO.transform.forward = puntoAdelantado.forward;

        // float queTanDerechoVoy = Vector3.Dot(transform.forward, puntoAdelantado.forward);
        // if (queTanDerechoVoy < 0.1f) queTanDerechoVoy = 0.1f;

        // Vector2 forward2D = new Vector2(transform.forward.x, transform.forward.z).normalized;
        // float crossProduct = Utils.CrossProduct(direction2D, forward2D);

        // car.drive(1 - Mathf.Abs(crossProduct) - 0.5f, crossProduct);


    }

    private Arrow? getTraveledPointInPath(float distance)
    {
        float traveledDistance = 0f;
        for (int i = 0; i < path.Length; i++)
        {
            if (distance - traveledDistance > path[i].magnitude)
            {
                traveledDistance += path[i].magnitude;
            }
            else
            {
                return new Arrow(path[i].origin + path[i].forward * (distance - traveledDistance), path[i].forward);
            }
        }
        return null;
    }



    private float getTraveledDistanceToClosestPoint(Vector3 position)
    {
        float closestSqrDistance = float.PositiveInfinity;
        float lastSqrDistanceToOrigin = 0;
        float traveledDistance = 0;

        for (int i = 0; i < path.Length; i++)
        {
            Arrow currentArrow = path[i];
            Vector3 closestPoint = Utils.closestPointToLine(currentArrow.origin, currentArrow.destiny, position);
            float sqrDistance = Vector3.SqrMagnitude(closestPoint - position);
            if (sqrDistance < closestSqrDistance)
            {
                if (i > 0) traveledDistance += path[i - 1].magnitude;
                closestSqrDistance = sqrDistance;
                lastSqrDistanceToOrigin = Vector3.SqrMagnitude(closestPoint - currentArrow.origin);
            }
            else return traveledDistance + Mathf.Sqrt(lastSqrDistanceToOrigin);
        }

        return traveledDistance + path[path.Length - 1].magnitude;
    }

    public void tracePath()
    {
        Vector3[] trackPoints = LevelParser.instance.trackPoints;
        Vector3[] smoothedCorner = Utils.getSmoothedCorner(transform.position, trackPoints[nextPointID], trackPoints[(nextPointID + 1) % trackPoints.Length]);

        path = new Arrow[10];
        for (int i = 0; i < 10; i++)
        {
            Vector3 difference = smoothedCorner[i + 1] - smoothedCorner[i];
            float magnitude = difference.magnitude;
            path[i] = new Arrow(smoothedCorner[i], difference / magnitude, magnitude);
        }
    }

    void OnDrawGizmos()
    {
        for (int i = 1; i < path.Length; i++)
        {
            Vector3 previous = path[i - 1].origin;
            Vector3 current = path[i].origin;
            Debug.DrawLine(previous, current, Color.yellow);
        }
    }
}
