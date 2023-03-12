using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RunDataManager : MonoBehaviour
{
    public RunData CurrentRunData;

    public void LoadData() {

    }
}

[System.Serializable]
public struct ResourcesData {
    public int Materials;
    public int Food;
    public int Gold;

    public float Water;
}

[Serializable]
public class RunData {
    [Header("Run Data")]
    public List<GameObject> Team = new();
    public List<GameObject> Horses = new();
    public List<GameObject> Carts = new();

    public List<GameObject> Items = new();
    public QuestSO Quest;

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
    public AreaSO currentArea;
}