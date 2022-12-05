using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DisplayEncounter : MonoBehaviour {
    public Canvas encounterCanvas;
    public Canvas outcomeCanvas;
    public Button buttonPrefab;

    public Color standardCandleColors;
    public List<Light> candleLights;

    public GameObject currentBackground;
    public GameObject currentSurrouning;

    public List<GameObject> buttons;
    public int index;

    public bool tmpboolWaiting = false;
    public Option tmpCurrentPickedOption;
    
    private EncounterSO currentEncounter;

    public void OnEnable() {
        EventManager<EncounterSO>.Subscribe(EventType.ON_CARAVAN_STOPPED, SetOrder);
    }

    public void OnDisable() {
        EventManager<EncounterSO>.Unsubscribe(EventType.ON_CARAVAN_STOPPED, SetOrder);
    }

    public void Update() {
        if (Input.GetKeyDown(KeyCode.Mouse0) && tmpboolWaiting) {
            tmpboolWaiting = false;

            StartCoroutine(RemoveOutcome(tmpCurrentPickedOption));

            tmpCurrentPickedOption = new Option();
        }
    }

    public void DisplayTitle(EncounterSO encounter) {
        encounterCanvas.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = encounter.name;
    }
    public void DisplayOutcome(Option ID) {
        outcomeCanvas.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = ID.Name;
        outcomeCanvas.transform.GetChild(3).GetComponent<TMPro.TextMeshProUGUI>().text = ID.Description;
    }

    public void DisplayOptions(EncounterSO encounter) {
        for (int i = 0; i < encounter.options.Count; i++) {
            var tmp = Instantiate(buttonPrefab, encounterCanvas.transform);
            tmp.name = "Button " + i.ToString();
            tmp.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = encounter.options[i].Name;

            tmp.GetComponent<RectTransform>().localPosition = new Vector3(0, 150 + i * -200, 0);

            var tmpButton = tmp.GetComponent<Button>();

            if (GameManager.instance.Rmanager.WoodStack.StackAmount < encounter.options[i].WoodUse) {
                tmpButton.interactable = false;
            }
            else if (GameManager.instance.Rmanager.MeatStack.StackAmount < encounter.options[i].MeatUse) {
                tmpButton.interactable = false;
            }
            else if (GameManager.instance.Rmanager.GoldStack.StackAmount < encounter.options[i].GoldUse) {
                tmpButton.interactable = false;
            }

            var param = i;

            tmpButton.onClick.AddListener(delegate { ChoseOption(param); });

            buttons.Add(tmp.gameObject);
        }
    }

    public void RemoveOptions() {
        for (int i = 0; i < buttons.Count; i++) {
            var tmp = buttons[i];

            var tmpButton = tmp.GetComponent<Button>();
            tmpButton.onClick.RemoveAllListeners();

            Destroy(tmp);
        }

        buttons.Clear();
    }

    public void ChoseOption(int ID) {
        foreach (var item in buttons) {
            item.GetComponent<Button>().interactable = false;
        }

        StartCoroutine(RollEncounter(currentEncounter.options[ID]));
        GameManager.instance.Rmanager.RemoveResources(currentEncounter.options[ID]);

        tmpCurrentPickedOption = currentEncounter.options[ID];
    }

    public void SetOrder(EncounterSO encounter) {
        index = 0;
        currentEncounter = encounter;

        foreach (var item in candleLights) {
            StartCoroutine(SetColor(item, encounter.lightColor));
        }

        StartCoroutine(SpawnBackground(encounter));
    }

    public IEnumerator SetColor(Light light, Color color) {
        while (light.color != color) {
            light.color = Color.LerpUnclamped(light.color, color, Time.deltaTime);

            yield return new WaitForEndOfFrame();
        }
    }

    public IEnumerator SpawnBackground(EncounterSO encounter) {
        var tmp = Instantiate(encounter.BackgroundPrefab).transform;
        currentBackground = tmp.gameObject;
        tmp.position = new Vector3(0, 10, 0);

        GameManager.instance.Amanager.PlayAudio("BackgroundHit");

        while (tmp.position.y != 0) {
            tmp.position = Vector3.MoveTowards(tmp.position, new Vector3(tmp.position.x, 0, tmp.position.z), 50 * Time.deltaTime);

            yield return new WaitForEndOfFrame();

            if (tmp.position.y == 0)
                break;
        }

        EventManager<float>.Invoke(EventType.DO_SCREENSHAKE, .4f);

        yield return new WaitForSeconds(.2f);

        StartCoroutine(SpawnSurrounding(encounter));
    }

    public IEnumerator SpawnSurrounding(EncounterSO encounter) {
        var tmp = Instantiate(encounter.SurroundingPrefab).transform;
        currentSurrouning = tmp.gameObject;
        tmp.position = new Vector3(0, 4, .5f);

        GameManager.instance.Amanager.PlayAudio("ForegroundHit");

        while (tmp.position.y != 1.6) {
            tmp.position = Vector3.MoveTowards(tmp.position, new Vector3(tmp.position.x, 1.6f, tmp.position.z), 25 * Time.deltaTime);

            yield return new WaitForEndOfFrame();

            if (tmp.position.y == 1.6f)
                break;
        }

        EventManager<float>.Invoke(EventType.DO_SCREENSHAKE, .12f);

        yield return new WaitForSeconds(.2f);

        StartCoroutine(UnrollEncounter(encounter));
    }

    public IEnumerator UnrollEncounter(EncounterSO encounter) {
        transform.position += new Vector3(0, 2, 0);
        encounterCanvas.gameObject.SetActive(true);

        DisplayOptions(encounter);
        DisplayTitle(encounter);

        GameManager.instance.Amanager.PlayAudio("PaperRoll");

        while (transform.position.y != 1.68) {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, 1.68f, transform.position.z), 10 * Time.deltaTime);

            yield return new WaitForEndOfFrame();

            if (transform.position.y == 1.68f)
                break;
        }
    }

    public IEnumerator RollEncounter(Option pickedOption) {
        while (transform.position.y != 3.68f) {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, 3.68f, transform.position.z), 10 * Time.deltaTime);

            yield return new WaitForEndOfFrame();

            if (transform.position.y == 3.68f)
                break;
        }

        encounterCanvas.gameObject.SetActive(false);

        RemoveOptions();
        yield return new WaitForSeconds(.2f);

        StartCoroutine(ShowOutcome(pickedOption));
    }

    public IEnumerator RemoveSurrounding() {
        while (currentSurrouning.transform.position.y != 3.6) {
            currentSurrouning.transform.position = Vector3.MoveTowards(currentSurrouning.transform.position, new Vector3(currentSurrouning.transform.position.x, 3.6f, currentSurrouning.transform.position.z), 25 * Time.deltaTime);

            yield return new WaitForEndOfFrame();

            if (currentSurrouning.transform.position.y == 3.6f)
                break;
        }

        Destroy(currentSurrouning);
        yield return new WaitForSeconds(.2f);

        StartCoroutine(RemoveBackground());
    }

    public IEnumerator RemoveBackground() {
        while (currentBackground.transform.position.y != 10) {
            currentBackground.transform.position = Vector3.MoveTowards(currentBackground.transform.position, new Vector3(currentBackground.transform.position.x, 10, currentBackground.transform.position.z), 50 * Time.deltaTime);

            yield return new WaitForEndOfFrame();

            if (currentBackground.transform.position.y == 10)
                break;
        }

        foreach (var item in candleLights) {
            StartCoroutine(SetColor(item, standardCandleColors));
        }
        Destroy(currentBackground);

        EventManager.Invoke(EventType.ON_ENCOUNTER_ENDED);
    }

    public IEnumerator ShowOutcome(Option pickedOption) {
        transform.position += new Vector3(0, 2, 0);
        outcomeCanvas.gameObject.SetActive(true);

        DisplayOutcome(pickedOption);

        GameManager.instance.Amanager.PlayAudio("PaperRoll");

        while (transform.position.y != 1.68) {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, 1.68f, transform.position.z), 10 * Time.deltaTime);

            yield return new WaitForEndOfFrame();

            if (transform.position.y == 1.68f)
                break;
        }

        tmpboolWaiting = true;
    }

    public IEnumerator RemoveOutcome(Option pickedOption) {
        GameManager.instance.Rmanager.AddResources(pickedOption);

        while (transform.position.y != 3.68f) {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, 3.68f, transform.position.z), 10 * Time.deltaTime);

            yield return new WaitForEndOfFrame();

            if (transform.position.y == 3.68f)
                break;
        }

        outcomeCanvas.gameObject.SetActive(false);

        yield return new WaitForSeconds(.2f);

        StartCoroutine(RemoveSurrounding());
    }
}