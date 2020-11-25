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
    [HideInInspector] public Player player;

    [SerializeField] private Race raceInfo;

    private Driver[] drivers;
    private Driver[] sortedDrivers;

    void Awake()
    {
        LevelParser.instance = this;

        Transform mapInfo = Utils.findNode(transform, "MapInfo");
        Transform colliders = Utils.findNode(mapInfo, "MapColliders");

        foreach (Transform machineCollider in colliders)
        {
            if (machineCollider.name.Contains("Machine"))
            {
                machineCollider.name = machineCollider.name.Remove(0, 21);
            }
        }

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

        drivers = new Driver[raceInfo.carsGrill.Length];

        for (int i = 0; i < raceInfo.carsGrill.Length; i++)
        {
            CarInfo carInfo = raceInfo.carsGrill[i];
            GameObject carGameObject = Instantiate(carInfo.carPrefab);
            Car car = carGameObject.GetComponent<Car>();
            if (carInfo.isPlayer)
            {
                player = carGameObject.AddComponent<Player>();
                drivers[i] = player;
            }
            else
            {
                drivers[i] = carGameObject.AddComponent<IA>();
            }
            drivers[i].Initialize(car, i);

        }
    }

    void Update()
    {
        sortDrivers();
    }

    private void sortDrivers()
    {
        SortedDictionary<int, SortedDictionary<int, List<Driver>>> carsByLapByCheckpoint = new SortedDictionary<int, SortedDictionary<int, List<Driver>>>();

        for (int i = 0; i < drivers.Length; i++)
        {
            Driver driver = drivers[i];
            if (!carsByLapByCheckpoint.ContainsKey(driver.laps))
            {
                carsByLapByCheckpoint.Add(driver.laps, new SortedDictionary<int, List<Driver>>());
            }
            SortedDictionary<int, List<Driver>> carsInSameLap = carsByLapByCheckpoint[driver.laps];
            if (!carsInSameLap.ContainsKey(driver.lastCheckpoint))
            {
                carsInSameLap.Add(driver.lastCheckpoint, new List<Driver>());
            }
            List<Driver> carsInSameCheckpoint = carsInSameLap[driver.lastCheckpoint];
            carsInSameCheckpoint.Add(driver);
        }

        List<Driver> sortedDriversAux = new List<Driver>();
        foreach (KeyValuePair<int, SortedDictionary<int, List<Driver>>> byLap in carsByLapByCheckpoint)
        {
            foreach (KeyValuePair<int, List<Driver>> byCheckpoint in byLap.Value)
            {
                List<Driver> drivers = byCheckpoint.Value;
                Vector3 nextPoint = checkpointOrigins[drivers[0].nextCheckpoint].position;
                drivers.Sort((p1, p2) =>
                {
                    float sqrDistanceP1 = Vector3.SqrMagnitude(p1.transform.position - nextPoint);
                    float sqrDistanceP2 = Vector3.SqrMagnitude(p2.transform.position - nextPoint);
                    return sqrDistanceP1.CompareTo(sqrDistanceP2);
                });

                foreach (Driver driver in drivers)
                {
                    sortedDriversAux.Add(driver);
                }
            }
        }

        sortedDriversAux.Reverse();
        sortedDrivers = sortedDriversAux.ToArray();

        for (int i = 0; i < sortedDrivers.Length; i++)
        {
            sortedDrivers[i].racePosition = i + 1;
        }
    }

    private Dictionary<int, List<Driver>> getCarsByLap()
    {
        Dictionary<int, List<Driver>> carsByLap = new Dictionary<int, List<Driver>>();

        for (int i = 0; i < drivers.Length; i++)
        {
            Driver driver = drivers[i];
            if (!carsByLap.ContainsKey(driver.laps))
            {
                carsByLap.Add(driver.laps, new List<Driver>());
            }
            List<Driver> carsInSameLap = carsByLap[driver.laps];
            carsInSameLap.Add(driver);
        }

        return carsByLap;
    }

    public void onCheckpointEnter(Car car, List<int> checkpointIDs)
    {
        Driver driver = car.driver;
        int nextDriverCheckpoint = driver.nextCheckpoint;

        if (checkpointIDs.Contains(nextDriverCheckpoint))
        {
            driver.lastCheckpoint = nextDriverCheckpoint;
            if (nextDriverCheckpoint == 0)
            {
                driver.laps++;
                if (driver.laps == raceInfo.laps - 1)
                {
                    if (driver.isPlayer) Debug.LogWarning("LAST LAP!");
                }
                else if (driver.laps == raceInfo.laps)
                {
                    if (driver.isPlayer) Debug.LogWarning("END RACE. WINNER ");
                }
                else
                {
                    if (driver.isPlayer) Debug.LogWarning((raceInfo.laps - driver.laps) + " LAPS LEFT");
                }
            }
            else
            {
                if (driver.isPlayer) Debug.LogWarning("PASSED " + nextDriverCheckpoint + " CHECKPOINT");
            }
        }
        else if (checkpointIDs.Contains(driver.lastCheckpoint))
        {
            if (driver.isPlayer) Debug.LogWarning("WRONG WAY!");
        }
        else
        {
            driver.Kill();
        }
    }

    public void onDeathzoneEnter(Car car, int enableID, int disableID)
    {
        Driver driver = car.driver;
        if (driver.lastCheckpoint >= enableID && driver.lastCheckpoint < disableID)
        {
            Debug.LogError("KILL: lastCheckpoint: " + driver.lastCheckpoint + " enableID: " + enableID + " disableID " + disableID);
            driver.Kill();
        }
        else
        {
            Debug.Log("ENTER ON SAFE DEATHZONE");
        }
    }
}
