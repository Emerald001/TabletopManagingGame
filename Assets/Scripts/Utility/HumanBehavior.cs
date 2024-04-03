using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanBehavior : MonoBehaviour {
    [SerializeField] private float walkspeed;
    [SerializeField] private GameObject man;
    [SerializeField] private Transform End;

    [SerializeField] private MapBehaviorManager CaravanWalk;

    private readonly List<GameObject> currentTeam = new();
    private bool encounterActive;

    private void OnEnable() {
        EventManager<CaravanEventType, EncounterSO>.Subscribe(CaravanEventType.ON_CARAVAN_STOPPED, AddedEncounter);
        EventManager<CaravanEventType>.Subscribe(CaravanEventType.DESTROY_HUMAN, RemoveMan);
        EventManager<CaravanEventType>.Subscribe(CaravanEventType.ON_ENCOUNTER_ENDED, RemovedEncounter);
    }
    private void OnDisable() {
        EventManager<CaravanEventType, EncounterSO>.Unsubscribe(CaravanEventType.ON_CARAVAN_STOPPED, AddedEncounter);
        EventManager<CaravanEventType>.Unsubscribe(CaravanEventType.DESTROY_HUMAN, RemoveMan);
        EventManager<CaravanEventType>.Unsubscribe(CaravanEventType.ON_ENCOUNTER_ENDED, RemovedEncounter);
    }

    private void Update() {
        if (encounterActive)
            GatherAroundObstacle();
        else
            UpdateManPositions();
    }

    private void UpdateManPositions() {
        for (int i = currentTeam.Count - 1; i >= 0; i--) {
            var item = currentTeam[i];

            item.transform.position = Vector3.MoveTowards(item.transform.position, new Vector3(End.transform.position.x, item.transform.position.y, item.transform.position.z), CaravanWalk.CurrentSpeed / 300 * Time.deltaTime);

            if (Random.Range(0, 1000 * (1 - Time.deltaTime)) < 1)
                StartCoroutine(WalkBack(item, new Vector3(Random.Range(0, -1.3f), 1.65f, Random.Range(-.25f, -.75f))));

            if (item.transform.position.x > 1)
                StartCoroutine(WalkBack(item, new Vector3(Random.Range(0, -1.3f), 1.65f, Random.Range(-.25f, -.75f))));
        }
    }

    private void AddedEncounter(EncounterSO encounter) {
        encounterActive = true;

        var tmpPos = CaravanWalk.CurrentObstacle.transform.position + new Vector3(.2f, 0, 0);
        for (int i = 0; i < currentTeam.Count; i++) {
            var dude = currentTeam[i];
            dude.transform.LookAt(tmpPos);
        }
    }

    private void RemovedEncounter() {
        encounterActive = false;
    }

    public void AddMan(GameObject manPrefab) {
        var tmp = Instantiate(manPrefab);
        tmp.transform.position = new Vector3(Random.Range(0, -1.3f), 1.65f, Random.Range(-.25f, -.75f));
        tmp.transform.eulerAngles = new Vector3(0, Random.Range(0, 180f));

        currentTeam.Add(tmp);
    }

    private void RemoveMan() {
        if (currentTeam.Count < 1)
            return;

        GameObject tmp = currentTeam[^1];

        currentTeam.Remove(tmp);
        Destroy(tmp);
    }

    private IEnumerator WalkBack(GameObject dude, Vector3 targetPos) {
        currentTeam.Remove(dude);
        dude.GetComponent<Animator>().SetBool("Wobbling", true);

        dude.transform.LookAt(targetPos);

        while (Vector3.Distance(dude.transform.position, targetPos) > .01f) {
            dude.transform.position = Vector3.MoveTowards(dude.transform.position, targetPos, walkspeed * Time.deltaTime);

            yield return new WaitForEndOfFrame();
        }

        currentTeam.Add(dude);
        dude.GetComponent<Animator>().SetBool("Wobbling", false);
    }

    private void GatherAroundObstacle() {
        Vector3 tmpPos;
        if (CaravanWalk.CurrentObstacle.transform.position.y < 1.7f)
            tmpPos = CaravanWalk.CurrentObstacle.transform.position + new Vector3(.4f, 0, 0);
        else
            return;

        for (int i = 0; i < currentTeam.Count; i++) {
            var dude = currentTeam[i];

            if (Vector3.Distance(dude.transform.position, tmpPos + new Vector3(0, 0, (-(currentTeam.Count / 2) + i) * .1f)) > .01f) {
                dude.transform.position = Vector3.MoveTowards(dude.transform.position, tmpPos + new Vector3(0, 0, (-(currentTeam.Count / 2) + i) * .1f), walkspeed * 4 * Time.deltaTime);
                dude.GetComponent<Animator>().SetBool("Wobbling", true);
            }
            else
                dude.GetComponent<Animator>().SetBool("Wobbling", false);
        }
    }
}