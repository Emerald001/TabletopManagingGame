using UnityEngine;

public class ResourceManager : MonoBehaviour {
    [SerializeField] private ResourceStack WoodStack;
    [SerializeField] private ResourceStack MeatStack;
    [SerializeField] private ResourceStack GoldStack;

    [SerializeField] private LiquidStack WaterStack;

    public void AddResources(Option ID) {
        if (ID.Woodgive < 0)
            StartCoroutine(WoodStack.RemoveItem(Mathf.Abs(ID.Woodgive)));
        else
            StartCoroutine(WoodStack.AddItem(ID.Woodgive));

        if (ID.Meatgive < 0)
            StartCoroutine(MeatStack.RemoveItem(Mathf.Abs(ID.Meatgive)));
        else
            StartCoroutine(MeatStack.AddItem(ID.Meatgive));

        if (ID.Goldgive < 0)
            StartCoroutine(GoldStack.RemoveItem(Mathf.Abs(ID.Goldgive)));
        else
            StartCoroutine(GoldStack.AddItem(ID.Goldgive));

        if (ID.WaterGive < 0)
            WaterStack.RemoveLiquid(Mathf.Abs(ID.WaterGive));
        else
            WaterStack.AddLiquid(ID.WaterGive);
    }

    public void ShowResources(Option ID) {
        if (!HasResources(ID.WoodUse, ID.MeatUse, ID.GoldUse, 0))
            return;

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

    public bool HasResources(int wood, int meat, int gold, float water) {
        if (WoodStack.HasResource(wood) || 
            MeatStack.HasResource(meat) || 
            GoldStack.HasResource(gold) || 
            WaterStack.HasResource(water))
            return false;

        return true;
    }
}