using UnityEngine;

public class QuestManager : MonoBehaviour
{
    [SerializeField] private QuestHolder QuestHolder;

    private QuestSO currentQuest;

    private void OnEnable() {
        EventManager<CaravanEventType, QuestSO>.Subscribe(CaravanEventType.SET_QUEST, DisplayQuest);
    }
    private void OnDisable() {
        EventManager<CaravanEventType, QuestSO>.Unsubscribe(CaravanEventType.SET_QUEST, DisplayQuest);
    }

    private void DisplayQuest(QuestSO quest) {
        currentQuest = quest;

        QuestHolder.SetQuestInfo(quest);
        QuestHolder.SetGoal(0, currentQuest.goal);

        EventManager<CaravanEventType, AreaSO>.Invoke(CaravanEventType.SET_AREA, currentQuest.area);
    }

    private void GetQuest() {
        EventManager<CaravanEventType, AreaSO>.Invoke(CaravanEventType.SET_AREA, currentQuest.area);
    }

    private void HandInQuest() {

    }
}