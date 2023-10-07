using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "AreaData")]
public class AreaSO : ScriptableObject {
    [Header("References")]
    public string Name;
    public string EncounterFolderName;

    [Space(10)]
    public Color AmbientLightColor;
    public GameObject MapPrefab;
    public List<GameObject> ForegroundPrefabs = new();
    public List<GameObject> BackgroundPrefabs = new();


    private List<EncounterSO> possibleEncounters = null;
    public List<EncounterSO> PossibleEncounters {
        get {
            if (possibleEncounters == null) {
                List<EncounterSO> list = new(Resources.LoadAll<EncounterSO>($"Encounters/{EncounterFolderName}/"));
                possibleEncounters = list.OrderBy(x => x.Rarity).ToList();
            }
            return possibleEncounters; 
        }
        private set { }
    }
}