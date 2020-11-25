using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyerBelt : MonoBehaviour
{
    Material mat;

    void Awake()
    {
        mat = GetComponent<Renderer>().material;
    }

    void Update()
    {
        mat.SetFloat("_ElapsedTime", Time.time);
    }
}
