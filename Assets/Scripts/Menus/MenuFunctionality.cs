using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuFunctionality : MonoBehaviour
{
    [Header("Toggle SettingsMenu")]
    public EncounterSO StartEncounter;
    public GameObject SettingsToggle;
    public GameObject MainMenuToggle;
    public GameObject PauseMenuToggle;

    public Transform MainStandardPos;
    public Transform MainHiddenPos;
    public Transform MainSidePos;
    public Transform SettingHiddenPos;
    public Transform SettingStandardPos;

    public bool SettingsMenuCurrentlyActive = false;
    public bool PauseMenuCurrentlyActive = false;

    [Header("Fullscreen Settings")]
    public GameObject Check;
    public bool isFullscreen = false;

    private ActionManager actionManager;
    private bool CanInvoke = true;
    public bool CanPause = false;

    private void OnEnable() {
        EventManager.Subscribe(EventType.ON_GAME_STARTED, HideMenu);
    }
    private void OnDisable() {
        EventManager.Unsubscribe(EventType.ON_GAME_STARTED, HideMenu);
    }

    private void Start() {
        actionManager = new(OnEmptyQueue);
    }

    private void Update() {
        actionManager.OnUpdate();

        if (Input.GetKeyDown(KeyCode.P)) {
            TogglePauseMenu();
        } 
    }

    public void OnEmptyQueue() {
        Debug.Log("Empty Queue");

        CanInvoke = true;
    }

    public void Continue() {
        EventManager<EncounterSO>.Invoke(EventType.ON_GAME_STARTED, StartEncounter);
        EventManager<bool>.Invoke(EventType.ON_GAME_STARTED, true);
        EventManager.Invoke(EventType.ON_GAME_STARTED);
    }

    public void NewGame() {
        EventManager<EncounterSO>.Invoke(EventType.ON_GAME_STARTED, StartEncounter);
        EventManager<bool>.Invoke(EventType.ON_GAME_STARTED, true);
        EventManager.Invoke(EventType.ON_GAME_STARTED);
    }

    public void ToggleSettingsMenu(GameObject menu) {
        if (!CanInvoke)
            return;

        CanInvoke = false;
        SettingsMenuCurrentlyActive = !SettingsMenuCurrentlyActive;

        if (SettingsMenuCurrentlyActive) {
            actionManager.Enqueue(new MoveObjectAction(menu, 10000, MainSidePos, "", 0));
            actionManager.Enqueue(new MoveObjectAction(SettingsToggle, 10000, SettingStandardPos, "", 0));
        }
        else {
            actionManager.Enqueue(new MoveObjectAction(SettingsToggle, 10000, SettingHiddenPos, "", 0));
            actionManager.Enqueue(new MoveObjectAction(menu, 10000, MainStandardPos, "", 0));
        }
    }

    public void TogglePauseMenu() {
        if (!CanInvoke || !CanPause)
            return;

        CanInvoke = false;
        PauseMenuCurrentlyActive = !PauseMenuCurrentlyActive;

        if (PauseMenuCurrentlyActive) {
            actionManager.Enqueue(new MoveObjectAction(PauseMenuToggle, 10000, MainStandardPos, "", 0));
            EventManager.Invoke(EventType.ON_GAME_PAUSED);
            EventManager<bool>.Invoke(EventType.ON_GAME_PAUSED, false);
        }
        else {
            actionManager.Enqueue(new MoveObjectAction(PauseMenuToggle, 10000, MainHiddenPos, "", 0));
            EventManager.Invoke(EventType.ON_GAME_UNPAUSED);
            EventManager<bool>.Invoke(EventType.ON_GAME_UNPAUSED, true);
        }
    }

    public void ToggleFullscreen() {
        isFullscreen = !isFullscreen;

        Check.SetActive(isFullscreen);
    }

    public void HideMenu() {
        actionManager.Enqueue(new MoveObjectAction(SettingsToggle, 10000, SettingHiddenPos, "", 0));
        actionManager.Enqueue(new MoveObjectAction(MainMenuToggle, 10000, MainHiddenPos, "", 0));

        CanPause = true;
    }

    public void Quit() {
        Application.Quit();

        Debug.Log("Quit");
    }
}