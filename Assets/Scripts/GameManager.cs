using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameManager() {
        instance = this;
    }

    public ResourceManager Rmanager;
    public CaravanWalk Mmanager;
    public DisplayEncounter EncounterDisplay;

    void Start() {
        
    }

    void Update() {
        
    }
}