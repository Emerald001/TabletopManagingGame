using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverOverHouse : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool isHovering;

    public GameObject HouseCanvas;
    public StartingArea owner;

    private Vector3 StartingPos;
    private Vector3 UpperPos;

    public void Init() {
        StartingPos = transform.position;
        UpperPos = StartingPos + new Vector3(0, .2f, 0);
    }

    private void Update() {
        if (isHovering)
            transform.position = Vector3.MoveTowards(transform.position, UpperPos, 40 * Time.deltaTime);
        else
            transform.position = Vector3.MoveTowards(transform.position, StartingPos, 40 * Time.deltaTime);
        
        if(isHovering && Input.GetKeyDown(KeyCode.Mouse0)) {
            owner.ClickOnHouse(this);
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        isHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData) {
        isHovering = false;
    }
}