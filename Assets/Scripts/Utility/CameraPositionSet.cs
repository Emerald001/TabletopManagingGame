using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPositionSet : MonoBehaviour
{
    public Transform MainPos;
    public Transform CaravanPos;
    public Transform ResourcePos;
    public Transform MainMenuPos;
    public Transform QuestPos;
    public Vector3 CardRot;

    public float cameraMoveSpeed;

    private Transform CurrentPos;
    private Transform Maincam;

    private bool InMainMenu;

    private void OnEnable() {
        EventManager.Subscribe(EventType.ON_GAME_STARTED, SetInGame);
        EventManager.Subscribe(EventType.ON_GAME_PAUSED, MainMenu);
        EventManager.Subscribe(EventType.ON_GAME_UNPAUSED, SetInGame);
    }
    private void OnDisable() {
        EventManager.Unsubscribe(EventType.ON_GAME_STARTED, SetInGame);
        EventManager.Unsubscribe(EventType.ON_GAME_PAUSED, MainMenu);
        EventManager.Unsubscribe(EventType.ON_GAME_UNPAUSED, SetInGame);
    }

    private void Start() {
        Maincam = Camera.main.transform;

        CurrentPos = MainPos;

        MainMenu();
    }

    void Update() {
        if (!InMainMenu)
            Ingame();

        Maincam.position = Vector3.MoveTowards(Maincam.position, CurrentPos.position, cameraMoveSpeed * Time.deltaTime);
        Maincam.eulerAngles = Vector3.MoveTowards(Maincam.eulerAngles, CurrentPos.eulerAngles, cameraMoveSpeed * 15 * Time.deltaTime);
    }

    public void MainMenu() {
        InMainMenu = true;

        CurrentPos = MainMenuPos;
    }

    public void SetInGame() {
        InMainMenu = false;

        CurrentPos = MainPos;
    }

    public void Ingame() {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetAxisRaw("Mouse ScrollWheel") > 0) {
            if (CurrentPos == MainPos) {
                GameManager.instance.Amanager.PlayAudio("QuickTransition");
                CurrentPos = CaravanPos;
            }
            else if (CurrentPos == CaravanPos) {
                GameManager.instance.Amanager.PlayAudio("QuickTransition");
                CurrentPos = ResourcePos;
            }
        }

        if (Input.GetKeyDown(KeyCode.S) || Input.GetAxisRaw("Mouse ScrollWheel") < 0) {
            if (CurrentPos == CaravanPos) {
                GameManager.instance.Amanager.PlayAudio("QuickTransition");
                CurrentPos = MainPos;
            }
            else if (CurrentPos == ResourcePos) {
                GameManager.instance.Amanager.PlayAudio("QuickTransition");
                CurrentPos = CaravanPos;
            }
        }

        if (Input.GetKeyDown(KeyCode.A)) {
            if (CurrentPos == CaravanPos || CurrentPos == MainPos) {
                GameManager.instance.Amanager.PlayAudio("QuickTransition");
                CurrentPos = QuestPos;
            }
        }
        if (Input.GetKeyDown(KeyCode.D)) {
            if (CurrentPos == QuestPos) {
                GameManager.instance.Amanager.PlayAudio("QuickTransition");
                CurrentPos = MainPos;
            }
        }
    }
}