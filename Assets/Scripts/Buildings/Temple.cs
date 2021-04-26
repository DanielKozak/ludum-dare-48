using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Temple : Building
{
    (int x, int y) position = (171, 39);

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
        // List<string> items = new List<string>();
        // items.Add($"Banana Price: 1");
        // items.Add($"Banana oil Price: 3");

        // // public int BananaCount = 0;
        // UIManager.Instance.ShowInfoPanel(name, items);
    }
}