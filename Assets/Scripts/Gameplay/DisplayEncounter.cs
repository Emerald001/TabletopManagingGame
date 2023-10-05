using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayEncounter : MonoBehaviour {
    [Header("Refences")]
    public Canvas encounterCanvas;
    public Canvas outcomeCanvas;
    public Button buttonPrefab;

    public Transform BackgroundStandardTransform;
    public Transform BackgroundUpperTransform;
    public Transform ForegroundStandardTransform;
    public Transform ForegroundUpperTransform;
    public Transform CanvasStandardTransform;
    public Transform CanvasUpperTransform;

    public Color standardCandleColors;
    public List<Light> candleLights;

    [HideInInspector] public GameObject currentBackground;
    [HideInInspector] public GameObject currentSurrouning;

    [HideInInspector] public List<GameObject> buttons;
    [HideInInspector] public int index;

    [HideInInspector] public bool tmpboolWaiting = false;

    private EncounterSO currentEncounter;
    private ActionManager actionQueue;

    private void Start() {
        actionQueue = new(EndFunc);
    }

    public void OnEnable() {
        EventManager<EncounterSO>.Subscribe(EventType.ON_CARAVAN_STOPPED, SetQueue);
    }
    public void OnDisable() {
        EventManager<EncounterSO>.Unsubscribe(EventType.ON_CARAVAN_STOPPED, SetQueue);
    }

    public void Update() {
        actionQueue.OnUpdate();

        if (Input.GetKeyDown(KeyCode.Mouse0) && tmpboolWaiting) {
            tmpboolWaiting = false;

            EventManager.Invoke(EventType.NEXT_ACTION);
        }
    }

    public void SetQueue(EncounterSO encounter) {
        index = 0;
        currentEncounter = encounter;
        encounterCanvas.transform.position = CanvasUpperTransform.position;
        outcomeCanvas.transform.position = CanvasUpperTransform.position;

        currentBackground = Instantiate(encounter.BackgroundPrefab, new Vector3(0, 10, 0), Quaternion.identity);
        currentSurrouning = Instantiate(encounter.SurroundingPrefab, new Vector3(0, 4, .5f), Quaternion.identity);

        actionQueue.Enqueue(new DoMethodAction<Color>(SetColor, encounter.lightColor));
        actionQueue.Enqueue(new MoveObjectAction(currentBackground, 50, BackgroundStandardTransform.position, "BackgroundHit", .4f));
        actionQueue.Enqueue(new WaitAction(.2f));
        actionQueue.Enqueue(new MoveObjectAction(currentSurrouning, 25, ForegroundStandardTransform.position, "ForegroundHit", .2f));
        actionQueue.Enqueue(new WaitAction(.2f));

        if (encounter.options.Count > 0) {
            actionQueue.Enqueue(new DoMethodAction<EncounterSO>(DisplayOptions, encounter));
            actionQueue.Enqueue(new DoMethodAction<EncounterSO>(DisplayTitle, encounter));
            actionQueue.Enqueue(new MoveObjectAction(encounterCanvas.gameObject, 10, CanvasStandardTransform.position, "PaperRoll", 0));
            actionQueue.Enqueue(new WaitForCallAction(EventType.NEXT_ACTION));
        }
    }

    public void PartTwo(Option PickedOption) {
        actionQueue.Enqueue(new MoveObjectAction(encounterCanvas.gameObject, 10, CanvasUpperTransform.position, "PaperRoll", 0));
        actionQueue.Enqueue(new DoMethodAction(RemoveOptions));
        actionQueue.Enqueue(new DoMethodAction<Option>(DisplayOutcome, PickedOption));
        actionQueue.Enqueue(new MoveObjectAction(outcomeCanvas.gameObject, 10, CanvasStandardTransform.position, "PaperRoll", 0));
        actionQueue.Enqueue(new DoMethodAction(() => { tmpboolWaiting = true; }));
        actionQueue.Enqueue(new WaitForCallAction(EventType.NEXT_ACTION));
        actionQueue.Enqueue(new DoMethodAction<Option>(GameManager.instance.Rmanager.AddResources, PickedOption));
        actionQueue.Enqueue(new DoMethodAction(() => { EventManager.Invoke(PickedOption.EventToCall); }));
        actionQueue.Enqueue(new DoMethodAction(PartThree));
    }

    public void PartThree() {
        actionQueue.Enqueue(new MoveObjectAction(outcomeCanvas.gameObject, 10, CanvasUpperTransform.position, "PaperRoll", 0));
        actionQueue.Enqueue(new WaitAction(.2f));
        actionQueue.Enqueue(new MoveObjectAction(currentSurrouning, 25, ForegroundUpperTransform.position, "", 0));
        actionQueue.Enqueue(new DestoyObjectAction(currentSurrouning));
        actionQueue.Enqueue(new WaitAction(.2f));
        actionQueue.Enqueue(new MoveObjectAction(currentBackground, 50, BackgroundUpperTransform.position, "", 0));
        actionQueue.Enqueue(new DestoyObjectAction(currentBackground));
        actionQueue.Enqueue(new DoMethodAction<Color>(SetColor, standardCandleColors));
    }

    public void DisplayTitle(EncounterSO encounter) {
        encounterCanvas.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = encounter.name;
    }
    public void DisplayOutcome(Option ID) {
        outcomeCanvas.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = ID.Name;
        outcomeCanvas.transform.GetChild(3).GetComponent<TMPro.TextMeshProUGUI>().text = ID.Description;
    }

    public void DisplayOptions(EncounterSO encounter) {
        if (encounter.options.Count < 1)
            return;

        for (int i = 0; i < encounter.options.Count; i++) {
            Button button = Instantiate(buttonPrefab, encounterCanvas.transform);
            button.name = "Button " + i.ToString();
            button.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = encounter.options[i].Name;

            button.GetComponent<RectTransform>().localPosition = new Vector3(0, 150 + i * -200, 0);

            if (GameManager.instance.Rmanager.WoodStack.StackAmount < encounter.options[i].WoodUse ||
                GameManager.instance.Rmanager.MeatStack.StackAmount < encounter.options[i].MeatUse ||
                GameManager.instance.Rmanager.GoldStack.StackAmount < encounter.options[i].GoldUse ||
                GameManager.instance.Rmanager.WaterStack.StackAmount < encounter.options[i].WaterUse)
                button.interactable = false;

            int param = i;
            button.onClick.AddListener(delegate { ChoseOption(param); });

            buttons.Add(button.gameObject);
        }

        bool check = true;
        foreach (var item in buttons) {
            if (item.GetComponent<Button>().interactable == true)
                check = false;
        }

        if (check)
            EventManager.Invoke(EventType.DO_GAME_OVER);
    }

    public void RemoveOptions() {
        for (int i = 0; i < buttons.Count; i++) {
            var tmpButton = buttons[i].GetComponent<Button>();
            tmpButton.onClick.RemoveAllListeners();

            Destroy(buttons[i]);
        }

        buttons.Clear();
    }

    public void ChoseOption(int ID) {
        foreach (var item in buttons)
            item.GetComponent<Button>().interactable = false;

        GameManager.instance.Rmanager.RemoveResources(currentEncounter.options[ID]);
        PartTwo(currentEncounter.options[ID]);
        EventManager.Invoke(EventType.NEXT_ACTION);
    }

    public void EndFunc() {
        EventManager<EncounterSO>.Invoke(EventType.ON_ENCOUNTER_ENDED, currentEncounter);
        EventManager.Invoke(EventType.ON_ENCOUNTER_ENDED);
    }

    public void SetColor(Color color) {
        foreach (var item in candleLights)
            item.color = color;
    }
}