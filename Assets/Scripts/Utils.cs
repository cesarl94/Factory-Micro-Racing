using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable IDE1006
#pragma warning disable IDE0028

public static class Utils
{
    public static string getValueInName(string source, string key)
    {
        int indexof = source.IndexOf(key);
        if (indexof == -1) return "";
        return source.Remove(0, indexof + key.Length);
    }

    public static float ruleOfFive(float inputDataA, float outputDataA, float inputDataB, float outputDataB, float input, bool clamp)
    {
        float t = (input - inputDataA) / (inputDataB - inputDataA);
        return outputDataA + (outputDataB - outputDataA) * (clamp ? Mathf.Min(Mathf.Max(t, 0), 1) : t);
    }

    public static float getInterpolatedValueInVectors(Vector2[] inputOutput, float input)
    {
        if (inputOutput.Length == 1)
        {
            return inputOutput[0].y;
        }

        for (int i = 0; i < inputOutput.Length - 1; i++)
        {
            Vector2 previous = inputOutput[i];
            Vector2 next = inputOutput[i + 1];

            if (input >= previous.x && input < next.x)
            {
                return Utils.ruleOfFive(previous.x, previous.y, next.x, next.y, input, false);
            }
        }

        return inputOutput[inputOutput.Length - 1].y;
    }


    public static Transform findNode(Transform parent, string name, bool exactly = true)
    {
        foreach (Transform child in parent)
        {
            if ((exactly && child.name == name) || (!exactly && child.name.Contains(name)))
            {
                return child;
            }
            else
            {
                Transform found = findNode(child, name);
                if (found != null)
                {
                    return found;
                }
            }
        }
        return null;
    }

    public static int SortByNameValue(Transform p1, Transform p2, string key)
    {
        return int.Parse(Utils.getValueInName(p1.name, key)).CompareTo(int.Parse(Utils.getValueInName(p2.name, key)));
    }

    public static float CrossProduct(Vector2 a, Vector2 b)
    {
        return a.x * b.y - a.y * b.x;
    }

    public static Vector3 getBezierPoint(Vector3 initPoint, Vector3 initHandle, Vector3 endHandle, Vector3 endPoint, float t)
    {
        Vector3 lerp1 = Vector3.Lerp(initPoint, initHandle, t);
        Vector3 lerp2 = Vector3.Lerp(endHandle, endPoint, t);
        return Vector3.Lerp(lerp1, lerp2, t);
    }

    public static Vector3[] getSmoothedCorner(Vector3 a, Vector3 b, Vector3 c, float cornerBezierFactor = 0.5f, float handleBezierFactor = 1.15f)
    {
        Vector3[] rv = new Vector3[11];
        rv[0] = a;

        Vector3 initBezierPoint = Vector3.Lerp(b, a, cornerBezierFactor);
        Vector3 endBezierPoint = Vector3.Lerp(b, c, cornerBezierFactor);
        Vector3 initHandle = Vector3.Lerp(initBezierPoint, b, handleBezierFactor);
        Vector3 endHandle = Vector3.Lerp(endBezierPoint, b, handleBezierFactor);

        for (int i = 0; i < 10; i++)
            rv[i] = Utils.getBezierPoint(initBezierPoint, initHandle, endHandle, endBezierPoint, (float)i / 9f);

        return rv;
    }

    // public static Vector3[] getSmoothedTrackPath(Vector3[] trackPoints, float cornerBezierFactor = 0.5f, float handleBezierFactor = 1.15f)
    // {
    //     Vector3[] path = new Vector3[(trackPoints.Length - 2) * 10 + 1];
    //     path[0] = trackPoints[0];

    //     for (int i = 1; i < trackPoints.Length - 1; i++)
    //     {
    //         Vector3 currentPoint = trackPoints[i];
    //         Vector3 previousPoint = trackPoints[i - 1];
    //         Vector3 nextPoint = trackPoints[i + 1];

    //         Vector3 initBezierPoint = Vector3.Lerp(previousPoint, currentPoint, cornerBezierFactor);
    //         Vector3 endBezierPoint = Vector3.Lerp(nextPoint, currentPoint, cornerBezierFactor);
    //         Vector3 initHandle = Vector3.Lerp(initBezierPoint, currentPoint, handleBezierFactor);
    //         Vector3 endHandle = Vector3.Lerp(endBezierPoint, currentPoint, handleBezierFactor);

    //         for (int j = 0; j < 10; j++)
    //         {
    //             float t = (float)(j + 1) / 10f;
    //             Vector3 bezierPoint = Utils.getBezierPoint(initBezierPoint, initHandle, endHandle, endBezierPoint, t);
    //             path[(i - 1) * 10 + j + 1] = bezierPoint;
    //         }
    //     }
    //     path[path.Length - 1] = trackPoints[0];

    //     List<Vector3> pathWithoutDoublePoints = new List<Vector3>();
    //     pathWithoutDoublePoints.Add(path[0]);
    //     for (int i = 1; i < path.Length; i++)
    //     {
    //         Vector3 previousPoint = path[i - 1];
    //         Vector3 currentPoint = path[i];

    //         if (!currentPoint.Equals(previousPoint))
    //         {
    //             pathWithoutDoublePoints.Add(currentPoint);

    //         }
    //         else
    //         {
    //             Debug.Log("EVITADOS DOS PUNTOS IGUALES");
    //         }

    //     }

    //     return pathWithoutDoublePoints.ToArray();
    // }

    public static Vector3 getMin(Vector3 a, Vector3 b)
    {
        return new Vector3(Mathf.Min(a.x, b.x), Mathf.Min(a.y, b.y), Mathf.Min(a.z, b.z));
    }

    public static Vector3 getMax(Vector3 a, Vector3 b)
    {
        return new Vector3(Mathf.Max(a.x, b.x), Mathf.Max(a.y, b.y), Mathf.Max(a.z, b.z));
    }

    public static Vector3 clamp(Vector3 min, Vector3 max, Vector3 value)
    {
        float x = value.x < min.x ? min.x : (value.x > max.x ? max.x : value.x);
        float y = value.y < min.y ? min.y : (value.y > max.y ? max.y : value.y);
        float z = value.z < min.z ? min.z : (value.z > max.z ? max.z : value.z);
        return new Vector3(x, y, z);
    }

    public static Vector3 closestPointToLine(Vector3 P0, Vector3 P1, Vector3 point)
    {
        Vector3 u = P1 - P0;
        Vector3 pq = point - P0;
        Vector3 w2 = pq - (u * Vector3.Dot(pq, u) / Vector3.SqrMagnitude(u));
        Vector3 rv = point - w2;
        Vector3 min = getMin(P0, P1);
        Vector3 max = getMax(P0, P1);
        return clamp(min, max, rv);
    }

}

public static class CoroutineUtil
{
    public static IEnumerator WaitForRealSeconds(float time)
    {
        float start = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup < start + time)
        {
            yield return null;
        }
    }
}
