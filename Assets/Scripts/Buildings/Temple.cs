using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Temple : Building
{
    (int x, int y) position = (149, 38);
    (int x, int y) interactPos = (88, 38);

    Camera camera;
    private void Start()
    {
        camera = Camera.main;
        OnRightClick = new IActions.Action_Generic(() =>
        {
            ShowContextMenu();
        });


    }



    void ShowContextMenu()
    {

        if (InputManager.GetSelectedBuilding() == null) return;

        if (InputManager.GetSelectedBuilding().isTruck)
        {

            var truck = ((Truck)InputManager.GetSelectedBuilding());



            switch (truck.CargoType)
            {
                case -1:
                    UIManager.Instance.ShowContextMenu("Get Golden Banana! (WIN)", () =>
           {
               truck.AddOrder(new TruckActions.Load(truck, this, interactPos, 99));
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


        items.Add(new UIManager.InfoItemStruct("GOLDEN BANANA:", $"", false));
        items.Add(new UIManager.InfoItemStruct("(Grab with a truck to win)", $"", false));
        // [NonSerialized] public int bananaPrice = 2;
        // [NonSerialized] public int fuelPrice = 2;
        // [NonSerialized] public int fuelBuyPrice = 3;
        // [NonSerialized] public int monkePrice = 10;


        UIManager.Instance.ShowInfoPanel(this, name, items);
    }
}