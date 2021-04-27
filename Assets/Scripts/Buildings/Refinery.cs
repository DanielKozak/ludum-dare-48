using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Refinery : Building
{
    [NonSerialized]

    public int cost = 50;
    (int x, int y) position;
    [NonSerialized]
    public int conversionRate = 4;
    int conversionRate_price = 25;
    [NonSerialized]
    public float conversionTime = 3;
    int conversionTime_price = 25;

    [NonSerialized]
    public int lifeTimeFuel = 0;


    public int BananaCount { get; private set; }
    public void SetBananaCount(int delta)
    {
        BananaCount += delta;
        UpdateResourceCounter();
    }
    public int currentProduct { get; private set; }
    public void SetProduct(int delta)
    {
        currentProduct += delta;
        UpdateResourceCounter();
    }

    [NonSerialized]
    public float cycleTimer = 10f;
    Camera camera;
    private void Start()
    {
        // id = GetHashCode();
        camera = Camera.main;
        OnRightClick = new IActions.Action_Generic(() =>
        {
            ShowContextMenu();
        });
        // UpdateResourceCounter();


    }

    public void Place((int x, int y) pos)
    {
        position = pos;
        Vector3 offsetPositionV3 = MapUtils.GetWorldPosByCellWithLayer(pos);
        offsetPositionV3.y += 0.2f;
        transform.position = offsetPositionV3;
        transform.DOMoveY(offsetPositionV3.y - 0.2f, 0.1f);
        AudioManager.PlaySound("placeBuilding");
        //EFFECT
        UpdateResourceCounter();
        GameManager.Instance.SetBananaBalance(-price);

        StartCoroutine(DoWork());
    }

    bool isToasted = false;
    IEnumerator DoWork()
    {
        while (true)
        {
            yield return null;
            if (BananaCount < conversionRate)
            {
                if (!isToasted)
                {
                    ToastController.Instance.Toast("Refinery without bananas!", false, true);
                    isToasted = true;
                }
                yield return null;
            }
            if (BananaCount >= conversionRate)
            {
                StartEffect();
                //TODO UI effecrt
                yield return new WaitForSeconds(conversionTime);
                StopEffect();

                BananaCount -= conversionRate;
                currentProduct += 1;
                lifeTimeFuel += 1;
                UpdateResourceCounter();
            }
        }
    }

    void StartEffect()
    {
        //TODO StartEffect
    }
    void StopEffect()
    {

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

                    UIManager.Instance.ShowContextMenu("Load Oil", () =>
           {
               if (currentProduct > 0) truck.AddOrder(new TruckActions.Load(truck, this, interactPos, 1));
           }, "Siphon bananas", () =>
             {
                 if (BananaCount > 0) truck.AddOrder(new TruckActions.Load(truck, this, interactPos, 0));
             });
                    break;
                case 0:
                    UIManager.Instance.ShowContextMenu("Add Bananas", () =>
           {
               truck.AddOrder(new TruckActions.Unload(truck, this, interactPos, -1));
           });
                    break;
                case 1:

                    UIManager.Instance.ShowContextMenu("Load More Oil", () =>
           {
               if (currentProduct > 0) truck.AddOrder(new TruckActions.Load(truck, this, interactPos, 1));
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

    [NonSerialized]
    public int lifeTimeBananas = 0;
    public override void ShowInfo()
    {
        if (!isSelected) return;
        List<UIManager.InfoItemStruct> items = new List<UIManager.InfoItemStruct>();
        items.Add(new UIManager.InfoItemStruct("Cost:", $"{cost}", false));
        items.Add(new UIManager.InfoItemStruct("Bananas per Fuel:", $"{conversionRate}", true, conversionRate_price,
            () => { conversionRate = Mathf.Clamp(conversionRate - 1, 1, 4); conversionRate_price *= 2; }));
        items.Add(new UIManager.InfoItemStruct("Banana conversion time :", $"{conversionTime}s", true, conversionTime_price,
            () => { conversionTime = Mathf.Clamp(conversionTime - 1, 1, 4); conversionTime_price *= 2; }));


        items.Add(new UIManager.InfoItemStruct("Lifetime fuel made :", $"{lifeTimeFuel}", false));
        items.Add(new UIManager.InfoItemStruct("Remaining bananas:", $"{BananaCount}", false));
        items.Add(new UIManager.InfoItemStruct("Fuel in storage:", $"{currentProduct}", false));

        // public int BananaCount = 0;
        UIManager.Instance.ShowInfoPanel(this, name, items);
    }

    public void UpdateResourceCounter()
    {
        FirstResourceText.text = $"{currentProduct}";
        SecondResourceText.text = $"{BananaCount}";
    }
}