using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EncounterStack : MonoBehaviour
{
    public bool CanClick;
    public bool Hovering;

    public GameObject CardPrefab;
    public Transform CardPos;
    public Transform LiftedCardPos;
    public Transform DisplayPos;
    public Transform SlamDownPos;
    public Transform EndCardPos;

    private List<EncounterSO> allEncounters;
    [HideInInspector] public Transform CurrentCard;
    [HideInInspector] public Transform lastCard;

    private void OnEnable() {
        EventManager.Subscribe(EventType.ON_ENCOUNTER_ENDED, ResetClick);
    }
    private void OnDisable() {
        EventManager.Unsubscribe(EventType.ON_ENCOUNTER_ENDED, ResetClick);
    }

    void Start() {
        CurrentCard = Instantiate(CardPrefab).transform;

        CurrentCard.position =  CardPos.position;
        CurrentCard.rotation = CardPos.rotation;

        var tmp = Resources.LoadAll<EncounterSO>("Encounters");

        allEncounters = new(tmp);
    }

    void Update() {
        if (!CanClick)
            return;

        CheckForHover();

        if (Hovering) {
            CurrentCard.position = Vector3.MoveTowards(CurrentCard.position, LiftedCardPos.position, Time.deltaTime);
            CurrentCard.rotation = Quaternion.Lerp(CurrentCard.rotation, LiftedCardPos.rotation, 15 * Time.deltaTime);
        }
        else {
            CurrentCard.position = Vector3.MoveTowards(CurrentCard.position, CardPos.position, Time.deltaTime);
            CurrentCard.rotation = Quaternion.Lerp(CurrentCard.rotation, CardPos.rotation, 15 * Time.deltaTime);
        }

        if(Hovering && Input.GetKeyDown(KeyCode.Mouse0)) {
            EventManager<EncounterSO>.Invoke(EventType.ON_ENCOUNTER_STARTED, GetEncounter());

            StartCoroutine(MoveCard(DisplayPos, false, false));

            CanClick = false;
        }
    }

    public void CheckForHover() { 
        Ray target = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(target, out hit, 10000)) {
            if (hit.transform.gameObject == this.gameObject)
                Hovering = true;
            else
                Hovering = false;
        }
    }

    public EncounterSO GetEncounter() {
        return allEncounters[Random.Range(0, allEncounters.Count)];
    }

    public void ResetClick() {
        StartCoroutine(MoveCard(SlamDownPos, true, false));
    }

    private IEnumerator MoveCard(Transform targetPos, bool slam, bool end) {
        while (CurrentCard.position != targetPos.position) {
            CurrentCard.position = Vector3.MoveTowards(CurrentCard.position, targetPos.position, 10 * Time.deltaTime);
            CurrentCard.rotation = Quaternion.Lerp(CurrentCard.rotation, targetPos.rotation, 30 * Time.deltaTime);

            yield return new WaitForEndOfFrame();
        }

        if (slam) {
            yield return new WaitForSeconds(.3f);
            StartCoroutine(MoveCard(EndCardPos, false, true));
        }

        if (end) {
            EventManager<float>.Invoke(EventType.DO_SCREENSHAKE, .2f);

            if(lastCard)
                Destroy(lastCard.gameObject);

            lastCard = CurrentCard;

            CurrentCard = Instantiate(CardPrefab).transform;

            CurrentCard.position = CardPos.position;
            CurrentCard.rotation = CardPos.rotation;

            CanClick = true;
        }
    }
}