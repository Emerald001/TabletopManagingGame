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
}