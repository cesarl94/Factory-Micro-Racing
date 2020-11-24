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

[System.Serializable]
public struct CarInfo
{
    public GameObject carPrefab;
    public int color;
    public bool isPlayer;
}

[System.Serializable]
public struct Race
{
    public bool forward;
    public int laps;
    public CarInfo[] carsGrill;
}

public class LevelParser : MonoBehaviour
{
    public static LevelParser instance;
    [HideInInspector] public Arrow[] checkpointOrigins;
    [HideInInspector] public Arrow[] startingPoints;
    [HideInInspector] public Vector3[] trackPoints;

    [SerializeField] private Race raceInfo;

    private Player player;
    private List<IA> ias;

    void Awake()
    {
        LevelParser.instance = this;
        enabled = false;

        Transform mapInfo = Utils.findNode(transform, "MapInfo");
        Transform track = Utils.findNode(mapInfo, "Track");

        List<Transform> allCheckpointOriginsNodes = new List<Transform>();
        Checkpoint[] checkpoints = track.GetComponentsInChildren<Checkpoint>();
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

        Transform startingPointsNode = Utils.findNode(track, "StartingPoints");
        startingPoints = new Arrow[startingPointsNode.childCount];
        for (int i = 0; i < startingPointsNode.childCount; i++)
        {
            Transform startingPointNode = startingPointsNode.GetChild(i);
            startingPoints[i] = new Arrow(startingPointNode.position, startingPointNode.forward);
        }
        Destroy(startingPointsNode.gameObject);

        Transform pointsNode = Utils.findNode(track, "Points");
        trackPoints = new Vector3[pointsNode.childCount];
        for (int i = 0; i < trackPoints.Length; i++)
        {
            trackPoints[i] = pointsNode.GetChild(i).position;
        }
        Destroy(pointsNode.gameObject);

        for (int i = 0; i < raceInfo.carsGrill.Length; i++)
        {
            CarInfo carInfo = raceInfo.carsGrill[i];
            GameObject carGameObject = Instantiate(carInfo.carPrefab);
            Car car = carGameObject.GetComponent<Car>();
            if (carInfo.isPlayer)
            {
                player = carGameObject.AddComponent<Player>();
                player.Initialize(car, i);
            }
            else
            {
                IA ia = carGameObject.AddComponent<IA>();
                ia.Initialize(car, i);
                ias.Add(ia);
            }

        }
    }

    public Vector3 getDirectionTo(int nextPoint)
    {
        int previousID = nextPoint - 1;
        if (previousID < 0)
        {
            previousID = trackPoints.Length - 1;
        }

        Vector3 previousPosition = trackPoints[previousID];
        Vector3 nextPosition = trackPoints[nextPoint];

        return (nextPosition - previousPosition).normalized;
    }


    public void onCheckpointEnter(Car car, List<int> checkpointIDs)
    {
        string checkPoints = checkpointIDs[0].ToString();
        for (int i = 1; i < checkpointIDs.Count; i++)
        {
            checkPoints += ", " + checkpointIDs[i].ToString();
        }

        Debug.Log("The car " + car.name + " enter on Checkpoint(s) " + checkPoints);
    }

    public void onDeathzoneEnter(Car car, int enableID, int disableID)
    {
        Debug.Log("The car " + car.name + " enter on deathzone. enableID " + enableID + " disableID " + disableID);
    }
}
