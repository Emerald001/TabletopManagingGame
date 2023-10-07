using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookInteract : MonoBehaviour {
    [SerializeField] private RectTransform book;
    [SerializeField] private RectTransform InfoHolder;

    [SerializeField] private Transform StandardPos;
    [SerializeField] private Transform LiftedPos;
    [SerializeField] private Transform HiddenPos;

    [SerializeField] private RectTransform displayPos;
    [SerializeField] private RectTransform hiddenPos;

    private readonly List<GameObject> allInfoBits = new();
    private int index = 0;

    private bool clickable;
    private bool hovering;
    private bool displaying;

    private void OnEnable() {
        EventManager<CaravanEventType, bool>.Subscribe(CaravanEventType.ON_GAME_STARTED, SetInteractable);
        EventManager<CaravanEventType, bool>.Subscribe(CaravanEventType.ON_GAME_PAUSED, SetInteractable);
        EventManager<CaravanEventType, bool>.Subscribe(CaravanEventType.ON_GAME_UNPAUSED, SetInteractable);
    }
    private void OnDisable() {
        EventManager<CaravanEventType, bool>.Unsubscribe(CaravanEventType.ON_GAME_STARTED, SetInteractable);
        EventManager<CaravanEventType, bool>.Unsubscribe(CaravanEventType.ON_GAME_PAUSED, SetInteractable);
        EventManager<CaravanEventType, bool>.Unsubscribe(CaravanEventType.ON_GAME_UNPAUSED, SetInteractable);
    }

    private void Start() {
        for (int i = 0; i < InfoHolder.childCount; i++)
            allInfoBits.Add(InfoHolder.GetChild(i).gameObject);
    }

    private void Update() {
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

        if (hovering && Input.GetKeyDown(KeyCode.Mouse0))
            Switch();
    }

    private void SetInteractable(bool bl) => clickable = bl;

    private void Switch() {
        if (displaying) {
            StartCoroutine(ActivateBook(hiddenPos, StandardPos.position));
            GameManager.Instance.AudioManager.PlayAudio("BookClose");
        }
        else {
            StartCoroutine(ActivateBook(displayPos, HiddenPos.position));
            GameManager.Instance.AudioManager.PlayAudio("BookOpen");
        }

        displaying = !displaying;
    }

    private void CheckForHover() {
        Ray target = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(target, out var hit, 10000)) {
            if (hit.transform.gameObject == gameObject)
                hovering = true;
            else
                hovering = false;
        }
    }

    private IEnumerator ActivateBook(RectTransform targetPos, Vector3 bookTarget) {
        EventManager<CaravanEventType, float>.Invoke(CaravanEventType.DO_SCREENSHAKE, .05f);

        while (book.position != targetPos.position) {
            book.position = Vector2.MoveTowards(book.position, targetPos.position, 5000 * Time.deltaTime);
            transform.position = Vector3.MoveTowards(transform.position, bookTarget, 10 * Time.deltaTime);

            yield return new WaitForEndOfFrame();
        }
    }

    public void Btn_NextInfoBit(int dir) {
        allInfoBits[index].SetActive(false);

        GameManager.Instance.AudioManager.PlayAudio("PageTurn");

        index += dir;
        if (index > allInfoBits.Count - 1)
            index = 0;
        else if (index < 0)
            index = allInfoBits.Count - 1;

        allInfoBits[index].SetActive(true);
    }
}