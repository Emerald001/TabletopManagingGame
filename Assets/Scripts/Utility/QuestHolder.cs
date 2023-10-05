using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestHolder : MonoBehaviour
{
    public Image Icon;

    public TextMeshProUGUI Name;
    public TextMeshProUGUI Discription;
    public TextMeshProUGUI GoalReason;
    public TextMeshProUGUI Goal;
    public TextMeshProUGUI ExtraRewards;

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