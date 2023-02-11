using System.Collections;
using System.Collections.Generic;
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
    }

    public void SetGoal(int currentVal, int maxVal) {
        Goal.text = currentVal.ToString() + "/" + maxVal.ToString();
    }
}