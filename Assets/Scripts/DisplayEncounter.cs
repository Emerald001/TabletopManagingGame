using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayEncounter : MonoBehaviour {
    public Canvas canvas;
    public Button buttonPrefab;

    public EncounterSO currentEncounter;
    public GameObject currentSurrouning;

    public List<GameObject> buttons;
    public int index;

    public void OnEnable() {
        EventManager<EncounterSO>.Subscribe(EventType.ON_CARAVAN_STOPPED, SetOrder<EncounterSO>);
    }

    public void OnDisable() {
        EventManager<EncounterSO>.Unsubscribe(EventType.ON_CARAVAN_STOPPED, SetOrder<EncounterSO>);
    }

    public void Update() {

    }

    public void DisplayTitle(EncounterSO encounter) {
        canvas.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = encounter.name;
    }

    public void DisplayOptions(EncounterSO encounter) {
        for (int i = 0; i < encounter.options.Count; i++) {
            var tmp = Instantiate(buttonPrefab, canvas.transform);
            tmp.name = "Button " + i.ToString();
            tmp.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = encounter.options[i].Description;

            tmp.GetComponent<RectTransform>().localPosition = new Vector3(0, 150 + i * -200, 0);

            var tmpButton = tmp.GetComponent<Button>();
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
        StartCoroutine(RollEncounter());
    }

    public void SetOrder<T>(EncounterSO encounter) {
        index = 0;

        StartCoroutine(MoveDown(encounter));
    }

    public IEnumerator MoveDown(EncounterSO encounter) {
        var tmp = Instantiate(encounter.SurroundingPrefab).transform;
        currentSurrouning = tmp.gameObject;
        tmp.position = new Vector3(0, 4, .5f);

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
        canvas.gameObject.SetActive(true);

        DisplayOptions(encounter);
        DisplayTitle(currentEncounter);

        while (transform.position.y != 1.68) {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, 1.68f, transform.position.z), 10 * Time.deltaTime);

            yield return new WaitForEndOfFrame();

            if (transform.position.y == 1.68f)
                break;
        }
    }

    public IEnumerator RollEncounter() {
        while (transform.position.y != 3.68f) {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, 3.68f, transform.position.z), 10 * Time.deltaTime);

            yield return new WaitForEndOfFrame();

            if (transform.position.y == 3.68f)
                break;
        }

        canvas.gameObject.SetActive(false);

        RemoveOptions();

        StartCoroutine(RemoveSurrounding());
    }

    public IEnumerator RemoveSurrounding() {
        while (currentSurrouning.transform.position.y != 3.6) {
            currentSurrouning.transform.position = Vector3.MoveTowards(currentSurrouning.transform.position, new Vector3(currentSurrouning.transform.position.x, 3.6f, currentSurrouning.transform.position.z), 25 * Time.deltaTime);

            yield return new WaitForEndOfFrame();

            if (currentSurrouning.transform.position.y == 3.6f)
                break;
        }

        Destroy(currentSurrouning);
        EventManager.Invoke(EventType.ON_ENCOUNTED_ENDED);
    }
}