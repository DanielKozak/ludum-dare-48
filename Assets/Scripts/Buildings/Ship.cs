using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Ship : Building
{
    (int x, int y) position = (23, 39);

    [NonSerialized] public int bananaPrice = 1;
    [NonSerialized] public int fuelPrice = 3;
    [NonSerialized] public int fuelBuyPrice = 4;
    [NonSerialized] public int monkePrice = 10;

    Camera camera;
    private void Start()
    {
        camera = Camera.main;
        OnRightClick = new IActions.Action_Generic(() =>
        {
            ShowContextMenu();
        });


    }
    public void Sell(int amount, int type)
    {
        switch (type)
        {
            case 0:
                GameManager.Instance.SetBananaBalance(amount * bananaPrice);
                break;

            case 1:
                GameManager.Instance.SetBananaBalance(amount * fuelPrice);

                break;
        }
        //EFFECT

    }
    public void BuyOil(int amount)
    {
        if (GameManager.Instance.SetBananaBalance(-1 * amount * fuelBuyPrice, true))
        {
            GameManager.Instance.SetBananaBalance(-1 * amount * fuelBuyPrice);
        }

    }



    void ShowContextMenu()
    {

        if (InputManager.GetSelectedBuilding() == null) return;

        if (InputManager.GetSelectedBuilding().isTruck)
        {

            var truck = ((Truck)InputManager.GetSelectedBuilding());

            var interactPos = SelectClosestConcrete();
            if (interactPos == (-1, -1))
            {
                UIManager.Instance.ShowContextMenu("No path", () =>
                           {
                               return;
                           });
                return;
            }

            switch (truck.CargoType)
            {
                case -1:
                    if (GameManager.Instance.SetBananaBalance(-fuelBuyPrice * truck.MaxLoad, true)) UIManager.Instance.ShowContextMenu("Buy truckload of oil", () =>
           {
               truck.AddOrder(new TruckActions.Load(truck, this, interactPos, 1));
           });
                    break;
                case 0:
                    UIManager.Instance.ShowContextMenu("Sell Bananas", () =>
           {
               truck.AddOrder(new TruckActions.Unload(truck, this, interactPos, -1));
           });
                    break;
                case 1:
                    UIManager.Instance.ShowContextMenu("Sell Fuel", () =>
           {
               truck.AddOrder(new TruckActions.Unload(truck, this, interactPos, -1));
           });
                    break;
            }
        }

    }

    (int, int) SelectClosestConcrete()
    {
        foreach (var item in MapUtils.GetNeighbours(position.x, position.y, MapController.Instance.MapW, MapController.Instance.MapH))
        {
            if (MapController.Instance.TerrainMap.GetValue(item) == 2)
            {
                return item;
            }
        }
        return (-1, -1);
    }

    public override void ShowInfo()
    {
        if (InputManager.GetSelectedBuilding() != this) return;

        List<UIManager.InfoItemStruct> items = new List<UIManager.InfoItemStruct>();


        items.Add(new UIManager.InfoItemStruct("Banana Export Price:", $"{bananaPrice}", false));
        items.Add(new UIManager.InfoItemStruct("Fuel Export Price:", $"{fuelPrice}", false));
        items.Add(new UIManager.InfoItemStruct("Fuel Import Price:", $"{fuelBuyPrice}", false));
        // [NonSerialized] public int bananaPrice = 2;
        // [NonSerialized] public int fuelPrice = 2;
        // [NonSerialized] public int fuelBuyPrice = 3;
        // [NonSerialized] public int monkePrice = 10;


        UIManager.Instance.ShowInfoPanel(this, name, items);

        // public int BananaCount = 0;
        // UIManager.Instance.ShowInfoPanel(name, items);
    }
}