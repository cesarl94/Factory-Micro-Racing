using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable IDE0051

public class Player : Driver
{
    protected override void Ready()
    {
        FollowCamera followCamera = Camera.main.GetComponent<FollowCamera>();
        followCamera.followedObject = transform;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Return))
        {
            PauseMenu.instance.activate(!PauseMenu.instance.getState());
        }

        car.drive(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));
    }
}
