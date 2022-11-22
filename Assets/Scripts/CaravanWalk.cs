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

    public Transform Beginning;
    public Transform End;

    private float treeTimer = .3f;
    private float bushTimer = .3f;
    private float timer;

    private List<GameObject> CaravanPositions = new();

    void Start() {
        treeTimer = timer;
        bushTimer = timer;

        var tmp = new GameObject();
        tmp.transform.position = new Vector3(transform.position.x, transform.position.y + .15f, transform.position.z);
        CaravanPositions.Add(tmp);
    }

    void Update() {
        timer = Mathf.InverseLerp(100f, 0, speed) * timerModifier;
        timer = Mathf.Max(timer, .4f);

        if (speed < 1)
            return;

        for (int i = Surroundings.Count - 1; i >= 0; i--) {
            var item = Surroundings[i];

            item.transform.position = Vector3.MoveTowards(item.transform.position, new Vector3(End.transform.position.x, item.transform.position.y, item.transform.position.z), speed / 300 * Time.deltaTime);

            if (item.transform.position == new Vector3(End.transform.position.x, item.transform.position.y, item.transform.position.z)) {
                Surroundings.RemoveAt(i);
                Destroy(item);
            }
        }

        for (int i = 0; i < Caravans.Count; i++) {
            Vector3 tmp = new(0, 0, Random.Range(-0.001f, 0.001f));

            var currentPos = CaravanPositions[i].transform;
            var currentHorse = Horses[i].transform;
            var currentCaravan = Caravans[i].transform;

            currentPos.position += tmp;
            if (Mathf.Abs(currentPos.position.z - transform.position.z) > .1f) {
                currentPos.position = new Vector3(currentPos.position.x, currentPos.position.y, transform.position.z);
            }

            currentHorse.position = Vector3.MoveTowards(currentHorse.position, currentPos.position, speed / 100 * Time.deltaTime);

            var newPos = new Vector3(currentCaravan.position.x, currentCaravan.position.y, currentHorse.position.z);

            currentCaravan.LookAt(currentHorse.position);
            currentCaravan.position = Vector3.MoveTowards(currentCaravan.position, newPos, speed / 500 * Time.deltaTime);
            currentCaravan.position = new Vector3(currentHorse.position.x + .4f, currentCaravan.position.y, currentCaravan.position.z);
        }

        SpawnNewTree();
        SpawnNewBush();

        if (Input.GetKeyDown(KeyCode.Space)) {
            AddCaravan();
        }
        if (Input.GetKeyDown(KeyCode.Backspace)) {
            RemoveCaravan();
        }
    }

    void SpawnNewTree() {
        if (Surroundings.Count < maxObstacleAmount && Timer(ref treeTimer) <= 0)
            if (Random.Range(0, spawnChance * (timer * 10)) <= 1) {
                var item = Instantiate(BackgroundPrefabs[Random.Range(0, BackgroundPrefabs.Count)], ObjectParent);

                item.transform.position = new Vector3(Beginning.transform.position.x, transform.position.y + item.transform.localScale.y / 2, Beginning.transform.position.z + Random.Range(-.42f, -.3f));
                item.transform.eulerAngles = new Vector3(0, Random.Range(0, 360), 0);

                Surroundings.Add(item);

                treeTimer = timer;
            }
    }

    void SpawnNewBush() {
        if (Surroundings.Count < maxObstacleAmount && Timer(ref bushTimer) <= 0)
            if (Random.Range(0, spawnChance * (timer * 10)) <= 1) {
                var item = Instantiate(ForegroundPrefabs[Random.Range(0, ForegroundPrefabs.Count)], ObjectParent);

                item.transform.position = new Vector3(Beginning.transform.position.x, transform.position.y + item.transform.localScale.y / 2, Beginning.transform.position.z + Random.Range(.3f, .42f));
                item.transform.eulerAngles = new Vector3(0, Random.Range(0, 360), 0);

                Surroundings.Add(item);

                bushTimer = timer;
            }
    }

    public void AddCaravan() {
        var tmp = new GameObject();
        CaravanPositions.Add(tmp);

        var totalWidth = caravanWidth * (CaravanPositions.Count + 1);

        var dis = totalWidth / (CaravanPositions.Count + 1);

        var tmpHorse = Instantiate(Horse);
        tmpHorse.transform.position = new Vector3(transform.position.x + 2.5f, transform.position.y + .16f, transform.position.z);
        Horses.Add(tmpHorse);

        var tmpCaravan = Instantiate(Caravan);
        tmpCaravan.transform.position = new Vector3(transform.position.x + 3f, transform.position.y + .16f, transform.position.z);
        Caravans.Add(tmpCaravan);

        for (int i = 0; i < CaravanPositions.Count; i++) {
            CaravanPositions[i].transform.position = new Vector3(transform.position.x + dis * (i + 1) - totalWidth / 2, transform.position.y + .16f, transform.position.z);
        }
    }

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

    }

    float Timer(ref float timer) {
        return timer -= Time.deltaTime;
    }
}