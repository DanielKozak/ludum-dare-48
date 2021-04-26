using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Ship : Building
{
    (int x, int y) position = (21, 39);

    [NonSerialized] public int bananaPrice = 2;
    [NonSerialized] public int fuelPrice = 3;

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
        GameManager.Instance.SetBananaBalance(-1 * amount * fuelPrice);

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
                    UIManager.Instance.ShowContextMenu("Buy truckload of oil", () =>
           {
               truck.AddOrder(new TruckActions.Load(truck, this, interactPos, -1));
           });
                    break;
                case 0:
                    UIManager.Instance.ShowContextMenu("Sell Bananas", () =>
           {
               truck.AddOrder(new TruckActions.Unload(truck, this, interactPos, 0));
           });
                    break;
                case 1:
                    UIManager.Instance.ShowContextMenu("Sell Fuel", () =>
           {
               truck.AddOrder(new TruckActions.Unload(truck, this, interactPos, 1));
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
        List<string> items = new List<string>();
        items.Add($"Banana Price: 1");
        items.Add($"Banana oil Price: 3");

        // public int BananaCount = 0;
        UIManager.Instance.ShowInfoPanel(name, items);
    }
}