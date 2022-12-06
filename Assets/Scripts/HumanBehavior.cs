using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanBehavior : MonoBehaviour {
    public float walkspeed;
    public float menAmount;
    public GameObject man;

    public CaravanWalk CaravanWalk;
    public AnimationCurve wobble;

    private List<GameObject> dudes = new();
    private float currentSpeed;
    private bool encounterActive;

    private void OnEnable() {
        EventManager<EncounterSO>.Subscribe(EventType.ON_CARAVAN_STOPPED, AddedEncounter);
        EventManager.Subscribe(EventType.ON_ENCOUNTER_ENDED, RemovedEncounter);
    }
    private void OnDisable() {
        EventManager<EncounterSO>.Unsubscribe(EventType.ON_CARAVAN_STOPPED, AddedEncounter);
        EventManager.Unsubscribe(EventType.ON_ENCOUNTER_ENDED, RemovedEncounter);
    }

    void Start() {
        for (int i = 0; i < menAmount; i++) {
            AddMan();
        }
    }

    void Update() {
        currentSpeed = CaravanWalk.currentsSpeed;

        if (encounterActive)
            GatherAroundObstacle();
        else
            UpdateManPositions();
    }

    private void UpdateManPositions() {
        for (int i = dudes.Count - 1; i >= 0; i--) {
            var item = dudes[i];

            item.transform.position = Vector3.MoveTowards(item.transform.position, new Vector3(CaravanWalk.End.transform.position.x, item.transform.position.y, item.transform.position.z), currentSpeed / 300 * Time.deltaTime);

            if (Random.Range(0, 1000 * (1 - Time.deltaTime)) < 1) {
                StartCoroutine(WalkBack(item, new Vector3(Random.Range(0, -1.3f), 1.65f, Random.Range(-.25f, -.75f))));
            }

            if (item.transform.position.x > 1)
                StartCoroutine(WalkBack(item, new Vector3(Random.Range(0, -1.3f), 1.65f, Random.Range(-.25f, -.75f))));
        }
    }

    public void AddedEncounter(EncounterSO encounter) {
        encounterActive = true;

        var tmpPos = CaravanWalk.currentObstacle.transform.position + new Vector3(.2f, 0, 0);
        for (int i = 0; i < dudes.Count; i++) {
            var dude = dudes[i];
            dude.transform.LookAt(tmpPos);
        }
    }
    public void RemovedEncounter() {
        encounterActive = false;
    }

    public void AddMan() {
        var tmp = Instantiate(man);
        tmp.transform.position = new Vector3(0, 1.65f, -.35f);

        dudes.Add(tmp);
    }

    public IEnumerator WalkBack(GameObject dude, Vector3 targetPos) {
        dudes.Remove(dude);
        dude.GetComponent<Animator>().SetBool("Wobbling", true);

        dude.transform.LookAt(targetPos);

        while (Vector3.Distance(dude.transform.position, targetPos) > .01f) {
            dude.transform.position = Vector3.MoveTowards(dude.transform.position, targetPos, walkspeed * Time.deltaTime);

            yield return new WaitForEndOfFrame();
        }

        dudes.Add(dude);
        dude.GetComponent<Animator>().SetBool("Wobbling", false);
    }

    public void GatherAroundObstacle() {
        var tmpPos = CaravanWalk.currentObstacle.transform.position + new Vector3(.4f, 0, 0);

        for (int i = 0; i < dudes.Count; i++) {
            var dude = dudes[i];

            if (Vector3.Distance(dude.transform.position, tmpPos + new Vector3(0, 0, (-(menAmount / 2) + i) * .1f)) > .01f) {
                dude.transform.position = Vector3.MoveTowards(dude.transform.position, tmpPos + new Vector3(0, 0, (-(menAmount / 2) + i) * .1f), walkspeed * 4 * Time.deltaTime);
                dude.GetComponent<Animator>().SetBool("Wobbling", true);
            }
            else {
                dude.GetComponent<Animator>().SetBool("Wobbling", false);
            }
        }
    }
}