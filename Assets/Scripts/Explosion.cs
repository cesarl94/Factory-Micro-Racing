using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    private ParticleSystem[] particles;
    void Awake()
    {
        enabled = false;
        particles = GetComponentsInChildren<ParticleSystem>();
    }

    public void Explode()
    {
        foreach (ParticleSystem ps in particles)
        {
            ps.Play();
        }
    }

    public bool isPlaying()
    {
        return particles[0].isPlaying;
    }
}
