using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBehaviorManager : MonoBehaviour {
    public const float CARAVAN_WIDTH = 1;
    public const float SLOWDOWN_MODIFIER = 10;

    [Header("References")]
    [SerializeField] private GameObject caravanPrefab;
    [SerializeField] private GameObject horsePrefab;
    [SerializeField] private Transform objectParent;

    [SerializeField] private Transform beginning;
    [SerializeField] private Transform end;
    [SerializeField] private MapUnrollSequence mapUnrollSequence;
    [SerializeField] private HumanBehavior humanBehavior;

    public GameObject CurrentObstacle { get; private set; }
    public float CurrentSpeed { get; private set; }

    private readonly List<GameObject> caravanPositions = new();
    private readonly List<GameObject> horses = new();
    private readonly List<GameObject> caravans = new();
    private readonly List<GameObject> surroundings = new();

    private List<GameObject> backgroundPrefabs = new();
    private List<GameObject> foregroundPrefabs = new();

    private AreaSO currentArea;

    private float timer;
    private float currentTimer;

    private bool encounterActive;
    private bool mapActive;

    private void OnEnable() {
        EventManager<CaravanEventType, EncounterSO>.Subscribe(CaravanEventType.ON_ENCOUNTER_STARTED, StartEncounter);
        EventManager<CaravanEventType>.Subscribe(CaravanEventType.DESTROY_CARAVAN, RemoveCaravan);
        EventManager<CaravanEventType>.Subscribe(CaravanEventType.ON_ENCOUNTER_ENDED, Restart);
        EventManager<CaravanEventType, AreaSO>.Subscribe(CaravanEventType.SET_AREA, SetArea);
    }
    private void OnDisable() {
        EventManager<CaravanEventType, EncounterSO>.Unsubscribe(CaravanEventType.ON_ENCOUNTER_STARTED, StartEncounter);
        EventManager<CaravanEventType>.Unsubscribe(CaravanEventType.DESTROY_CARAVAN, RemoveCaravan);
        EventManager<CaravanEventType>.Unsubscribe(CaravanEventType.ON_ENCOUNTER_ENDED, Restart);
        EventManager<CaravanEventType, AreaSO>.Unsubscribe(CaravanEventType.SET_AREA, SetArea);
    }

    private void Update() {
        if (!mapActive)
            return;

        timer = Mathf.InverseLerp(100f, 0, CurrentSpeed) * currentArea.timerModifier;
        timer = Mathf.Max(timer, .4f);

        UpdateSurroundingsPositions();
        UpdateCaravanPositions();
    }

    private void SetArea(AreaSO area) {
        currentArea = area;

        backgroundPrefabs = area.BackgroundPrefabs;
        foregroundPrefabs = area.ForegroundPrefabs;

        StartCoroutine(SetupMap());
    }

    private void UpdateSurroundingsPositions() {
        for (int i = surroundings.Count - 1; i >= 0; i--) {
            GameObject item = surroundings[i];
            item.transform.position = Vector3.MoveTowards(item.transform.position, new Vector3(end.transform.position.x, item.transform.position.y, item.transform.position.z), CurrentSpeed / 300 * Time.deltaTime);

            if (item.transform.position == new Vector3(end.transform.position.x, item.transform.position.y, item.transform.position.z)) {
                surroundings.RemoveAt(i);
                Destroy(item);
            }
        }

        if (CurrentSpeed < 1 || encounterActive)
            return;

        if (surroundings.Count < currentArea.maxObstacleAmount && Timer(ref currentTimer) <= 0) {
            if (Random.Range(0, 2) == 1)
                SpawnSurroundings(backgroundPrefabs[Random.Range(0, backgroundPrefabs.Count)], Random.Range(-.42f, -.3f));
            else
                SpawnSurroundings(foregroundPrefabs[Random.Range(0, foregroundPrefabs.Count)], Random.Range(.3f, .42f));

            currentTimer = timer + Random.Range(0f, 1f);
        }
    }

    private void UpdateCaravanPositions() {
        for (int i = 0; i < caravans.Count; i++) {
            Vector3 offset = new(0, 0, Random.Range(-0.001f, 0.001f));

            Transform currentPos = caravanPositions[i].transform;
            Transform currentHorse = horses[i].transform;
            Transform currentCaravan = caravans[i].transform;

            currentPos.position += offset;
            if (Mathf.Abs(currentPos.position.z - transform.position.z) > .1f)
                currentPos.position = new Vector3(currentPos.position.x, currentPos.position.y, transform.position.z);

            currentHorse.position = Vector3.MoveTowards(currentHorse.position, currentPos.position, CurrentSpeed / 100 * Time.deltaTime);
            Vector3 newPos = new(currentCaravan.position.x, currentCaravan.position.y, currentHorse.position.z);

            currentCaravan.LookAt(new Vector3(currentHorse.position.x, end.position.y + .03f, currentHorse.position.z));
            currentCaravan.position = Vector3.MoveTowards(currentCaravan.position, newPos, CurrentSpeed / 500 * Time.deltaTime);
            currentCaravan.position = new Vector3(currentHorse.position.x + .4f, currentCaravan.position.y, currentCaravan.position.z);
        }
    }

    private void SpawnSurroundings(GameObject model, float offset) {
        GameObject item = Instantiate(model, objectParent);

        item.transform.position = new Vector3(beginning.position.x, beginning.position.y + 2, beginning.position.z + offset);
        item.transform.eulerAngles = new Vector3(0, Random.Range(0, 360), 0);

        StartCoroutine(MoveSurrounding(item.transform, beginning.position.y, false, false));

        surroundings.Add(item);
    }

    private void SpawnCaravan(HorseData horse, CaravanData caravan) {
        GameObject go = new();
        caravanPositions.Add(go);

        float totalWidth = CARAVAN_WIDTH * (caravanPositions.Count + 1);
        float dis = totalWidth / (caravanPositions.Count + 1);

        GameObject tmpHorse = Instantiate(horse.prefab);
        tmpHorse.transform.position = new Vector3(end.position.x, end.position.y + .16f, end.position.z);
        horses.Add(tmpHorse);

        GameObject tmpCaravan = Instantiate(caravan.prefab);
        tmpCaravan.transform.position = new Vector3(end.position.x, end.position.y + .03f, end.position.z);
        caravans.Add(tmpCaravan);

        for (int i = 0; i < caravanPositions.Count; i++)
            caravanPositions[i].transform.position = new Vector3(transform.position.x + dis * (i + 1) - totalWidth / 2, end.position.y + .16f, transform.position.z);
    }

    private void RemoveCaravan() {
        GameObject go = caravanPositions[^1];

        caravanPositions.Remove(go);
        Destroy(go);

        GameObject Horse = horses[^1];
        GameObject caravan = caravans[^1];

        horses.Remove(Horse);
        caravans.Remove(caravan);

        surroundings.Add(Horse);
        surroundings.Add(caravan);

        float totalWidth = CARAVAN_WIDTH * (caravanPositions.Count + 1);
        float dis = totalWidth / (caravanPositions.Count + 1);

        for (int i = 0; i < caravanPositions.Count; i++)
            caravanPositions[i].transform.position = new Vector3(end.position.x + dis * (i + 1) - totalWidth / 2, end.position.y + .16f, end.position.z);
    }

    private void StartEncounter(EncounterSO encounter) {
        encounterActive = true;

        SpawnObstacle(encounter.ObstaclePrefab);

        StartCoroutine(SlowDown());
        StartCoroutine(CallEvent(encounter));
    }

    private void SpawnObstacle(GameObject obstacle) {
        GameObject item = Instantiate(obstacle, objectParent);
        item.transform.position = new Vector3(beginning.transform.position.x, end.position.y + 2, beginning.transform.position.z);

        StartCoroutine(MoveSurrounding(item.transform, end.position.y, false, true));

        CurrentObstacle = item;
        surroundings.Add(item);
    }

    private void Restart() {
        surroundings.Remove(CurrentObstacle);

        StopAllCoroutines();

        StartCoroutine(SpeedUp());
        StartCoroutine(MoveSurrounding(CurrentObstacle.transform, 3, true, false));

        encounterActive = false;
    }

    private float Timer(ref float timer) {
        return timer -= Time.deltaTime;
    }

    private IEnumerator SetupMap() {
        mapUnrollSequence.DoUnrollRoll(currentArea.MapRollPrefab);

        yield return new WaitForSeconds(2f);

        InventoryData inventory = InventoryManager.Instance.RunInventory;

        for (int i = 0; i < inventory.HorseDatas.Count; i++) {
            HorseData item = inventory.HorseDatas[i];
            CaravanData caravan = inventory.CaravanDatas[i];

            SpawnCaravan(item, caravan);
        }

        for (int i = 0; i < inventory.ManDatas.Count; i++) {
            ManData man = inventory.ManDatas[i];
            humanBehavior.AddMan(man.prefab);
        }

        CurrentSpeed = currentArea.movementSpeed;
        mapActive = true;
    }

    private IEnumerator MoveSurrounding(Transform model, float targetY, bool destroy, bool shakeScreen) {
        if (shakeScreen)
            GameManager.Instance.AudioManager.PlayAudio("ObstacleHit");

        while (model.transform.position.y != targetY) {
            model.position = Vector3.MoveTowards(model.position, new Vector3(model.position.x, targetY, model.position.z), CurrentSpeed / 4 * Time.deltaTime);

            yield return new WaitForEndOfFrame();
        }

        if (shakeScreen)
            EventManager<CaravanEventType, float>.Invoke(CaravanEventType.DO_SCREENSHAKE, .07f);

        if (destroy)
            Destroy(model.gameObject);
    }

    private IEnumerator SlowDown() {
        while (CurrentSpeed > 1) {
            CurrentSpeed -= SLOWDOWN_MODIFIER * Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }

        CurrentSpeed = 0;
    }

    private IEnumerator CallEvent(EncounterSO encounter) {
        yield return new WaitForSeconds(1f);

        EventManager<CaravanEventType, EncounterSO>.Invoke(CaravanEventType.ON_CARAVAN_STOPPED, encounter);
    }

    private IEnumerator SpeedUp() {
        while (CurrentSpeed < currentArea.movementSpeed) {
            CurrentSpeed += SLOWDOWN_MODIFIER * 3 * Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }
    }
}