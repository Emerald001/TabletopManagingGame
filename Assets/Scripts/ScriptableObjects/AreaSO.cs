using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "AreaData")]
public class AreaSO : ScriptableObject
{
    public string Name;
    public Color AmbientLightColor;

    public List<GameObject> ForegroundPrefabs = new();
    public List<GameObject> BackgroundPrefabs = new();

    public string EncounterFolderName;
    public int EncounterAmount;
    public List<EncounterSO> ScriptedEncounters = new();

    private List<EncounterSO> possibleEncounters;
    public List<EncounterSO> PossibleEncounters {
        get { if (possibleEncounters == null) {
                var tmp = new List<EncounterSO>(Resources.LoadAll<EncounterSO>(EncounterFolderName));
                possibleEncounters = tmp.OrderBy(x => x.Rarity).ToList();
            }
            return possibleEncounters; }
    }
}