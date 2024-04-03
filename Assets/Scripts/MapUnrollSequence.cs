using UnityEngine;

public class MapUnrollSequence : MonoBehaviour {
    [SerializeField] private Transform rollStartpoint;
    [SerializeField] private Transform rollEndPoint;

    [SerializeField] private GameObject mask;
    [SerializeField] private Transform maskStartPoint;
    [SerializeField] private Transform maskEndPoint;

    private readonly ActionQueue ActionQueue = new(null);

    void Update() {
        ActionQueue.OnUpdate();
    }

    public void DoUnrollRoll(GameObject rollPrefab) {
        GameObject roll = Instantiate(rollPrefab, rollStartpoint.position, Quaternion.identity);
        roll.transform.rotation = Quaternion.Euler(0f, 180f, 0f);

        ActionQueue.Enqueue(new DoMethodAction(() => GameManager.Instance.AudioManager.PlayAudio("PaperRoll")));
        ActionQueue.Enqueue(new ActionStack(
            new MoveObjectAction(roll, 5, rollEndPoint),
            new ResizeAction(roll.transform, 1, new Vector3(.2f, .2f, 1)),
            new MoveObjectAction(mask, 4.9f, maskEndPoint.position)));
        ActionQueue.Enqueue(new DestoyObjectAction(roll));
    }

    public void DoRoll(GameObject rollPrefab) {
        GameObject roll = Instantiate(rollPrefab, rollEndPoint.position, Quaternion.identity);
        roll.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        roll.transform.localScale = new Vector3(.2f, .2f, 1);

        ActionQueue.Enqueue(new DoMethodAction(() => GameManager.Instance.AudioManager.PlayAudio("PaperRoll")));
        ActionQueue.Enqueue(new ActionStack(
            new MoveObjectAction(roll, 5, rollStartpoint),
            new ResizeAction(roll.transform, 1, new Vector3(1, 1, 1)),
            new MoveObjectAction(mask, 5, maskStartPoint)));
        ActionQueue.Enqueue(new DestoyObjectAction(roll));
    }
}
