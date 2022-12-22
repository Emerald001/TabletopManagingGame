using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData {
    public bool isNewGame;

    public List<QuestSO> currentQuests;

    public int materials;
    public int gold;
    public int meat;
    public float water;
}