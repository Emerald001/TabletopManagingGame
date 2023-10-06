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
}

public class HorseData {
    public GameObject prefab;

    public float speed;
    public float size;
}
