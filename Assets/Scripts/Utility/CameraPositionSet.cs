using UnityEngine;

public class CameraPositionSet : MonoBehaviour {
    [SerializeField] private Transform mainPos;
    [SerializeField] private Transform caravanPos;
    [SerializeField] private Transform resourcePos;
    [SerializeField] private Transform mainMenuPos;
    [SerializeField] private Transform questPos;
    [SerializeField] private Vector3 cardRot;

    [SerializeField] private float cameraMoveSpeed;

    private Transform currentPos;
    private Transform maincam;

    private bool inMainMenu;

    private void OnEnable() {
        EventManager<CaravanEventType>.Subscribe(CaravanEventType.ON_GAME_STARTED, SetInGame);
        EventManager<CaravanEventType>.Subscribe(CaravanEventType.ON_GAME_PAUSED, MainMenu);
        EventManager<CaravanEventType>.Subscribe(CaravanEventType.ON_GAME_UNPAUSED, SetInGame);
    }
    private void OnDisable() {
        EventManager<CaravanEventType>.Unsubscribe(CaravanEventType.ON_GAME_STARTED, SetInGame);
        EventManager<CaravanEventType>.Unsubscribe(CaravanEventType.ON_GAME_PAUSED, MainMenu);
        EventManager<CaravanEventType>.Unsubscribe(CaravanEventType.ON_GAME_UNPAUSED, SetInGame);
    }

    private void Start() {
        maincam = Camera.main.transform;

        currentPos = mainPos;

        MainMenu();
    }

    private void Update() {
        if (!inMainMenu)
            Ingame();

        maincam.position = Vector3.MoveTowards(maincam.position, currentPos.position, cameraMoveSpeed * Time.deltaTime);
        maincam.eulerAngles = Vector3.MoveTowards(maincam.eulerAngles, currentPos.eulerAngles, cameraMoveSpeed * 15 * Time.deltaTime);
    }

    private void MainMenu() {
        inMainMenu = true;

        currentPos = mainMenuPos;
    }

    private void SetInGame() {
        inMainMenu = false;

        currentPos = mainPos;
    }

    private void Ingame() {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetAxisRaw("Mouse ScrollWheel") > 0) {
            if (currentPos == mainPos) {
                GameManager.Instance.AudioManager.PlayAudio("QuickTransition");
                currentPos = caravanPos;
            }
            else if (currentPos == caravanPos) {
                GameManager.Instance.AudioManager.PlayAudio("QuickTransition");
                currentPos = resourcePos;
            }
        }

        if (Input.GetKeyDown(KeyCode.S) || Input.GetAxisRaw("Mouse ScrollWheel") < 0) {
            if (currentPos == caravanPos) {
                GameManager.Instance.AudioManager.PlayAudio("QuickTransition");
                currentPos = mainPos;
            }
            else if (currentPos == resourcePos) {
                GameManager.Instance.AudioManager.PlayAudio("QuickTransition");
                currentPos = caravanPos;
            }
        }

        if (Input.GetKeyDown(KeyCode.A)) {
            if (currentPos == caravanPos || currentPos == mainPos) {
                GameManager.Instance.AudioManager.PlayAudio("QuickTransition");
                currentPos = questPos;
            }
        }
        if (Input.GetKeyDown(KeyCode.D)) {
            if (currentPos == questPos) {
                GameManager.Instance.AudioManager.PlayAudio("QuickTransition");
                currentPos = mainPos;
            }
        }
    }
}