using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Driver : MonoBehaviour
{
    [HideInInspector] public Car car;
    [HideInInspector] public int lastCheckpoint;
    [HideInInspector] public int laps;
    [HideInInspector] public int racePosition;

    public bool isPlayer
    {
        get
        {
            return this == LevelParser.instance.player;
        }
    }

    public int nextCheckpoint
    {
        get
        {
            int nextDriverCheckpoint = lastCheckpoint + 1;
            if (nextDriverCheckpoint >= LevelParser.instance.checkpointOrigins.Length)
            {
                return 0;
            }

            return nextDriverCheckpoint;
        }
    }

    protected int startingPos;

    public void Initialize(Car car, int startingPos)
    {
        this.car = car;
        this.startingPos = startingPos;

        this.car.transform.position = LevelParser.instance.startingPoints[startingPos].position;
        this.car.transform.forward = LevelParser.instance.startingPoints[startingPos].forward;

        car.driver = this;
        car.color = 0;
        laps = -1;
        racePosition = startingPos;
        lastCheckpoint = -1;

        Ready();


    }

    protected virtual void Ready()
    {
        Debug.Log("READY DRIVER");
    }



    public void Kill()
    {
        Debug.LogError("KILL");
    }




}
