using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiquidStack : MonoBehaviour
{
    public GameObject Resource;

    public int addAmount;
    public int removeAmount;

    [Range(0.01f, 1f)]
    public float size;

    private void Update() {
        Resource.transform.localPosition = new Vector3(0, size, 0);
        Resource.transform.localScale = new Vector3(.7f, size, .7f);
    }

    public void AddLiquid(float amount) {

    }

    public void RemoveLiquid(float amount) {

    }
}