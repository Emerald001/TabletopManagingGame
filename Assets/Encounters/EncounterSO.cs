using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "EncounterData")]
public class EncounterSO : ScriptableObject
{
    public new string name;
    public GameObject ObstaclePrefab;

    public List<Option> options = new();
}

[System.Serializable]
public struct Option {
    public string Description;

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