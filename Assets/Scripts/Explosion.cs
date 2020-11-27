using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    private ParticleSystem[] particles;
    private AudioSource sound;

    void Awake()
    {
        enabled = false;
        particles = GetComponentsInChildren<ParticleSystem>();
        sound = GetComponent<AudioSource>();
    }

    public void Explode()
    {
        foreach (ParticleSystem ps in particles)
        {
            ps.Play();
        }
        sound.Play();
    }

    public bool isPlaying()
    {
        return particles[0].isPlaying;
    }
}
