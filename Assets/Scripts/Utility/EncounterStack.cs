using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System.Linq;

public class EncounterStack : MonoBehaviour
{
    public bool CanClick;
    public bool Hovering;
    public bool HoveringOldStack;
    public bool Displaying;

    public GameObject CardPrefab;
    public Transform CardPos;
    public Transform LiftedCardPos;
    public Transform DisplayPos;
    public Transform SlamDownPos;
    public Transform EndCardPos;

    public GameObject PastStack;

    private List<GameObject> pastEncounters = new();

    private AreaSO currentArea;
    private int currentEncounterIndex;

    [HideInInspector] public Transform CurrentCard;
    [HideInInspector] public Transform lastCard;

    private void OnEnable() {
        EventManager.Subscribe(EventType.ON_ENCOUNTER_ENDED, ResetClick);
        EventManager<AreaSO>.Subscribe(EventType.SET_AREA, SetArea);
        //EventManager<EncounterSO>.Subscribe(EventType.ON_GAME_STARTED, StartEncounter);
    }
    private void OnDisable() {
        EventManager.Unsubscribe(EventType.ON_ENCOUNTER_ENDED, ResetClick);
        EventManager<AreaSO>.Unsubscribe(EventType.SET_AREA, SetArea);
        EventManager<EncounterSO>.Unsubscribe(EventType.ON_GAME_STARTED, StartEncounter);
    }

    void Start() {
        CurrentCard = Instantiate(CardPrefab).transform;
        CurrentCard.SetPositionAndRotation(CardPos.position, CardPos.rotation);
    }

    void Update() {
        if (!CanClick) 
            return;

        if (Displaying) {
            if (Input.GetKeyDown(KeyCode.Mouse0))
                StartCoroutine(ResetCards());
            return;
        }

        CheckForHover();

        if (Hovering) {
            CurrentCard.SetPositionAndRotation(Vector3.MoveTowards(CurrentCard.position, LiftedCardPos.position, Time.deltaTime), Quaternion.Lerp(CurrentCard.rotation, LiftedCardPos.rotation, 20 * Time.deltaTime));
        }
        else {
            CurrentCard.SetPositionAndRotation(Vector3.MoveTowards(CurrentCard.position, CardPos.position, Time.deltaTime), Quaternion.Lerp(CurrentCard.rotation, CardPos.rotation, 20 * Time.deltaTime));
        }

        if (Hovering && Input.GetKeyDown(KeyCode.Mouse0)) {
            StartEncounter(GetEncouterFromArea());

            currentEncounterIndex++;
        }

        if (HoveringOldStack && Input.GetKeyDown(KeyCode.Mouse0)) {
            StartCoroutine(DisplayCards());
        }
    }

    public void CheckForHover() { 
        Ray target = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(target, out var hit, 10000)) {
            if (hit.transform.gameObject == gameObject)
                Hovering = true;
            else
                Hovering = false;

            if (hit.transform.gameObject == PastStack)
                HoveringOldStack = true;
            else
                HoveringOldStack = false;
        }
    }

    public void StartEncounter(EncounterSO encounter) {
        GameManager.instance.Amanager.PlayAudio("CardGrab");

        EventManager<EncounterSO>.Invoke(EventType.ON_ENCOUNTER_STARTED, encounter);

        CurrentCard.GetChild(1).GetComponent<TextMeshPro>().text = encounter.name;
        CurrentCard.GetChild(0).GetComponent<SpriteRenderer>().sprite = encounter.Icon;
        StartCoroutine(MoveCard(DisplayPos, false, false));

        CanClick = false;
    }

    public void ResetClick() {
        CanClick = false;

        StartCoroutine(MoveCard(SlamDownPos, true, false));
    }

    public void SetArea(AreaSO area) {
        currentArea = area;

        currentEncounterIndex = 0;
    }

    public EncounterSO GetEncouterFromArea() {
        var tmp = currentArea.ScriptedEncounters;

        if (tmp.Count < currentEncounterIndex)
            throw new System.Exception($"Should not be possible! New area should already be set!");

        if (tmp[currentEncounterIndex] == null)
            return currentArea.PossibleEncounters[Random.Range(0, currentArea.PossibleEncounters.Count)];

        return tmp[currentEncounterIndex];
    }

    #region Sequence
    private IEnumerator MoveCard(Transform targetPos, bool slam, bool end) {
        while (CurrentCard.position != targetPos.position) {
            CurrentCard.SetPositionAndRotation(Vector3.MoveTowards(CurrentCard.position, targetPos.position, 10 * Time.deltaTime), Quaternion.Lerp(CurrentCard.rotation, targetPos.rotation, 30 * Time.deltaTime));

            yield return new WaitForEndOfFrame();
        }

        if (slam) {
            yield return new WaitForSeconds(.3f);

            GameManager.instance.Amanager.PlayAudio("CardSlam");

            var tmp = EndCardPos;
            StartCoroutine(MoveCard(tmp, false, true));
        }

        if (end) {
            EventManager<float>.Invoke(EventType.DO_SCREENSHAKE, .2f);

            CurrentCard.position = EndCardPos.position + new Vector3(0, pastEncounters.Count * .001f, 0);
            pastEncounters.Add(CurrentCard.gameObject);

            CurrentCard = Instantiate(CardPrefab).transform;
            CurrentCard.SetPositionAndRotation(CardPos.position, CardPos.rotation);

            CanClick = true;
        }
    }

    private IEnumerator DisplayCards() {
        float middle = pastEncounters.Count / 2;

        for (int i = 0; i < pastEncounters.Count; i++) {
            StartCoroutine(DisplayCards(pastEncounters[i], DisplayPos.position + new Vector3((middle - i) * .5f, 0, 0), DisplayPos.rotation));

            yield return new WaitForSeconds(.1f);
        }

        Displaying = true;
    }

    private IEnumerator ResetCards() {
        for (int i = 0; i < pastEncounters.Count; i++) {
            StartCoroutine(DisplayCards(pastEncounters[i], EndCardPos.position + new Vector3(0, i * .001f, 0), EndCardPos.rotation));

            yield return new WaitForSeconds(.1f);
        }

        Displaying = false;
    }

    private IEnumerator DisplayCards(GameObject Card, Vector3 targetPos, Quaternion rotation) {
        while (Card.transform.position != targetPos) {
            Card.transform.SetPositionAndRotation(
                Vector3.MoveTowards(Card.transform.position, targetPos, 25 * Time.deltaTime), 
                Quaternion.Lerp(Card.transform.rotation, rotation, 50 * Time.deltaTime));

            yield return new WaitForEndOfFrame();
        }
    }
    #endregion
}