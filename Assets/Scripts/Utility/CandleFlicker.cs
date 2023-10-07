using UnityEngine;

public class CandleFlicker : MonoBehaviour
{
    [SerializeField] private Light currentLight;
    [SerializeField] private float StandardVal;
    [SerializeField] private float offset;

    private float noiseVal;

    private void Update() {
        noiseVal += Time.deltaTime * offset;

        var val = Mathf.PerlinNoise(noiseVal, 0) * 2 - 1;

        currentLight.intensity = StandardVal + val;
    }
}