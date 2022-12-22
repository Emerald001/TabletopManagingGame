using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandleFlicker : MonoBehaviour
{
    public Light light;
    public float StandardVal;
    public float offset;

    private float noiseVal;

    void Update() {
        noiseVal += Time.deltaTime * offset;

        var val = Mathf.PerlinNoise(noiseVal, 0) * 2 - 1;

        light.intensity = StandardVal + val;
    }
}