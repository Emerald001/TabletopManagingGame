using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : Singleton<InventoryManager> {
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

[SerializeField]
[CreateAssetMenu(menuName = "Inventory/HorseData")]
public class HorseData : BaseData {
    public float speed;
}

[SerializeField]
[CreateAssetMenu(menuName = "Inventory/ManData")]
public class ManData : BaseData {
    public float speed;
}

[SerializeField]
[CreateAssetMenu(menuName = "Inventory/CaravanData")]
public class CaravanData : BaseData {
    public int teamSpace;
}

public class PlayerData {

}

public abstract class BaseData : ScriptableObject {
    public GameObject prefab;

    public new string name;
    public string description;
}