using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;

public class EncounterStack : MonoBehaviour
{
    public bool CanClick;
    public bool Hovering;
    public bool HoveringOldStack;
    public bool Displaying;

    public GameObject CardPrefab;
    public Transform CurrentCardPos;
    public Transform CurrentLiftedCardPos;
    public Transform DisplayPos;
    public Transform SlamDownPos;
    public Transform EndCardPos;

    public GameObject PastStack;

    private List<GameObject> nextEncounters = new();
    private List<GameObject> pastEncounters = new();

    private AreaSO currentArea;
    private int currentEncounterIndex;

    [HideInInspector] public Transform CurrentCard;
    [HideInInspector] public Transform lastCard;

    private Transform NewCardStackPos;

    private ActionManager actionQueue;

    private void OnEnable() {
        EventManager.Subscribe(EventType.ON_ENCOUNTER_ENDED, ResetClickOnEncounterEnded);
        EventManager<AreaSO>.Subscribe(EventType.SET_AREA, SetArea);
        //EventManager<EncounterSO>.Subscribe(EventType.ON_GAME_STARTED, StartEncounter);
    }
    private void OnDisable() {
        EventManager.Unsubscribe(EventType.ON_ENCOUNTER_ENDED, ResetClickOnEncounterEnded);
        EventManager<AreaSO>.Unsubscribe(EventType.SET_AREA, SetArea);
        EventManager<EncounterSO>.Unsubscribe(EventType.ON_GAME_STARTED, StartEncounter);
    }

    void Start() {
        actionQueue = new(EmptyQueue);

        NewCardStackPos = transform;

        //CurrentCard = Instantiate(CardPrefab).transform;
        //CurrentCard.SetPositionAndRotation(CurrentCardPos.position, CurrentCardPos.rotation);
    }

    void Update() {
        actionQueue.OnUpdate();

        if (!CanClick) 
            return;

        if (Displaying) {
            if (Input.GetKeyDown(KeyCode.Mouse0))
                StartCoroutine(ResetCards());
            return;
        }

        CheckForHover();

        if (Hovering) {
            CurrentCard.SetPositionAndRotation(Vector3.MoveTowards(CurrentCard.position, CurrentLiftedCardPos.position, Time.deltaTime), Quaternion.Lerp(CurrentCard.rotation, CurrentLiftedCardPos.rotation, 20 * Time.deltaTime));
        }
        else {
            CurrentCard.SetPositionAndRotation(Vector3.MoveTowards(CurrentCard.position, CurrentCardPos.position, Time.deltaTime), Quaternion.Lerp(CurrentCard.rotation, CurrentCardPos.rotation, 20 * Time.deltaTime));
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

    public void EmptyQueue() {

    }

    public void StartEncounter(EncounterSO encounter) {
        GameManager.instance.Amanager.PlayAudio("CardGrab");

        EventManager<EncounterSO>.Invoke(EventType.ON_ENCOUNTER_STARTED, encounter);

        CurrentCard.GetChild(1).GetComponent<TextMeshPro>().text = encounter.name;
        CurrentCard.GetChild(0).GetComponent<SpriteRenderer>().sprite = encounter.Icon;
        actionQueue.Enqueue(new MoveObjectAction(CurrentCard.gameObject, 10, DisplayPos));

        CanClick = false;
    }

    public void ResetClickOnEncounterEnded() {
        CanClick = false;

        actionQueue.Enqueue(new MoveObjectAction(CurrentCard.gameObject, 10, SlamDownPos));
        actionQueue.Enqueue(new WaitAction(.3f));
        actionQueue.Enqueue(new DoMethodAction(() => GameManager.instance.Amanager.PlayAudio("CardSlam")));
        actionQueue.Enqueue(new MoveObjectAction(CurrentCard.gameObject, 10, EndCardPos));
    }

    public void SetArea(AreaSO area) {
        currentArea = area;

        currentEncounterIndex = 0;

        SpawnAllCards();
    }

    public void SpawnAllCards() {
        for (int i = 0; i < currentArea.EncounterAmount; i++) {
            var tmp = Instantiate(CardPrefab);
            tmp.transform.position = transform.position + new Vector3(0, 3, 0);

            var tmpTf = new GameObject().transform;

            tmpTf.transform.position = transform.position + new Vector3(0, i * .01f + .01f, 0);
            tmpTf.transform.eulerAngles = transform.eulerAngles + new Vector3(0, Random.Range(-5, 5), 0);

            actionQueue.Enqueue(new MoveObjectAction(tmp, 20f, tmpTf, "", .08f));
            actionQueue.Enqueue(new DestoyObjectAction(tmpTf.gameObject));

            nextEncounters.Add(tmp);
        }
    }

    public EncounterSO GetEncouterFromArea() {
        var tmp = currentArea.ScriptedEncounters;

        if (tmp.Count < currentEncounterIndex)
            throw new System.Exception($"Should not be possible! New area should already be set!");

        if (tmp[currentEncounterIndex] == null)
            return currentArea.PossibleEncounters[SkewedRandomRange(currentArea.PossibleEncounters.Count)];

        return tmp[currentEncounterIndex];
    }

    private int SkewedRandomRange(float maxVal) {
        return Mathf.RoundToInt(Mathf.Pow(Random.value, maxVal) * (maxVal - 0) + 0);
    }

    #region Sequence
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