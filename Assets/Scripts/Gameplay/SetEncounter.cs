using UnityEngine;

public class SetEncounter : MonoBehaviour
{
    public EncounterSO setEncounter;

    void Update() {
        if (Input.GetKeyDown(KeyCode.L)) 
            EventManager<CaravanEventType, EncounterSO>.Invoke(CaravanEventType.ON_ENCOUNTER_STARTED, setEncounter);
    }
}