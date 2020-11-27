using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deathzone : MonoBehaviour
{
    private int enableID;
    private int disableID;
    private List<Car> insideCars;

    void Awake()
    {
        enabled = false;
        string nameIDs = Utils.getValueInName(transform.name, "Deathzone.");
        string enableStringID = Utils.getValueInName(nameIDs, "-e");
        enableID = enableStringID != "" ? int.Parse(enableStringID.Remove(3, enableStringID.Length - 3)) : -1;
        disableID = enableStringID != "" ? int.Parse(Utils.getValueInName(enableStringID, "d")) : 99999;

        insideCars = new List<Car>();
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.GetComponentInParent<Car>() is Car car)
        {
            if (!insideCars.Contains(car))
            {
                LevelParser.instance.onDeathzoneEnter(car, enableID, disableID);
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
