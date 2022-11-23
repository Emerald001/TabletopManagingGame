using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public ResourceStack WoodStack;
    public ResourceStack MeatStack;
    public ResourceStack GoldStack;

    public LiquidStack WaterStack;

    void Start() {
        
    }

    void Update() {
        
    }

    public bool HasResources(int wood, int meat, int gold, float water) {
        if (WoodStack.StackAmount < wood || MeatStack.StackAmount < meat || GoldStack.StackAmount < gold)
            return false;

        return true;
    }

    public void AddResources(int wood, int meat, int gold, float water) {
        WoodStack.AddItem(wood);
        MeatStack.AddItem(meat);
        GoldStack.AddItem(gold);

        WaterStack.AddLiquid(water);
    }

    public void ShowResources(int wood, int meat, int gold, float water) {
        if(HasResources(wood, meat, gold, water))

        WoodStack.ShowItems(wood);
        MeatStack.ShowItems(meat);
        GoldStack.ShowItems(gold);
    }

    public void RemoveResources(int wood, int meat, int gold, float water) {
        WoodStack.RemoveItem(wood);
        MeatStack.RemoveItem(meat);
        GoldStack.RemoveItem(gold);

        WaterStack.AddLiquid(water);
    }
}