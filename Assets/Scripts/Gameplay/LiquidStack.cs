using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiquidStack : MonoBehaviour
{
    public GameObject Resource;

    [Range(0.01f, .45f)]
    public float size = 0.01f;

    private float targetHeight;

    [HideInInspector] public float StackAmount;

    private void Update() {
        targetHeight = Mathf.Lerp(targetHeight, size, .5f * Time.deltaTime);

        Resource.transform.localPosition = new Vector3(0, targetHeight, 0);
        Resource.transform.localScale = new Vector3(.7f, targetHeight, .7f);
    }

    public void AddLiquid(float amount) {
        if (size + amount / 100 < .45f)
            size += amount / 100;
        else
            size = .45f;

        StackAmount = size * 100;
    }

    public void RemoveLiquid(float amount) {
        if(size > amount / 100)
            size -= amount / 100;

        StackAmount = size * 100;
    }
}