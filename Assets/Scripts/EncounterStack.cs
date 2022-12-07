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
        EventManager<EncounterSO>.Subscribe(EventType.ON_GAME_STARTED, StartEncounter);
    }
    private void OnDisable() {
        EventManager.Unsubscribe(EventType.ON_ENCOUNTER_ENDED, ResetClick);
        EventManager<EncounterSO>.Unsubscribe(EventType.ON_GAME_STARTED, StartEncounter);
    }

    void Start() {
        CurrentCard = Instantiate(CardPrefab).transform;
        CurrentCard.SetPositionAndRotation(CardPos.position, CardPos.rotation);

        var tmp = Resources.LoadAll<EncounterSO>("Encounters");

        allEncounters = new(tmp);
    }

    void Update() {
        if (!CanClick) 
            return;

        CheckForHover();

        if (Hovering) {
            CurrentCard.SetPositionAndRotation(Vector3.MoveTowards(CurrentCard.position, LiftedCardPos.position, Time.deltaTime), Quaternion.Lerp(CurrentCard.rotation, LiftedCardPos.rotation, 15 * Time.deltaTime));
        }
        else {
            CurrentCard.SetPositionAndRotation(Vector3.MoveTowards(CurrentCard.position, CardPos.position, Time.deltaTime), Quaternion.Lerp(CurrentCard.rotation, CardPos.rotation, 15 * Time.deltaTime));
        }

        if(Hovering && Input.GetKeyDown(KeyCode.Mouse0)) {
            StartEncounter(allEncounters[Random.Range(0, allEncounters.Count)]);
        }
    }

    public void CheckForHover() { 
        Ray target = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(target, out var hit, 10000)) {
            if (hit.transform.gameObject == gameObject)
                Hovering = true;
            else
                Hovering = false;
        }
    }

    public void StartEncounter(EncounterSO encounter) {
        EventManager<EncounterSO>.Invoke(EventType.ON_ENCOUNTER_STARTED, encounter);

        StartCoroutine(MoveCard(DisplayPos, false, false));

        CanClick = false;
    }

    public void ResetClick() {
        CanClick = false;

        StartCoroutine(MoveCard(SlamDownPos, true, false));
    }
     
    private IEnumerator MoveCard(Transform targetPos, bool slam, bool end) {
        while (CurrentCard.position != targetPos.position) {
            CurrentCard.SetPositionAndRotation(Vector3.MoveTowards(CurrentCard.position, targetPos.position, 10 * Time.deltaTime), Quaternion.Lerp(CurrentCard.rotation, targetPos.rotation, 30 * Time.deltaTime));

            yield return new WaitForEndOfFrame();
        }

        if (slam) {
            yield return new WaitForSeconds(.3f);

            GameManager.instance.Amanager.PlayAudio("ObstacleHit");
            StartCoroutine(MoveCard(EndCardPos, false, true));
        }

        if (end) {
            EventManager<float>.Invoke(EventType.DO_SCREENSHAKE, .2f);

            if(lastCard)
                Destroy(lastCard.gameObject);

            lastCard = CurrentCard;

            CurrentCard = Instantiate(CardPrefab).transform;

            CurrentCard.SetPositionAndRotation(CardPos.position, CardPos.rotation);

            CanClick = true;
        }
    }
}