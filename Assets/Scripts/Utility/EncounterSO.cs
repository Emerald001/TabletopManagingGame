using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EncounterCatagory { 
    RESOURCE_GATHER,
    RESOURCE_TAKE,

}

public enum Difficulty { 
    GOOD,
    NEUTRAL,
    BAD
}

[CreateAssetMenu(menuName = "EncounterData")]
public class EncounterSO : ScriptableObject
{
    public new string name;
    public Sprite Icon;
    public EncounterCatagory catagory;
    public Difficulty difficulty;

    public Color lightColor;

    public GameObject ObstaclePrefab;
    public GameObject SurroundingPrefab;
    public GameObject BackgroundPrefab;

    public List<Option> options = new();
}

[System.Serializable]
public struct Option {
    public string Name;
    public string Description;

    public EventType EventToCall;

    [Header("Uses")]
    public int WoodUse;
    public int MeatUse;
    public int GoldUse;
    public float WaterUse;

    [Header("Gives")]
    public int Woodgive;
    public int Meatgive;
    public int Goldgive;
    public float WaterGive;
}