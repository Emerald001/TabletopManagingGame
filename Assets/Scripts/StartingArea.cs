using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StartingArea : MonoBehaviour
{
    public GameObject Village;

    public Transform UpperPos;

    public List<HoverOverHouse> houses;

    private ActionManager actionManager;
    private bool CanInvoke = true;

    private GameObject currentCanvas;

    private void OnEnable() {

    }
    private void OnDisable() {
        
    }

    private void Init() {
        actionManager = new(EndQueue);

        foreach (var item in houses) {
            item.Init();
        }
    }

    public void EndQueue() {
        CanInvoke = true;
    }

    public void ClickOnHouse(HoverOverHouse house) {
        if (currentCanvas) {
            actionManager.Enqueue(new MoveObjectAction(currentCanvas, 10,  UpperPos, "Swish"));
        }
        actionManager.Enqueue(new DoMethodAction(() => currentCanvas = house.HouseCanvas ));
        actionManager.Enqueue(new MoveObjectAction(currentCanvas, 10, transform, "Swish"));
    }
}