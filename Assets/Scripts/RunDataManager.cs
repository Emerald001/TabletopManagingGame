using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunDataManager : MonoBehaviour
{
    [Header("Run Data")]
    public List<GameObject> Team = new();
    public List<GameObject> Horses = new();
    public List<GameObject> Carts = new();

    public List<QuestSO> Quests = new();
    public List<GameObject> Items = new();

    [Header("Choises Made")]
    [Range(0, 100)]
    public int NatureAlignment;
    [Range(0, 100)]
    public int HumanAlignment;
    [Range(0, 100)]
    public int DarkAlignment;

    public bool KilledBandits;

    [Header("Past Encounters")]
    public List<EncounterSO> encounters;

    [Header("Misc")]
    public ResourcesData currentResources;
    public Areas currentArea;


}

[System.Serializable]
public struct ResourcesData {
    public int Materials;
    public int Food;
    public int Gold;

    public float Water;
}

public enum Areas{
    Village,
    Forest,
    Swamp,
    Canyon,
    Caves,
    Desert
}