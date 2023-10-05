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