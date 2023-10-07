using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OnButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    public bool ShowResources { get; private set; } = false;
    private Button button;

    private void Start() {
        button = GetComponent<Button>();
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (button.interactable)
            ShowResources = true;
    }

    public void OnPointerExit(PointerEventData eventData) {
        ShowResources = false;
    }
}