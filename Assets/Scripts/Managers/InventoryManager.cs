using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public InventoryData GetCurrentlySelectedInventory() {
        return new InventoryData();
    }
}

[Serializable]
public class InventoryData {
    public List<HorseData> HorseDatas;
    public List<ManData> ManDatas;
    public List<CaravanData> CaravanDatas;
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