using UnityEngine;
using System.Collections.Generic;
public class Garage : Building
{
    float cooldown = 0;
    private void Start()
    {
        isGarage = true;
    }
    public override void ShowInfo()
    {
        if (InputManager.GetSelectedBuilding() != this) return;

        List<UIManager.InfoItemStruct> items = new List<UIManager.InfoItemStruct>();


        items.Add(new UIManager.InfoItemStruct("TruckPrice :", $"{UIManager.Instance.price_truck}", false));
        items.Add(new UIManager.InfoItemStruct("Truck build time:", $"{cooldown}", false));
        // [NonSerialized] public int bananaPrice = 2;
        // [NonSerialized] public int fuelPrice = 2;
        // [NonSerialized] public int fuelBuyPrice = 3;
        // [NonSerialized] public int monkePrice = 10;


        UIManager.Instance.ShowInfoPanel(this, name, items);
    }
}