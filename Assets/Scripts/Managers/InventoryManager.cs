using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : Singleton<InventoryManager>
{
    public InventoryData RunInventory { get; private set; } = new();

    private void Awake() {
        Init(this);
    }

    public InventoryData GetCurrentlySelectedInventory() {
        return new InventoryData();
    }
}

[Serializable]
public class InventoryData {
    public List<HorseData> HorseDatas = new();
    public List<ManData> ManDatas = new();
    public List<CaravanData> CaravanDatas = new();
}

public class HorseData {
    public GameObject prefab;

    public float speed;
    public float size;

    public string name;
    public string description;
}

public class ManData {
    public GameObject prefab;

    public float speed;
    public float size;

    public string name;
    public string description;
}

public class CaravanData {
    public GameObject prefab;

    public float speed;
    public float size;

    public string name;
    public string description;
}

public class PlayerData {

}