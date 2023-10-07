using UnityEngine;

public class MenuFunctionality : MonoBehaviour
{
    [Header("Toggle Menus")]
    [SerializeField] private QuestSO StartQuest;
    [SerializeField] private GameObject SettingsToggle;
    [SerializeField] private GameObject MainMenuToggle;
    [SerializeField] private GameObject PauseMenuToggle;

    [SerializeField] private Transform MainStandardPos;
    [SerializeField] private Transform MainHiddenPos;
    [SerializeField] private Transform MainSidePos;
    [SerializeField] private Transform SettingHiddenPos;
    [SerializeField] private Transform SettingStandardPos;

    [Header("Fullscreen Settings")]
    [SerializeField] private GameObject Check;

    private ActionQueue actionManager;

    private bool settingsMenuCurrentlyActive;
    private bool pauseMenuCurrentlyActive;
    private bool isFullscreen;
    private bool canPause;
    private bool canInvoke = true;

    private void OnEnable() {
        EventManager<CaravanEventType>.Subscribe(CaravanEventType.ON_GAME_STARTED, HideMenu);
    }
    private void OnDisable() {
        EventManager<CaravanEventType>.Unsubscribe(CaravanEventType.ON_GAME_STARTED, HideMenu);
    }

    private void Start() {
        actionManager = new(OnEmptyQueue);
    }

    private void Update() {
        actionManager.OnUpdate();

        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePauseMenu();    
    }

    private void OnEmptyQueue() {
        canInvoke = true;
    }

    private void TogglePauseMenu() {
        if (!canInvoke || !canPause)
            return;

        canInvoke = false;
        pauseMenuCurrentlyActive = !pauseMenuCurrentlyActive;

        if (pauseMenuCurrentlyActive) {
            actionManager.Enqueue(new MoveObjectAction(PauseMenuToggle, 10000, MainStandardPos));
            EventManager<CaravanEventType>.Invoke(CaravanEventType.ON_GAME_PAUSED);
            EventManager<CaravanEventType, bool>.Invoke(CaravanEventType.ON_GAME_PAUSED, false);
        }
        else {
            actionManager.Enqueue(new MoveObjectAction(PauseMenuToggle, 10000, MainHiddenPos));
            EventManager<CaravanEventType>.Invoke(CaravanEventType.ON_GAME_UNPAUSED);
            EventManager<CaravanEventType, bool>.Invoke(CaravanEventType.ON_GAME_UNPAUSED, true);
        }
    }

    private void HideMenu() {
        actionManager.Enqueue(new MoveObjectAction(SettingsToggle, 10000, SettingHiddenPos));
        actionManager.Enqueue(new MoveObjectAction(MainMenuToggle, 10000, MainHiddenPos));

        canPause = true;
    }

    public void Btn_Continue() {
        EventManager<CaravanEventType, QuestSO>.Invoke(CaravanEventType.ON_GAME_STARTED, StartQuest);
        EventManager<CaravanEventType, bool>.Invoke(CaravanEventType.ON_GAME_STARTED, true);
        EventManager<CaravanEventType>.Invoke(CaravanEventType.ON_GAME_STARTED);
    }

    public void Btn_NewGame() {
        EventManager<CaravanEventType, QuestSO>.Invoke(CaravanEventType.SET_QUEST, StartQuest);
        EventManager<CaravanEventType, bool>.Invoke(CaravanEventType.ON_GAME_STARTED, true);
        EventManager<CaravanEventType>.Invoke(CaravanEventType.ON_GAME_STARTED);
    }

    public void Btn_ToggleSettingsMenu(GameObject menu) {
        if (!canInvoke)
            return;

        canInvoke = false;
        settingsMenuCurrentlyActive = !settingsMenuCurrentlyActive;

        if (settingsMenuCurrentlyActive) {
            actionManager.Enqueue(new MoveObjectAction(menu, 10000, MainSidePos));
            actionManager.Enqueue(new MoveObjectAction(SettingsToggle, 10000, SettingStandardPos));
        }
        else {
            actionManager.Enqueue(new MoveObjectAction(SettingsToggle, 10000, SettingHiddenPos));
            actionManager.Enqueue(new MoveObjectAction(menu, 10000, MainStandardPos));
        }
    }

    public void Btn_Quit() {
        Application.Quit();

        Debug.Log("Quit");
    }

    public void Btn_ToggleFullscreen() {
        isFullscreen = !isFullscreen;

        Check.SetActive(isFullscreen);
    }
}