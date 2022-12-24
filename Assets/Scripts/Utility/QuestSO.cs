using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "QuestData")]
public class QuestSO : ScriptableObject
{
    public new string name;
    public Sprite Icon;

    public Difficulty difficulty;
    public string discription;
}