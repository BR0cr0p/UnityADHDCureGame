using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandleFlameFlicker : MonoBehaviour
{
    public Light light;
    public float baseIntensity = 2;

    void Update()
    {
        light.intensity = baseIntensity + Random.Range(-0.3f, 0.3f);
    }
}

