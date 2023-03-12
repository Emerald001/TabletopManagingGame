using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Difficulty { 
    EASY,
    NORMAL,
    HARD
}

[CreateAssetMenu(menuName = "QuestData")]
public class QuestSO : ScriptableObject
{
    public Sprite Icon;
    public new string name;
    public string discription;
    public string extraDisc;
    public List<string> extraRewards;

    public int goal;

    public Difficulty difficulty;

    public AreaSO area;
}