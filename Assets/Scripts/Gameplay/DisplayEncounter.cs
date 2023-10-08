using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayEncounter : MonoBehaviour {
    [Header("Refences")]
    [SerializeField] private Canvas encounterCanvas;
    [SerializeField] private Canvas outcomeCanvas;
    [SerializeField] private Button buttonPrefab;

    [SerializeField] private Transform backgroundStandardTransform;
    [SerializeField] private Transform backgroundUpperTransform;
    [SerializeField] private Transform foregroundStandardTransform;
    [SerializeField] private Transform foregroundUpperTransform;
    [SerializeField] private Transform canvasStandardTransform;
    [SerializeField] private Transform canvasUpperTransform;

    [SerializeField] private Color standardCandleColors;
    [SerializeField] private List<Light> candleLights;

    public List<GameObject> Buttons { get; private set; } = new();

    private GameObject currentBackground;
    private GameObject currentSurrouning;
    private EncounterSO currentEncounter;
    private ActionQueue actionQueue;

    private bool tmpboolWaiting = false;

    private void Start() {
        actionQueue = new(EndFunc);
    }

    private void OnEnable() {
        EventManager<CaravanEventType, EncounterSO>.Subscribe(CaravanEventType.ON_CARAVAN_STOPPED, SetQueue);
    }
    private void OnDisable() {
        EventManager<CaravanEventType, EncounterSO>.Unsubscribe(CaravanEventType.ON_CARAVAN_STOPPED, SetQueue);
    }

    private void Update() {
        actionQueue.OnUpdate();

        if (Input.GetKeyDown(KeyCode.Mouse0) && tmpboolWaiting) {
            tmpboolWaiting = false;

            EventManager<CaravanEventType>.Invoke(CaravanEventType.NEXT_ACTION);
        }
    }

    private void SetQueue(EncounterSO encounter) {
        currentEncounter = encounter;
        encounterCanvas.transform.position = canvasUpperTransform.position;
        outcomeCanvas.transform.position = canvasUpperTransform.position;

        currentBackground = Instantiate(encounter.BackgroundPrefab, new Vector3(0, 10, 0), Quaternion.identity);
        currentSurrouning = Instantiate(encounter.SurroundingPrefab, new Vector3(0, 4, .5f), Quaternion.identity);

        actionQueue.Enqueue(new DoMethodAction<Color>(SetColor, encounter.lightColor));
        actionQueue.Enqueue(new DoMethodAction(() => GameManager.Instance.AudioManager.PlayAudio("BackgroundHit")));
        actionQueue.Enqueue(new MoveObjectAction(currentBackground, 50, backgroundStandardTransform.position));
        actionQueue.Enqueue(new DoMethodAction(() => EventManager<CaravanEventType, float>.Invoke(CaravanEventType.DO_SCREENSHAKE, .4f)));
        actionQueue.Enqueue(new WaitAction(.2f));
        actionQueue.Enqueue(new DoMethodAction(() => GameManager.Instance.AudioManager.PlayAudio("ForegroundHit")));
        actionQueue.Enqueue(new MoveObjectAction(currentSurrouning, 25, foregroundStandardTransform.position));
        actionQueue.Enqueue(new DoMethodAction(() => EventManager<CaravanEventType, float>.Invoke(CaravanEventType.DO_SCREENSHAKE, .2f)));
        actionQueue.Enqueue(new WaitAction(.2f));

        if (encounter.options.Count > 0) {
            actionQueue.Enqueue(new DoMethodAction<EncounterSO>(DisplayOptions, encounter));
            actionQueue.Enqueue(new DoMethodAction<EncounterSO>(DisplayTitle, encounter));
            actionQueue.Enqueue(new DoMethodAction(() => GameManager.Instance.AudioManager.PlayAudio("PaperRoll")));
            actionQueue.Enqueue(new MoveObjectAction(encounterCanvas.gameObject, 10, canvasStandardTransform.position));
            actionQueue.Enqueue(new WaitForCallAction<CaravanEventType>(CaravanEventType.NEXT_ACTION));
        }
    }

    private void PartTwo(Option PickedOption) {
        actionQueue.Enqueue(new DoMethodAction(() => GameManager.Instance.AudioManager.PlayAudio("PaperRoll")));
        actionQueue.Enqueue(new MoveObjectAction(encounterCanvas.gameObject, 10, canvasUpperTransform.position));
        actionQueue.Enqueue(new DoMethodAction(RemoveOptions));
        actionQueue.Enqueue(new DoMethodAction<Option>(DisplayOutcome, PickedOption));
        actionQueue.Enqueue(new DoMethodAction(() => GameManager.Instance.AudioManager.PlayAudio("PaperRoll")));
        actionQueue.Enqueue(new MoveObjectAction(outcomeCanvas.gameObject, 10, canvasStandardTransform.position));
        actionQueue.Enqueue(new DoMethodAction(() => { tmpboolWaiting = true; }));
        actionQueue.Enqueue(new WaitForCallAction<CaravanEventType>(CaravanEventType.NEXT_ACTION));
        actionQueue.Enqueue(new DoMethodAction<Option>(GameManager.Instance.ResourceManager.AddResources, PickedOption));
        actionQueue.Enqueue(new DoMethodAction(() => { EventManager<CaravanEventType>.Invoke(PickedOption.EventToCall); }));
        actionQueue.Enqueue(new DoMethodAction(PartThree));
    }

    private void PartThree() {
        actionQueue.Enqueue(new DoMethodAction(() => GameManager.Instance.AudioManager.PlayAudio("PaperRoll")));
        actionQueue.Enqueue(new MoveObjectAction(outcomeCanvas.gameObject, 10, canvasUpperTransform.position));
        actionQueue.Enqueue(new WaitAction(.2f));
        actionQueue.Enqueue(new MoveObjectAction(currentSurrouning, 25, foregroundUpperTransform.position));
        actionQueue.Enqueue(new DestoyObjectAction(currentSurrouning));
        actionQueue.Enqueue(new WaitAction(.2f));
        actionQueue.Enqueue(new MoveObjectAction(currentBackground, 50, backgroundUpperTransform.position));
        actionQueue.Enqueue(new DestoyObjectAction(currentBackground));
        actionQueue.Enqueue(new DoMethodAction<Color>(SetColor, standardCandleColors));
    }

    private void DisplayTitle(EncounterSO encounter) {
        encounterCanvas.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = encounter.name;
    }

    private void DisplayOutcome(Option ID) {
        outcomeCanvas.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = ID.Name;
        outcomeCanvas.transform.GetChild(3).GetComponent<TMPro.TextMeshProUGUI>().text = ID.Description;
    }

    private void DisplayOptions(EncounterSO encounter) {
        if (encounter.options.Count < 1)
            return;

        for (int i = 0; i < encounter.options.Count; i++) {
            Button button = Instantiate(buttonPrefab, encounterCanvas.transform);
            button.name = "Button " + i.ToString();
            button.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = encounter.options[i].Name;

            button.GetComponent<RectTransform>().localPosition = new Vector3(0, 150 + i * -200, 0);

            if (GameManager.Instance.ResourceManager.HasResources(
                encounter.options[i].WoodUse,
                encounter.options[i].MeatUse,
                encounter.options[i].GoldUse,
                encounter.options[i].WaterUse))
                    button.interactable = false;

            int param = i;
            button.onClick.AddListener(delegate { ChoseOption(param); });

            Buttons.Add(button.gameObject);
        }

        bool check = true;
        foreach (var item in Buttons) {
            if (item.GetComponent<Button>().interactable == true)
                check = false;
        }

        if (check)
            EventManager<CaravanEventType>.Invoke(CaravanEventType.DO_GAME_OVER);
    }

    private void RemoveOptions() {
        for (int i = 0; i < Buttons.Count; i++) {
            var tmpButton = Buttons[i].GetComponent<Button>();
            tmpButton.onClick.RemoveAllListeners();

            Destroy(Buttons[i]);
        }

        Buttons.Clear();
    }

    private void ChoseOption(int ID) {
        foreach (var item in Buttons)
            item.GetComponent<Button>().interactable = false;

        GameManager.Instance.ResourceManager.RemoveResources(currentEncounter.options[ID]);
        PartTwo(currentEncounter.options[ID]);
        EventManager<CaravanEventType>.Invoke(CaravanEventType.NEXT_ACTION);
    }

    private void EndFunc() {
        EventManager<CaravanEventType, EncounterSO >.Invoke(CaravanEventType.ON_ENCOUNTER_ENDED, currentEncounter);
        EventManager<CaravanEventType>.Invoke(CaravanEventType.ON_ENCOUNTER_ENDED);
    }

    private void SetColor(Color color) {
        foreach (var item in candleLights)
            item.color = color;
    }
}