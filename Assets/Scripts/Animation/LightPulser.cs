using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class LightPulser : MonoBehaviour
{

    public float amplitude, speed, cutoff;
    private Light light;
    void Start()
    {
        light = GetComponent<Light>();
    }

    void Update()
    {
        light.intensity = ((Mathf.Sin(Time.time * speed) + 1) / 2 + cutoff) * amplitude;
    }
}