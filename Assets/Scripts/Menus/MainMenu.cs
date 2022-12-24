using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject Menu;
    public EncounterSO StartEncounter;

    public void Continue() {
        Menu.SetActive(false);

        EventManager.Invoke(EventType.ON_GAME_STARTED);
    }

    public void NewGame() {
        Menu.SetActive(false);

        EventManager<EncounterSO>.Invoke(EventType.ON_GAME_STARTED, StartEncounter);
        EventManager<bool>.Invoke(EventType.ON_GAME_STARTED, true);
        EventManager.Invoke(EventType.ON_GAME_STARTED);
    }

    public void Quit() {
        Application.Quit();

        Debug.Log("Quit");
    }
}
