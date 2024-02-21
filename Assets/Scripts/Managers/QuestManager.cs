using UnityEngine;

public class QuestManager : MonoBehaviour
{
    [SerializeField] private QuestHolder QuestHolder;

    [SerializeField] private Transform standardPos;
    [SerializeField] private Transform raisedPos;

    private QuestSO currentQuest;
    private readonly ActionQueue actionQueue = new();

    private void OnEnable() {
        EventManager<CaravanEventType, QuestSO>.Subscribe(CaravanEventType.SET_QUEST, DisplayQuest);
    }
    private void OnDisable() {
        EventManager<CaravanEventType, QuestSO>.Unsubscribe(CaravanEventType.SET_QUEST, DisplayQuest);
    }

    private void Update() {
        actionQueue.OnUpdate();
    }

    private void DisplayQuest(QuestSO quest) {
        currentQuest = quest;

        QuestHolder.SetQuestInfo(quest);
        QuestHolder.SetGoal(0, currentQuest.goal);

        actionQueue.Enqueue(new WaitAction(1f));
        actionQueue.Enqueue(new MoveObjectAction(QuestHolder.gameObject, 30, standardPos));
        actionQueue.Enqueue(new DoMethodAction(() => EventManager<CaravanEventType, float>.Invoke(CaravanEventType.DO_SCREENSHAKE, .7f)));
        actionQueue.Enqueue(new DoMethodAction(() => EventManager<CaravanEventType, AreaSO>.Invoke(CaravanEventType.SET_AREA, currentQuest.area)));
    }

    private void HandInQuest() {

    }
}