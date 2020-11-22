using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable RCS1222
#pragma warning disable IDE1006
#pragma warning disable RCS1018
#pragma warning disable RCS1213
#pragma warning disable RCS1110
#pragma warning disable IDE0051

public class LevelParser : MonoBehaviour
{
    public static LevelParser instance;

    void Awake()
    {
        LevelParser.instance = this;
        enabled = false;

        List<Transform> allCheckPointOrigins = new List<Transform>();
        Checkpoint[] checkpoints = GetComponentsInChildren<Checkpoint>();
        foreach (Checkpoint checkpoint in checkpoints)
        {
            List<Transform> checkpointOrigins = checkpoint.getOrigins();
            foreach (Transform origin in checkpointOrigins)
            {
                allCheckPointOrigins.Add(origin);
            }
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
        Debug.Log("The car " + car.name + " enter on deathzone");
    }
}
