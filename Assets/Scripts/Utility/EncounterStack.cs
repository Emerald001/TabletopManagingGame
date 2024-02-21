using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EncounterStack : MonoBehaviour {
    [Header("References")]
    [SerializeField] private QuestSO Village;

    [SerializeField] private GameObject CardPrefab;
    [SerializeField] private Transform DisplayPos;
    [SerializeField] private Transform SlamDownPos;
    [SerializeField] private Transform EndCardPos;

    [SerializeField] private GameObject PastStack;

    private readonly List<GameObject> nextEncounters = new();
    private readonly List<GameObject> pastEncounters = new();
    private readonly List<Transform> nextEncountersTransforms = new();

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

    private ActionQueue actionQueue;

    private void OnEnable() {
        EventManager<CaravanEventType, QuestSO>.Subscribe(CaravanEventType.SET_QUEST, SetQuest);
        EventManager<CaravanEventType>.Subscribe(CaravanEventType.ON_ENCOUNTER_ENDED, ResetClickOnEncounterEnded);
    }
    private void OnDisable() {
        EventManager<CaravanEventType, QuestSO>.Unsubscribe(CaravanEventType.SET_QUEST, SetQuest);
        EventManager<CaravanEventType>.Unsubscribe(CaravanEventType.ON_ENCOUNTER_ENDED, ResetClickOnEncounterEnded);
    }

    private void Start() {
        CurrentLiftedCardPos = new GameObject().transform;
        CurrentLiftedCardPos.SetParent(transform);

        actionQueue = new(EmptyQueue);
    }

    private void Update() {
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

    private void CheckForHover() {
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

    private void EmptyQueue() {

    }

    private void SetNextCardActive() {
        if (nextEncounters.Count < 1) {
            Debug.Log("No More Encounters, going back to village!");
            EventManager<CaravanEventType, QuestSO>.Invoke(CaravanEventType.SET_QUEST, Village);
        }

        CurrentCard = nextEncounters[^1].transform;
        CurrentCardPos = nextEncountersTransforms[^1];
        CurrentLiftedCardPos.position = CurrentCardPos.position + new Vector3(0, .01f, 0);

        CanClick = true;
    }

    private void StartEncounter(EncounterSO encounter) {
        GameManager.Instance.AudioManager.PlayAudio("CardGrab");

        EventManager<CaravanEventType, EncounterSO>.Invoke(CaravanEventType.ON_ENCOUNTER_STARTED, encounter);

        CurrentCard.GetChild(1).GetComponent<TextMeshPro>().text = encounter.name;
        CurrentCard.GetChild(0).GetComponent<SpriteRenderer>().sprite = encounter.Icon;
        actionQueue.Enqueue(new MoveObjectAction(CurrentCard.gameObject, 10, DisplayPos));

        nextEncounters.Remove(CurrentCard.gameObject);
        nextEncountersTransforms.Remove(CurrentCardPos);
        Destroy(CurrentCardPos.gameObject);

        CanClick = false;
    }

    private void ResetClickOnEncounterEnded() {
        actionQueue.Enqueue(new MoveObjectAction(CurrentCard.gameObject, 10, SlamDownPos));
        actionQueue.Enqueue(new WaitAction(.3f));
        actionQueue.Enqueue(new DoMethodAction(() => GameManager.Instance.AudioManager.PlayAudio("CardSlam")));
        actionQueue.Enqueue(new MoveObjectAction(CurrentCard.gameObject, 10, EndCardPos));
        actionQueue.Enqueue(new DoMethodAction(() => EventManager<CaravanEventType, float>.Invoke(CaravanEventType.DO_SCREENSHAKE, .3f)));
        actionQueue.Enqueue(new DoMethodAction(SetNextCardActive));
    }

    private void SetQuest(QuestSO quest) {
        currentQuest = quest;
        currentArea = Instantiate(quest.area);

        currentEncounterIndex = 0;

        SpawnAllCards();
    }

    private void SpawnAllCards() {
        for (int i = 0; i < currentQuest.EncounterAmount; i++) {
            GameObject card = Instantiate(CardPrefab, transform);
            card.transform.position = transform.position + new Vector3(0, 3, 0);

            Transform tmpTf = new GameObject().transform;

            tmpTf.transform.position = transform.position + new Vector3(0, i * .01f + .01f, 0);
            tmpTf.transform.eulerAngles = transform.eulerAngles + new Vector3(0, Random.Range(-5, 5), 0);

            actionQueue.Enqueue(new MoveObjectAction(card, 20f, tmpTf));
            actionQueue.Enqueue(new DoMethodAction(() => EventManager<CaravanEventType, float>.Invoke(CaravanEventType.DO_SCREENSHAKE, .08f)));

            nextEncounters.Add(card);
            nextEncountersTransforms.Add(tmpTf);
        }

        SetNextCardActive();
    }

    private EncounterSO GetEncouterFromArea() {
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