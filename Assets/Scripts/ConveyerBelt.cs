using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyerBelt : MonoBehaviour
{
    Material mat;

    void Awake()
    {
        mat = GetComponent<Renderer>().material;
        mat.SetFloat("_TimeScale", mat.GetFloat("_TimeScale") * -1f);
    }

    void Update()
    {
        mat.SetFloat("_ElapsedTime", Time.time);
    }
}
