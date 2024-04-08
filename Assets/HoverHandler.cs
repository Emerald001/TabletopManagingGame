using UnityEngine;
using UnityEngine.EventSystems;

public class HoverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    public bool IsHovering { get; private set; }

    public void OnPointerEnter(PointerEventData eventData) {
        IsHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData) {
        IsHovering = false;
    }
}
