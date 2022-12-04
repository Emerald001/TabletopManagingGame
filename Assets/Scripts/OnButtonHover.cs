using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OnButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [HideInInspector] public bool ShowResources = false;
    private Button button;

    public void Start() {
        button = GetComponent<Button>();
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if(button.interactable)
            ShowResources = true;
    }
    public void OnPointerExit(PointerEventData eventData) {
        ShowResources = false;
    }
}