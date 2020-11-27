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
        restoreFollowCamera();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Return))
            PauseMenu.instance.activate(!PauseMenu.instance.getState());

        car.drive(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));

        if (car.enabled && Input.GetKeyDown(KeyCode.Space)) car.honk.Play();
        if (Input.GetKeyUp(KeyCode.Space)) car.honk.Stop();
    }

    public void restoreFollowCamera()
    {
        FollowCamera followCamera = Camera.main.GetComponent<FollowCamera>();
        followCamera.transform.position = transform.position;
        followCamera.transform.position -= transform.forward * followCamera.distanceXZ;
        followCamera.transform.position += transform.up * followCamera.distanceY;
    }
}
