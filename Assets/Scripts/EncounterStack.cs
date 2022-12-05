using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EncounterStack : MonoBehaviour
{
    public bool CanClick;
    public bool Hovering;

    public Transform Card;
    public Transform CardPos;
    public Transform LiftedCardPos;
    public Transform DisplayPos;

    private List<EncounterSO> allEncounters;

    private void OnEnable() {
        EventManager.Subscribe(EventType.ON_ENCOUNTER_ENDED, ResetClick);
    }
    private void OnDisable() {
        EventManager.Unsubscribe(EventType.ON_ENCOUNTER_ENDED, ResetClick);
    }

    void Start() {
        var tmp = Resources.FindObjectsOfTypeAll<EncounterSO>();

        allEncounters = new(tmp);

        Debug.Log(allEncounters.Count);
    }

    void Update() {
        if (!CanClick) {
            Card.position = Vector3.MoveTowards(Card.position, DisplayPos.position, 10 * Time.deltaTime);
            Card.rotation = Quaternion.Lerp(Card.rotation, DisplayPos.rotation, 30 * Time.deltaTime);
            return;
        }

        CheckForHover();

        if (Hovering) {
            Card.position = Vector3.MoveTowards(Card.position, LiftedCardPos.position, Time.deltaTime);
            Card.rotation = Quaternion.Lerp(Card.rotation, LiftedCardPos.rotation, 15 * Time.deltaTime);
        }
        else {
            Card.position = Vector3.MoveTowards(Card.position, CardPos.position, Time.deltaTime);
            Card.rotation = Quaternion.Lerp(Card.rotation, CardPos.rotation, 15 * Time.deltaTime);
        }

        if(Hovering && Input.GetKeyDown(KeyCode.Mouse0)) {
            EventManager<EncounterSO>.Invoke(EventType.ON_ENCOUNTER_STARTED, GetEncounter());

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

    public void ResetClick() => CanClick = true;
}