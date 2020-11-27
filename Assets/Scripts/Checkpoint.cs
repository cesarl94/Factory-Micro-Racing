using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private List<int> checkpointIDs;
    private List<Car> insideCars;

    void Awake()
    {
        enabled = false;
        string nameIDs = Utils.getValueInName(transform.name, "Checkpoint.");
        string firstID = nameIDs.Remove(3, nameIDs.Length - 3);
        string secondID = Utils.getValueInName(nameIDs, "-a");

        checkpointIDs = new List<int>();
        if (firstID != "") checkpointIDs.Add(int.Parse(firstID));
        if (secondID != "") checkpointIDs.Add(int.Parse(secondID));

        insideCars = new List<Car>();
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
        if (collider.GetComponentInParent<Car>() is Car car)
        {
            if (!insideCars.Contains(car))
            {
                LevelParser.instance.onCheckpointEnter(car, checkpointIDs);
            }
            insideCars.Add(car);
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.GetComponentInParent<Car>() is Car car)
        {
            insideCars.Remove(car);
        }
    }
}
