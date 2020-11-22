using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

#pragma warning disable RCS1222
#pragma warning disable IDE1006
#pragma warning disable RCS1018
#pragma warning disable RCS1213
#pragma warning disable RCS1110
#pragma warning disable IDE0051

public struct Arrow
{
    public Vector3 position;
    public Vector3 forward;

    public Arrow(Vector3 position, Vector3 forward)
    {
        this.position = position;
        this.forward = forward;
    }
}

public class LevelParser : MonoBehaviour
{
    public static LevelParser instance;
    private Arrow[] checkpointOrigins;
    private Arrow[] startingPoints;
    private Vector3[] trackPoints;

    void Awake()
    {
        LevelParser.instance = this;
        enabled = false;

        List<Transform> allCheckpointOriginsNodes = new List<Transform>();
        Checkpoint[] checkpoints = GetComponentsInChildren<Checkpoint>();
        foreach (Checkpoint checkpoint in checkpoints)
        {
            List<Transform> originsNodes = checkpoint.getOrigins();
            foreach (Transform originNode in originsNodes)
            {
                allCheckpointOriginsNodes.Add(originNode);
            }
        }

        allCheckpointOriginsNodes.Sort((p1, p2) => int.Parse(Utils.getValueInName(p1.name, "Origin.")).CompareTo(int.Parse(Utils.getValueInName(p2.name, "Origin."))));
        checkpointOrigins = new Arrow[allCheckpointOriginsNodes.Count];
        for (int i = 0; i < allCheckpointOriginsNodes.Count; i++)
        {
            Transform originNode = allCheckpointOriginsNodes[i];
            checkpointOrigins[i] = new Arrow(originNode.position, originNode.forward);
            Destroy(originNode.gameObject);
        }

        Transform startingPointsNode = Utils.findNode(transform, "StartingPoints");
        startingPoints = new Arrow[startingPointsNode.childCount];
        for (int i = 0; i < startingPointsNode.childCount; i++)
        {
            Transform startingPointNode = startingPointsNode.GetChild(i);
            startingPoints[i] = new Arrow(startingPointNode.position, startingPointNode.forward);
        }
        Destroy(startingPointsNode.gameObject);

        Transform pointsNode = Utils.findNode(transform, "Points");
        trackPoints = new Vector3[pointsNode.childCount];
        for (int i = 0; i < trackPoints.Length; i++)
        {
            trackPoints[i] = pointsNode.GetChild(i).position;
        }
        Destroy(pointsNode.gameObject);
    }

    private void logArray(List<Transform> origins)
    {
        foreach (Transform origin in origins)
        {
            Debug.Log(origin.name);
        }
    }

    public void onCheckpointEnter(RearWheelDrive car, List<int> checkpointIDs)
    {
        string checkPoints = checkpointIDs[0].ToString();
        for (int i = 1; i < checkpointIDs.Count; i++)
        {
            checkPoints += ", " + checkpointIDs[i].ToString();
        }

        Debug.Log("The car " + car.name + " enter on Checkpoint(s) " + checkPoints);
    }

    public void onDeathzoneEnter(RearWheelDrive car, int enableID, int disableID)
    {
        Debug.Log("The car " + car.name + " enter on deathzone. enableID " + enableID + " disableID " + disableID);
    }
}
