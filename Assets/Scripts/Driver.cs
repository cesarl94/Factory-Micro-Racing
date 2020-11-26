using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Driver : MonoBehaviour
{
    [HideInInspector] public Car car;
    [HideInInspector] public int lastCheckpoint;
    [HideInInspector] public int laps;
    [HideInInspector] public int racePosition;
    [HideInInspector] public float deathTime;

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

    public void Initialize(Car car, int startingPos, int color)
    {
        this.car = car;
        this.startingPos = startingPos;

        car.transform.position = LevelParser.instance.startingPoints[startingPos].position;
        car.transform.forward = LevelParser.instance.startingPoints[startingPos].forward;

        car.driver = this;
        car.setColor(color);
        laps = -1;
        racePosition = startingPos;
        lastCheckpoint = -1;

        Ready();
    }

    protected virtual void Ready() { }

    public void Kill()
    {
        deathTime = Time.time;
        car.Explode();
    }

    public void Respawn()
    {
        car.Respawn();
    }


}
