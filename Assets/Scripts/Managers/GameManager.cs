using UnityEngine;

public class GameManager : Singleton<GameManager> {
    [SerializeField] private Option startingResources;

    [SerializeField] private QuestSO currentQuest;

    [SerializeField] private ResourceManager Rmanager;
    [SerializeField] private MapBehaviorManager Mmanager;
    [SerializeField] private AudioManager Amanager;
    [SerializeField] private QuestManager Qmanager;
    [SerializeField] private DisplayEncounter EncounterDisplay;

    public ResourceManager ResourceManager => Rmanager;
    public MapBehaviorManager MapManager => Mmanager;
    public AudioManager AudioManager => Amanager;
    public QuestManager QuestManager => Qmanager;
    public DisplayEncounter DisplayEncounter => EncounterDisplay;

    private EncounterSO currentEncounter;

    private void OnEnable() {
        EventManager<CaravanEventType, EncounterSO>.Subscribe(CaravanEventType.ON_ENCOUNTER_STARTED, SetEncounter);
        EventManager<CaravanEventType>.Subscribe(CaravanEventType.ON_GAME_STARTED, Setup);
        EventManager<CaravanEventType>.Subscribe(CaravanEventType.ON_ENCOUNTER_ENDED, ResetEncounter);

        EventManager<CaravanEventType>.Subscribe(CaravanEventType.DO_GAME_OVER, () => Debug.Log("Game Over"));
    }

    private void OnDisable() {
        EventManager<CaravanEventType, EncounterSO>.Unsubscribe(CaravanEventType.ON_ENCOUNTER_STARTED, SetEncounter);
        EventManager<CaravanEventType>.Unsubscribe(CaravanEventType.ON_GAME_STARTED, Setup);
        EventManager<CaravanEventType>.Unsubscribe(CaravanEventType.ON_ENCOUNTER_ENDED, ResetEncounter);

        EventManager<CaravanEventType>.Unsubscribe(CaravanEventType.DO_GAME_OVER, () => Debug.Log("Game Over"));
    }

    private void Awake() {
        Init(this);
    }

    void Start() {
        Amanager.Init();

        Amanager.PlayLoopedAudio("BackgroundMusic", true);
    }

    private void Setup() {
        Rmanager.AddResources(startingResources);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.R))
            EventManager<CaravanEventType>.Invoke(CaravanEventType.DO_SCREENSHAKE);

        if (Input.GetKeyDown(KeyCode.H))
            EventManager<CaravanEventType, QuestSO>.Invoke(CaravanEventType.SET_QUEST, currentQuest);

        for (int i = 0; i < EncounterDisplay.Buttons.Count; i++) {
            if (EncounterDisplay.Buttons[i].GetComponent<OnButtonHover>().ShowResources)
                Rmanager.ShowResources(currentEncounter.options[i]);
        }
    }

    private void SetEncounter(EncounterSO encounter) {
        currentEncounter = encounter;
    }

    private void ResetEncounter() {
        currentEncounter = null;
    }
}