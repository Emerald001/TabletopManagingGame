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

    public void ToggleSettingsMenu() {
        CurrentlyActive = !CurrentlyActive;

        if (CurrentlyActive) {
            MainMenuToggle.transform.position = MainSidePos.position;
            SettingsToggle.transform.position = SettingStandardPos.position;
        }
        else {
            MainMenuToggle.transform.position = MainStandardPos.position;
            SettingsToggle.transform.position = SettingHiddenPos.position;
        }

        SettingsToggle.SetActive(CurrentlyActive);
    }

    public void ToggleFullscreen() {
        isFullscreen = !isFullscreen;

        Check.SetActive(isFullscreen);
    }
}