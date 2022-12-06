using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public ResourceStack WoodStack;
    public ResourceStack MeatStack;
    public ResourceStack GoldStack;

    public LiquidStack WaterStack;

    public bool HasResources(int wood, int meat, int gold, float water) {
        if (WoodStack.StackAmount < wood || MeatStack.StackAmount < meat || GoldStack.StackAmount < gold)
            return false;

        return true;
    }

    public void AddResources(Option ID) {
        StartCoroutine(WoodStack.AddItem(ID.Woodgive));
        StartCoroutine(MeatStack.AddItem(ID.Meatgive));
        StartCoroutine(GoldStack.AddItem(ID.Goldgive));

        WaterStack.AddLiquid(ID.WaterGive);
    }

    public void ShowResources(Option ID) {
        if(HasResources(ID.WoodUse, ID.MeatUse, ID.GoldUse, 0))

        WoodStack.ShowItems(ID.WoodUse);
        MeatStack.ShowItems(ID.MeatUse);
        GoldStack.ShowItems(ID.GoldUse);
    }

    public void RemoveResources(Option ID) {
        StartCoroutine(WoodStack.RemoveItem(ID.WoodUse));
        StartCoroutine(MeatStack.RemoveItem(ID.MeatUse));
        StartCoroutine(GoldStack.RemoveItem(ID.GoldUse));

        WaterStack.RemoveLiquid(ID.WaterUse);
    }
}