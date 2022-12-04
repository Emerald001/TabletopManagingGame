using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetEncounter : MonoBehaviour
{
    public EncounterSO setEncounter;

    void Start() {
        
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.L)) {
            EventManager<EncounterSO>.Invoke(EventType.ON_ENCOUNTER_STARTED, setEncounter);
        }
    }
}