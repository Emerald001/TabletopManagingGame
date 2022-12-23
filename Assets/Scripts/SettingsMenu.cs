using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsMenu : MonoBehaviour {
    [Header("Toggle SettingsMenu")]
    public GameObject SettingsToggle;
    public GameObject MainMenuToggle;

    public Transform MainStandardPos;
    public Transform MainSidePos;
    public Transform SettingHiddenPos;
    public Transform SettingStandardPos;

    public bool CurrentlyActive = false;

    [Header("Fullscreen Settings")]
    public GameObject Check;
    public bool isFullscreen = false;

    private ActionManager actionManager;
    private bool CanInvoke = true;

    private void Start() {
        actionManager = new(OnEmptyQueue);
    }

    private void Update() {
        actionManager.OnUpdate();
    }

    public void ToggleSettingsMenu() {
        if (!CanInvoke)
            return;

        CanInvoke = false;
        CurrentlyActive = !CurrentlyActive;

        if (CurrentlyActive) {
            actionManager.Enqueue(new MoveObjectAction(MainMenuToggle, 10000, MainSidePos));
            actionManager.Enqueue(new MoveObjectAction(SettingsToggle, 10000, SettingStandardPos));
        }
        else {
            actionManager.Enqueue(new MoveObjectAction(SettingsToggle, 10000, SettingHiddenPos));
            actionManager.Enqueue(new MoveObjectAction(MainMenuToggle, 10000, MainStandardPos));
        }
    }

    public void ToggleFullscreen() {
        isFullscreen = !isFullscreen;

        Check.SetActive(isFullscreen);
    }

    public void OnEmptyQueue() {
        CanInvoke = true;
    }
}