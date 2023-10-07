using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestHolder : MonoBehaviour
{
    [SerializeField] private Image Icon;

    [SerializeField] private TextMeshProUGUI Name;
    [SerializeField] private TextMeshProUGUI Discription;
    [SerializeField] private TextMeshProUGUI GoalReason;
    [SerializeField] private TextMeshProUGUI Goal;
    [SerializeField] private TextMeshProUGUI ExtraRewards;

    public void SetQuestInfo(QuestSO quest) {
        Icon.sprite = quest.Icon;
        Name.text = quest.name;
        Discription.text = quest.discription;
        GoalReason.text = quest.extraDisc;

        string extraRewards = "";
        for (int i = 0; i < quest.extraRewards.Count; i++)
            extraRewards += $"- {quest.extraRewards[i]}\r";

        ExtraRewards.text = extraRewards;
    }

    public void SetGoal(int currentVal, int maxVal) {
        Goal.text = $"Goal: {currentVal} / {maxVal}";
    }
}