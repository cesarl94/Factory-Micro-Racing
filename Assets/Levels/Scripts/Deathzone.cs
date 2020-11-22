using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable RCS1222
#pragma warning disable IDE1006
#pragma warning disable RCS1018
#pragma warning disable RCS1213
#pragma warning disable RCS1110
#pragma warning disable IDE0051

public class Deathzone : MonoBehaviour
{
    private int enableID;
    private int disableID;

    void Awake()
    {
        enabled = false;
        string nameIDs = Utils.getValueInName(transform.name, "Deathzone.");
        string enableStringID = Utils.getValueInName(nameIDs, "-e");
        enableID = enableStringID != "" ? int.Parse(enableStringID.Remove(3, enableStringID.Length - 3)) : -1;
        disableID = enableStringID != "" ? int.Parse(Utils.getValueInName(enableStringID, "d")) : 99999;
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.GetComponentInParent<RearWheelDrive>() is RearWheelDrive car)
        {
            LevelParser.instance.onDeathzoneEnter(car, enableID, disableID);
        }
    }
}
