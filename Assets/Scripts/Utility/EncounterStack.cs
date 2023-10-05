using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EncounterStack : MonoBehaviour
{
    [Header("References")]
    public QuestSO Village;

    public GameObject CardPrefab;
    public Transform DisplayPos;
    public Transform SlamDownPos;
    public Transform EndCardPos;

    public GameObject PastStack;

    private List<GameObject> nextEncounters = new();
    private List<GameObject> pastEncounters = new();
    private List<Transform> nextEncountersTransforms = new();

    private AreaSO currentArea;
    private QuestSO currentQuest;
    private int currentEncounterIndex;

    private Transform CurrentCard;
    private Transform lastCard;

    private Transform CurrentCardPos;
    private Transform CurrentLiftedCardPos;

    private bool CanClick;
    private bool Hovering;
    private bool HoveringOldStack;
    private bool Displaying;

    private ActionManager actionQueue;

    private void OnEnable() {
        EventManager<QuestSO>.Subscribe(EventType.SET_QUEST, SetQuest);
        EventManager.Subscribe(EventType.ON_ENCOUNTER_ENDED, ResetClickOnEncounterEnded);
    }
    private void OnDisable() {
        EventManager<QuestSO>.Unsubscribe(EventType.SET_QUEST, SetQuest);
        EventManager.Unsubscribe(EventType.ON_ENCOUNTER_ENDED, ResetClickOnEncounterEnded);
    }

    void Start() {
        CurrentLiftedCardPos = new GameObject().transform;
        CurrentLiftedCardPos.SetParent(transform);

        actionQueue = new(EmptyQueue);
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
            CurrentCard.SetPositionAndRotation(
                Vector3.MoveTowards(
                    CurrentCard.position,
                    CurrentLiftedCardPos.position,
                    Time.deltaTime),
                Quaternion.Lerp(
                    CurrentCard.rotation,
                    CurrentLiftedCardPos.rotation,
                    20 * Time.deltaTime));
        }
        else {
            CurrentCard.SetPositionAndRotation(
                Vector3.MoveTowards(
                    CurrentCard.position,
                    CurrentCardPos.position,
                    Time.deltaTime),
                Quaternion.Lerp(
                    CurrentCard.rotation,
                    CurrentCardPos.rotation,
                    20 * Time.deltaTime));
        }

        if (Hovering && Input.GetKeyDown(KeyCode.Mouse0)) {
            StartEncounter(GetEncouterFromArea());

            currentEncounterIndex++;
        }

        if (HoveringOldStack && Input.GetKeyDown(KeyCode.Mouse0))
            StartCoroutine(DisplayCards());
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

    public void SetNextCardActive() {
        if (nextEncounters.Count < 1) {
            Debug.Log("No More Encounters, going back to village!");
            EventManager<QuestSO>.Invoke(EventType.SET_QUEST, Village);
        }

        CurrentCard = nextEncounters[^1].transform;
        CurrentCardPos = nextEncountersTransforms[^1];
        CurrentLiftedCardPos.position = CurrentCardPos.position + new Vector3(0, .01f, 0);

        CanClick = true;
    }

    public void StartEncounter(EncounterSO encounter) {
        Debug.Log(encounter);

        GameManager.instance.Amanager.PlayAudio("CardGrab");

        EventManager<EncounterSO>.Invoke(EventType.ON_ENCOUNTER_STARTED, encounter);

        CurrentCard.GetChild(1).GetComponent<TextMeshPro>().text = encounter.name;
        CurrentCard.GetChild(0).GetComponent<SpriteRenderer>().sprite = encounter.Icon;
        actionQueue.Enqueue(new MoveObjectAction(CurrentCard.gameObject, 10, DisplayPos));

        nextEncounters.Remove(CurrentCard.gameObject);
        nextEncountersTransforms.Remove(CurrentCardPos);
        Destroy(CurrentCardPos.gameObject);

        CanClick = false;
    }

    public void ResetClickOnEncounterEnded() {
        actionQueue.Enqueue(new MoveObjectAction(CurrentCard.gameObject, 10, SlamDownPos));
        actionQueue.Enqueue(new WaitAction(.3f));
        actionQueue.Enqueue(new DoMethodAction(() => GameManager.instance.Amanager.PlayAudio("CardSlam")));
        actionQueue.Enqueue(new MoveObjectAction(CurrentCard.gameObject, 10, EndCardPos, "", .3f));
        actionQueue.Enqueue(new DoMethodAction(SetNextCardActive));
    }

    public void SetQuest(QuestSO quest) {
        currentQuest = quest;
        currentArea = Instantiate(quest.area);

        currentEncounterIndex = 0;

        SpawnAllCards();
    }

    public void SpawnAllCards() {
        Debug.Log(currentQuest.EncounterAmount);
        for (int i = 0; i < currentQuest.EncounterAmount; i++) {

            GameObject card = Instantiate(CardPrefab, transform);
            card.transform.position = transform.position + new Vector3(0, 3, 0);

            Transform tmpTf = new GameObject().transform;

            tmpTf.transform.position = transform.position + new Vector3(0, i * .01f + .01f, 0);
            tmpTf.transform.eulerAngles = transform.eulerAngles + new Vector3(0, Random.Range(-5, 5), 0);

            actionQueue.Enqueue(new MoveObjectAction(card, 20f, tmpTf, "", .08f));

            nextEncounters.Add(card);
            nextEncountersTransforms.Add(tmpTf);
        }

        SetNextCardActive();
    }

    public EncounterSO GetEncouterFromArea() {
        List<EncounterSO> list = currentQuest.ScriptedEncounters;

        if (list.Count < currentEncounterIndex)
            throw new System.Exception($"Should not be possible! New <color=red>Quest</color> should already be set!");

        if (list[currentEncounterIndex] == null)
            return currentArea.PossibleEncounters[SkewedRandomRange(currentArea.PossibleEncounters.Count - 1)];

        return list[currentEncounterIndex];
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