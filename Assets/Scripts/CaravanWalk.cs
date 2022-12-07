using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaravanWalk : MonoBehaviour
{
    public List<GameObject> BackgroundPrefabs = new();
    public List<GameObject> ForegroundPrefabs = new();

    public GameObject Caravan;
    public GameObject Horse;
    public Transform ObjectParent;

    public List<GameObject> Horses = new();
    public List<GameObject> Caravans = new();
    public List<GameObject> Surroundings = new();

    [Range(0f, 100f)]
    public float speed;
    public float maxObstacleAmount;
    public float timerModifier;
    public float caravanWidth;
    public float spawnChance;
    public float slowdownModifier;

    public Transform Beginning;
    public Transform End;

    private float treeTimer = .3f;
    private float bushTimer = .3f;
    private float timer;
    [HideInInspector] public float currentsSpeed;

    private readonly List<GameObject> CaravanPositions = new();
    [HideInInspector] public GameObject currentObstacle;

    private void OnEnable() {
        EventManager<EncounterSO>.Subscribe(EventType.ON_ENCOUNTER_STARTED, StartEncounter);
        EventManager.Subscribe(EventType.ON_ENCOUNTER_ENDED, Restart);
    }
    private void OnDisable() {
        EventManager.Unsubscribe(EventType.ON_ENCOUNTER_ENDED, Restart);
        EventManager<EncounterSO>.Unsubscribe(EventType.ON_ENCOUNTER_STARTED, StartEncounter);
    }

    void Start() {
        currentsSpeed = speed;
        treeTimer = timer;
        bushTimer = timer;

        var tmp = new GameObject();
        tmp.transform.position = new Vector3(transform.position.x, transform.position.y + .15f, transform.position.z);
        CaravanPositions.Add(tmp);

        var tmp2 = new GameObject();
        tmp2.transform.position = new Vector3(transform.position.x, transform.position.y + .15f, transform.position.z);
        CaravanPositions.Add(tmp2);

        var totalWidth = caravanWidth * (CaravanPositions.Count + 1);

        var dis = totalWidth / (CaravanPositions.Count);

        for (int i = 0; i < CaravanPositions.Count; i++) {
            CaravanPositions[i].transform.position = new Vector3(transform.position.x + dis * (i + 1) - totalWidth / 2, transform.position.y + .16f, transform.position.z);
        }
    }

    void Update() {
        timer = Mathf.InverseLerp(100f, 0, currentsSpeed) * timerModifier;
        timer = Mathf.Max(timer, .4f);

        UpdateSurroundingsPositions();
        UpdateCaravanPositions();
    }

    private void UpdateSurroundingsPositions() {
        for (int i = Surroundings.Count - 1; i >= 0; i--) {
            var item = Surroundings[i];

            item.transform.position = Vector3.MoveTowards(item.transform.position, new Vector3(End.transform.position.x, item.transform.position.y, item.transform.position.z), currentsSpeed / 300 * Time.deltaTime);

            if (item.transform.position == new Vector3(End.transform.position.x, item.transform.position.y, item.transform.position.z)) {
                Surroundings.RemoveAt(i);

                StartCoroutine(MoveSurrounding(item.transform, transform.position.y + 2, true, false));
            }
        }

        if (currentsSpeed < 1)
            return;

        if (Surroundings.Count < maxObstacleAmount && Timer(ref treeTimer) <= 0)
            if (Random.Range(0, spawnChance * (timer * 10) * (1 - Time.deltaTime)) <= 1) {
                SpawnSurroundings(BackgroundPrefabs[Random.Range(0, BackgroundPrefabs.Count)], Random.Range(-.42f, -.3f));

                treeTimer = timer;
            }
        if (Surroundings.Count < maxObstacleAmount && Timer(ref bushTimer) <= 0)
            if (Random.Range(0, spawnChance * (timer * 10) * (1 - Time.deltaTime)) <= 1) {
                SpawnSurroundings(ForegroundPrefabs[Random.Range(0, ForegroundPrefabs.Count)], Random.Range(.3f, .42f));

                bushTimer = timer;
            }
    }

    private void UpdateCaravanPositions() {
        for (int i = 0; i < Caravans.Count; i++) {
            Vector3 tmp = new(0, 0, Random.Range(-0.001f, 0.001f));

            var currentPos = CaravanPositions[i].transform;
            var currentHorse = Horses[i].transform;
            var currentCaravan = Caravans[i].transform;

            currentPos.position += tmp;
            if (Mathf.Abs(currentPos.position.z - transform.position.z) > .1f) {
                currentPos.position = new Vector3(currentPos.position.x, currentPos.position.y, transform.position.z);
            }

            currentHorse.position = Vector3.MoveTowards(currentHorse.position, currentPos.position, currentsSpeed / 100 * Time.deltaTime);

            var newPos = new Vector3(currentCaravan.position.x, currentCaravan.position.y, currentHorse.position.z);

            currentCaravan.LookAt(new Vector3(currentHorse.position.x, transform.position.y + .03f, currentHorse.position.z));
            currentCaravan.position = Vector3.MoveTowards(currentCaravan.position, newPos, currentsSpeed / 500 * Time.deltaTime);
            currentCaravan.position = new Vector3(currentHorse.position.x + .4f, currentCaravan.position.y, currentCaravan.position.z);
        }
    }

    void SpawnSurroundings(GameObject model, float offset) {
        var item = Instantiate(model, ObjectParent);

        item.transform.position = new Vector3(Beginning.transform.position.x, transform.position.y + 2, Beginning.transform.position.z + offset);
        item.transform.eulerAngles = new Vector3(0, Random.Range(0, 360), 0);

        StartCoroutine(MoveSurrounding(item.transform, transform.position.y + item.transform.localScale.y / 2, false, false));

        Surroundings.Add(item);
    }

    public void AddCaravan() {
        var tmp = new GameObject();
        CaravanPositions.Add(tmp);

        var totalWidth = caravanWidth * (CaravanPositions.Count + 1);

        var dis = totalWidth / (CaravanPositions.Count);

        var tmpHorse = Instantiate(Horse);
        tmpHorse.transform.position = new Vector3(transform.position.x + 2.5f, transform.position.y + .16f, transform.position.z);
        Horses.Add(tmpHorse);

        var tmpCaravan = Instantiate(Caravan);
        tmpCaravan.transform.position = new Vector3(transform.position.x + 3f, transform.position.y + .03f, transform.position.z);
        Caravans.Add(tmpCaravan);

        for (int i = 0; i < CaravanPositions.Count; i++) {
            CaravanPositions[i].transform.position = new Vector3(transform.position.x + dis * (i + 1) - totalWidth / 2, transform.position.y + .16f, transform.position.z);
        }
    }

    //doesnt work yet
    public void RemoveCaravan() {
        var tmp = CaravanPositions[0];

        CaravanPositions.Remove(tmp);

        Destroy(tmp);

        var totalWidth = caravanWidth * (CaravanPositions.Count + 1);

        var dis = totalWidth / (CaravanPositions.Count + 1);

        for (int i = 0; i < CaravanPositions.Count; i++) {
            CaravanPositions[i].transform.position = new Vector3(dis * (i + 1) - totalWidth / 2, 0, 0);
        }
    }

    public void SpawnObstacle(GameObject obstacle) {
        var item = Instantiate(obstacle, ObjectParent);

        item.transform.position = new Vector3(Beginning.transform.position.x, transform.position.y + 2, Beginning.transform.position.z);
        //item.transform.eulerAngles = new Vector3(0, Random.Range(0, 360), 0);

        StartCoroutine(MoveSurrounding(item.transform, transform.position.y, false, true));

        currentObstacle = item;
        Surroundings.Add(item);
    }

    float Timer(ref float timer) {
        return timer -= Time.deltaTime;
    }

    public IEnumerator MoveSurrounding(Transform model, float targetY, bool destroy, bool shakeScreen) {
        if(shakeScreen)
            GameManager.instance.Amanager.PlayAudio("ObstacleHit");

        while (model.transform.position.y != targetY) {
            model.position = Vector3.MoveTowards(model.position, new Vector3(model.position.x, targetY, model.position.z), currentsSpeed / 4 * Time.deltaTime);

            yield return new WaitForEndOfFrame();
        }

        if (shakeScreen) 
            EventManager<float>.Invoke(EventType.DO_SCREENSHAKE, .07f);

        if (destroy)
            Destroy(model.gameObject);
    }

    public void StartEncounter(EncounterSO encounter) {
        SpawnObstacle(encounter.ObstaclePrefab);

        StartCoroutine(SlowDown(encounter));
        StartCoroutine(CallEvent(encounter));
    }

    public IEnumerator SlowDown(EncounterSO encounter) {
        while (currentsSpeed > 1) {
            currentsSpeed -= slowdownModifier * Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }

        currentsSpeed = 0;
    }

    public IEnumerator CallEvent(EncounterSO encounter) {
        yield return new WaitForSeconds(1f);

        EventManager<EncounterSO>.Invoke(EventType.ON_CARAVAN_STOPPED, encounter);
    }

    public IEnumerator SpeedUp() {
        while (currentsSpeed < speed) {
            currentsSpeed += slowdownModifier * 3 * Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }
    }

    public void Restart() {
        Surroundings.Remove(currentObstacle);
        StartCoroutine(SpeedUp());
        StartCoroutine(MoveSurrounding(currentObstacle.transform, 3, true, false));
    }
}