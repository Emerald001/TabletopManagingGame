using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public QuestHolder QuestHolder;
    private QuestSO currentQuest;

    public void OnEnable() {
        EventManager<QuestSO>.Subscribe(EventType.SET_QUEST, DisplayQuest);
    }
    public void OnDisable() {
        EventManager<QuestSO>.Unsubscribe(EventType.SET_QUEST, DisplayQuest);
    }

    public void DisplayQuest(QuestSO quest) {
        currentQuest = quest;

        QuestHolder.SetQuestInfo(quest);
        QuestHolder.SetGoal(0, currentQuest.goal);

        EventManager<AreaSO>.Invoke(EventType.SET_AREA, currentQuest.area);
    }

    public void GetQuest() {
        EventManager<AreaSO>.Invoke(EventType.SET_AREA, currentQuest.area);
    }

    public void HandInQuest() {

    }
}