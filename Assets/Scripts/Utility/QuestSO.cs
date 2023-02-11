using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "QuestData")]
public class QuestSO : ScriptableObject
{
    public Sprite Icon;
    public new string name;
    public string discription;

    public Difficulty difficulty;
}