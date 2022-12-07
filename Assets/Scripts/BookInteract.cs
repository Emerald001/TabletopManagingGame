using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BookInteract : MonoBehaviour
{
    public bool clickable;

    public RectTransform book;
    public RectTransform InfoHolder;

    public Transform StandardPos;
    public Transform LiftedPos;
    public Transform HiddenPos;

    public RectTransform displayPos;
    public RectTransform hiddenPos;

    private bool hovering;
    private bool displaying;

    private List<GameObject> allInfoBits = new();
    private int index = 0;

    private void OnEnable() {
        EventManager<bool>.Subscribe(EventType.ON_GAME_STARTED, SetInteractable);
    }
    private void OnDisable() {
        EventManager<bool>.Unsubscribe(EventType.ON_GAME_STARTED, SetInteractable);
    }

    private void Start() {
        for (int i = 0; i < InfoHolder.childCount; i++) {
            allInfoBits.Add(InfoHolder.GetChild(i).gameObject);
        }
    }

    void Update() {
        CheckForHover();

        if (displaying || !clickable)
            return;

        if (hovering) {
            transform.position = Vector3.MoveTowards(transform.position, LiftedPos.position, 5 * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, LiftedPos.rotation, 40 * Time.deltaTime);
        }
        else {
            transform.position = Vector3.MoveTowards(transform.position, StandardPos.position, 5 * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, StandardPos.rotation, 40 * Time.deltaTime);
        }

        if (hovering && Input.GetKeyDown(KeyCode.Mouse0)) {
            Switch();
        }
    }

    public void SetInteractable(bool bl) => clickable = bl;

    public void Switch() {

        if (displaying) {
            StartCoroutine(ActivateBook(hiddenPos, StandardPos.position));
            GameManager.instance.Amanager.PlayAudio("BookClose");
        }
        else {
            StartCoroutine(ActivateBook(displayPos, HiddenPos.position));
            GameManager.instance.Amanager.PlayAudio("BookOpen");
        }

        displaying = !displaying;
    }

    public void NextInfoBit(int dir) {
        allInfoBits[index].SetActive(false);

        index += dir;
        if (index > allInfoBits.Count - 1)
            index = 0;
        else if (index < 0)
            index = allInfoBits.Count - 1;

        allInfoBits[index].SetActive(true);
    }

    public void CheckForHover() {
        Ray target = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(target, out var hit, 10000)) {
            if (hit.transform.gameObject == gameObject)
                hovering = true;
            else
                hovering = false;
        }
    }

    public IEnumerator ActivateBook(RectTransform targetPos, Vector3 bookTarget) {
        EventManager<float>.Invoke(EventType.DO_SCREENSHAKE, .05f);

        while (book.position != targetPos.position) {
            book.position = Vector2.MoveTowards(book.position, targetPos.position, 5000 * Time.deltaTime);
            transform.position = Vector3.MoveTowards(transform.position, bookTarget, 10 * Time.deltaTime);
            
            yield return new WaitForEndOfFrame();
        }
    }
}