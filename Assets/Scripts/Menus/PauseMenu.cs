using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    private bool menuToggle;

    private ActionManager actionManager;
    private bool CanInvoke = true;

    public void TogglePauseMenu() {
        menuToggle = !menuToggle;


    }

    public void MainMenu() {

    }

    public void Quit() {
        Application.Quit();

        Debug.Log("Quit");
    }
}