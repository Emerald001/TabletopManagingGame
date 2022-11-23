using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayEncounter : MonoBehaviour
{
    public Canvas canvas;
    public Button buttonPrefab;

    public EncounterSO currentEncounter;

    public List<GameObject> buttons;

    public void Update() {
        if (Input.GetKeyDown(KeyCode.L)) {
            canvas.gameObject.SetActive(true);

            DisplayTitle(currentEncounter);
            DisplayOptions(currentEncounter);
        }
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
        RemoveOptions();

        canvas.gameObject.SetActive(false);
    }
}