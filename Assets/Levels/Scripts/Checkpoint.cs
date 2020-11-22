using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable RCS1222
#pragma warning disable IDE1006
#pragma warning disable RCS1018
#pragma warning disable RCS1213
#pragma warning disable RCS1110
#pragma warning disable IDE0051

public class Checkpoint : MonoBehaviour
{
    private List<int> checkpointIDs;

    void Awake()
    {
        enabled = false;
        string nameIDs = Utils.getValueInName(transform.name, "Checkpoint.");
        string firstID = nameIDs.Remove(3, nameIDs.Length - 3);
        string secondID = Utils.getValueInName(nameIDs, "-a");

        checkpointIDs = new List<int>();
        if (firstID != "") checkpointIDs.Add(int.Parse(firstID));
        if (secondID != "") checkpointIDs.Add(int.Parse(secondID));
    }

    public List<Transform> getOrigins()
    {
        List<Transform> origins = new List<Transform>();
        foreach (Transform child in transform)
        {
            if (child.name.Contains("Origin"))
            {
                origins.Add(child);
            }
        }
        return origins;
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.GetComponentInParent<RearWheelDrive>() is RearWheelDrive car)
        {
            LevelParser.instance.onCheckpointEnter(car, checkpointIDs);
        }
    }
}
