using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameManager() {
        instance = this;
    }

    public Option startingResources;
    public ResourceManager Rmanager;
    public CaravanWalk Mmanager;
    public AudioManager Amanager;
    public DisplayEncounter EncounterDisplay;

    private EncounterSO currentEncounter;

    private void OnEnable() {
        EventManager<EncounterSO>.Subscribe(EventType.ON_ENCOUNTER_STARTED, SetEncounter);
        EventManager.Subscribe(EventType.ON_GAME_STARTED, Init);
        EventManager.Subscribe(EventType.ON_ENCOUNTER_ENDED, ResetEncounter);
    }

    private void OnDisable() {
        EventManager<EncounterSO>.Unsubscribe(EventType.ON_ENCOUNTER_STARTED, SetEncounter);
        EventManager.Unsubscribe(EventType.ON_GAME_STARTED, Init);
        EventManager.Unsubscribe(EventType.ON_ENCOUNTER_ENDED, ResetEncounter);
    }

    void Start() {
        Amanager.Init();

        Amanager.PlayLoopedAudio("BackgroundMusic", true);
    }

    public void Init() {
        Rmanager.AddResources(startingResources);
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.R)) {
            EventManager.Invoke(EventType.DO_SCREENSHAKE);
        }

        for (int i = 0; i < EncounterDisplay.buttons.Count; i++) {
            if (EncounterDisplay.buttons[i].GetComponent<OnButtonHover>().ShowResources) {
                Rmanager.ShowResources(currentEncounter.options[i]);
            }
        }
    }

    public void SetEncounter(EncounterSO encounter) {
        currentEncounter = encounter;
    }

    public void ResetEncounter() {
        currentEncounter = null;
    }
}