using UnityEngine;

public class CameraManager : MonoBehaviour {
    [SerializeField] private Transform mainPos;
    [SerializeField] private Transform caravanPos;
    [SerializeField] private Transform resourcePos;
    [SerializeField] private Transform mainMenuPos;
    [SerializeField] private Transform questPos;
    [SerializeField] private Vector3 cardRot;

    [SerializeField] private float cameraMoveSpeed;

    private Transform currentPos;
    private Transform maincam;

    private bool canMove;

    private void OnEnable() {
        EventManager<CaravanEventType>.Subscribe(CaravanEventType.ON_GAME_STARTED, SetInGame);
        EventManager<CaravanEventType>.Subscribe(CaravanEventType.ON_GAME_PAUSED, SetMainMenu);
        EventManager<CaravanEventType>.Subscribe(CaravanEventType.ON_GAME_UNPAUSED, SetInGame);
        EventManager<CameraEventType, bool>.Subscribe(CameraEventType.SET_INTERACTABLE, SetInteractable);
    }
    private void OnDisable() {
        EventManager<CaravanEventType>.Unsubscribe(CaravanEventType.ON_GAME_STARTED, SetInGame);
        EventManager<CaravanEventType>.Unsubscribe(CaravanEventType.ON_GAME_PAUSED, SetMainMenu);
        EventManager<CaravanEventType>.Unsubscribe(CaravanEventType.ON_GAME_UNPAUSED, SetInGame);
        EventManager<CameraEventType, bool>.Unsubscribe(CameraEventType.SET_INTERACTABLE, SetInteractable);
    }

    private void Start() {
        maincam = Camera.main.transform;

        SetMainMenu();
    }

    private void Update() {
        if (canMove)
            ReadMoveInput();

        maincam.position = Vector3.MoveTowards(maincam.position, currentPos.position, cameraMoveSpeed * Time.deltaTime);
        maincam.eulerAngles = Vector3.MoveTowards(maincam.eulerAngles, currentPos.eulerAngles, cameraMoveSpeed * 15 * Time.deltaTime);
    }

    private void SetMainMenu() {
        canMove = false;
        currentPos = mainMenuPos;
    }

    private void SetInGame() {
        canMove = true;
        currentPos = mainPos;
    }

    private void SetInteractable(bool interactable) {
        canMove = interactable;
    }

    private void ReadMoveInput() {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetAxisRaw("Mouse ScrollWheel") > 0) {
            if (currentPos == mainPos)
                SetCameraGoal(CameraPositions.caravanPos);
            else if (currentPos == caravanPos)
                SetCameraGoal(CameraPositions.resourcePos);
        }

        if (Input.GetKeyDown(KeyCode.S) || Input.GetAxisRaw("Mouse ScrollWheel") < 0) {
            if (currentPos == caravanPos)
                SetCameraGoal(CameraPositions.mainPos);
            else if (currentPos == resourcePos)
                SetCameraGoal(CameraPositions.caravanPos);
        }

        if (Input.GetKeyDown(KeyCode.A)) {
            if (currentPos == caravanPos || currentPos == mainPos)
                SetCameraGoal(CameraPositions.questPos);
        }
        if (Input.GetKeyDown(KeyCode.D)) {
            if (currentPos == questPos)
                SetCameraGoal(CameraPositions.mainPos);
        }
    }

    // Can be accessed by other scripts to display certain things
    public void SetCameraGoal(CameraPositions goal) {
        GameManager.Instance.AudioManager.PlayAudio("QuickTransition");

        currentPos = (goal) switch {
            CameraPositions.mainPos => mainPos,
            CameraPositions.caravanPos => caravanPos,
            CameraPositions.resourcePos => resourcePos,
            CameraPositions.mainMenuPos => mainMenuPos,
            CameraPositions.questPos => questPos,
            _ => throw new System.NotImplementedException()
        };
    }
}

public enum CameraPositions {
    mainPos,
    caravanPos,
    resourcePos,
    mainMenuPos,
    questPos
}
