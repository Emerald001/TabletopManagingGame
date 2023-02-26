using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AreaData")]
public class AreaSO : ScriptableObject
{
    public string Name;
    public Color AmbientLightColor;

    public List<GameObject> ForegroundPrefabs = new();
    public List<GameObject> BackgroundPrefabs = new();

    public string EncounterFolderName;
    public List<EncounterSO> ScriptedEncounters = new();

    private List<EncounterSO> possibleEncounter;
    public List<EncounterSO> PossibleEncounters {
        get { if (possibleEncounter == null)
                possibleEncounter = new(Resources.LoadAll<EncounterSO>(EncounterFolderName));
            return possibleEncounter; }
    }
}