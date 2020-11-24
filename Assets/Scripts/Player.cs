using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable IDE0051

public class Player : Driver
{
    protected override void Ready()
    {
        //follow camera
    }

    void Update()
    {
        car.drive(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));
    }
}
