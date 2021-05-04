using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class BananaExtractor : Building
{
    public int id;
    (int x, int y) position;
    public int cost = 50;
    public int maxArms = 5;
    int armsOut = 5;
    [NonSerialized]
    public int armDistance = 3;
    int armDistance_price = 50;
    [NonSerialized]
    public int bananasPerReach = 1;
    int bananasPerReach_price = 20;
    [NonSerialized]
    public float armReachSpeed = 5f;
    int armReachSpeed_price = 50;

    int currentArms = 0;
    [NonSerialized]
    public int fuelConsumptionPerCollect = 3;
    int fuelConsumptionPerCollect_price = 50;
    public int maxFuel = 100;
    public int remainingFuel { get; private set; }

    public void SetFuel(int delta)
    {
        Debug.Log("refueling extractor by " + delta);
        remainingFuel += delta;
        UpdateResourceCounter();
        Debug.Log("extractor refueled, now  " + remainingFuel);

    }

    public int BananaCount { get; private set; }
    public void SetBananaCount(int delta)
    {
        BananaCount += delta;
        if (BananaCount < 0) BananaCount = 0;
        UpdateResourceCounter();
    }

    (int x, int y) dumpPosition;
    public bool isWorking = true;

    List<(int, int)> occupiedTiles = new List<(int, int)>();

    public Transform Head;
    Camera camera;
    private void Start()
    {
        id = GetHashCode();
        camera = Camera.main;
        OnRightClick = new IActions.Action_Generic(() =>
        {
            ShowContextMenu();
        });
        UpdateResourceCounter();


    }

    public void Place((int x, int y) pos)
    {
        remainingFuel = 50;
        position = pos;
        Vector3 offsetPositionV3 = MapUtils.GetWorldPosByCellWithLayer(pos);
        offsetPositionV3.y += 0.2f;
        transform.position = offsetPositionV3;
        transform.DOMoveY(offsetPositionV3.y - 0.2f, 0.1f);
        AudioManager.PlaySound("placeBuilding");
        //EFFECT
        StartCoroutine(DoWork());
    }

    bool firstFuelToast = false;
    bool secondFuelToast = false;
    IEnumerator DoWork()
    {
        while (isWorking)
        {
            // Debug.Log($"Extractor {id} cycle");
            yield return new WaitForSeconds(0.2f);
            if (remainingFuel <= 0)
            {
                if (!firstFuelToast)
                {
                    firstFuelToast = true;
                    ToastController.Instance.Toast("An extractor has run out of fuel", true, true);
                }

                remainingFuel = 0;
            }
            if (remainingFuel <= 20)
            {

                if (!secondFuelToast)
                {
                    firstFuelToast = true;
                    ToastController.Instance.Toast("An extractor is running out of fuel", false, false);
                }
            }
            if (remainingFuel > 20)
            {
                firstFuelToast = secondFuelToast = true;
            }
            if (armsOut <= maxArms)
            {
                if (remainingFuel > 0)
                {
                    var tileList = MapUtils.GetNeighboursByDistance(position.x, position.y, MapController.Instance.MapW, MapController.Instance.MapH,
                                                                        armDistance, false);
                    currentReachableBananas = 0;
                    List<(int, int)> bananaTiles = new List<(int, int)>();
                    foreach (var item in tileList)
                    {
                        if (MapController.Instance.BananaMap.GetValue(item) > 0)
                        {

                            bananaTiles.Add(item);
                            currentReachableBananas += MapController.Instance.BananaMap.GetValue(item);
                        }
                    }
                    if (bananaTiles.Count == 0)
                    {
                        ToastController.Instance.Toast("An extractor has depleated it's banana supply");
                        isWorking = false;
                        yield break;
                    }
                    int index = UnityEngine.Random.Range(0, bananaTiles.Count);
                    var chosenBanana = bananaTiles[index];
                    // Debug.Log($"{id} at {position} chose {chosenBanana} banana");
                    occupiedTiles.Add(bananaTiles[index]);
                    armsOut++;
                    remainingFuel -= fuelConsumptionPerCollect;
                    if (remainingFuel < 0) remainingFuel = 0;
                    UpdateResourceCounter();

                    StartCoroutine(CollectBanana(chosenBanana));
                }
            }
        }
    }
    public GameObject ArmPrefab;

    public List<GameObject> ArmObjects = new List<GameObject>();
    int currentReachableBananas = 0;
    IEnumerator CollectBanana((int x, int y) bananaTile)
    {
        yield return null;
        GameObject lineObject = Instantiate(ArmPrefab, transform);

        var line = lineObject.GetComponent<Shapes.Line>();
        line.Start = Vector3.zero;

        var wPos = MapUtils.GetWorldPosByCell(bananaTile);
        Vector3 end = transform.worldToLocalMatrix * new Vector4(wPos.x, wPos.y, wPos.z, 1f);
        line.End = Vector3.zero;
        Vector2 delta = wPos - transform.position;

        var lineAngle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
        Vector3 lineDir = new Vector3(0, 0, lineAngle - 90);
        // Debug.Log($" {transform.position} {wPos} {lineAngle}, {lineDir}");
        Head.DORotate(lineDir, 0.2f);
        DOTween.To(() => line.End, x => line.End = x, end, armReachSpeed * 0.5f).OnComplete(() => DOTween.To(() => line.End, x => line.End = x, Vector3.zero, armReachSpeed * 0.5f));

        // DOTween.To(() => line.DashOffset, x => line.DashOffset = x, line.DashOffset - 4, armReachSpeed);
        yield return new WaitForSeconds(armReachSpeed * 0.5f);
        // MapController.Instance.BananaMap.SetValueAdditive(bananaTile, -bananasPerReach);
        MapController.Instance.SetBananaMapValue(bananaTile, MapController.Instance.BananaMap.GetValue(bananaTile) - bananasPerReach);
        // Debug.Log(MapController.Instance.BananaMap.GetValue(bananaTile));
        if (MapController.Instance.BananaMap.GetValue(bananaTile) <= 0)
        {
            MapController.Instance.RemoveJungle(bananaTile);
        }
        yield return new WaitForSeconds(armReachSpeed * 0.5f);
        lifeTimeBananas += bananasPerReach;
        BananaCount += bananasPerReach;
        UpdateResourceCounter();
        Destroy(lineObject);
        occupiedTiles.Remove(bananaTile);
        armsOut--;

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

                    UIManager.Instance.ShowContextMenu("Load Bananas", () =>
           {
               if (BananaCount > 0) truck.AddOrder(new TruckActions.Load(truck, this, interactPos, 0));
           }, "Siphon fuel", () =>
            {
                if (remainingFuel > 0) truck.AddOrder(new TruckActions.Load(truck, this, interactPos, 1));
            }
           );
                    break;
                case 0:
                    UIManager.Instance.ShowContextMenu("Load MORE Bananas", () =>
           {
               if (BananaCount > 0) truck.AddOrder(new TruckActions.Load(truck, this, interactPos, 0));
           });
                    break;
                case 1:
                    UIManager.Instance.ShowContextMenu("Refuel", () =>
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

    [NonSerialized]
    public int lifeTimeBananas = 0;
    public override void ShowInfo()
    {
        // Debug.Log($"{isSelected}");
        // ToastController.Instance.Toast("banana show info");
        if (!isSelected) return;

        List<UIManager.InfoItemStruct> items = new List<UIManager.InfoItemStruct>();
        items.Add(new UIManager.InfoItemStruct("Cost:", $"{cost}", false));
        items.Add(new UIManager.InfoItemStruct("Arm grab distance:", $"{armDistance}", true, armDistance_price,
            () =>
            {
                armDistance += 1; armDistance_price *= 2; isWorking = true;
            }));
        items.Add(new UIManager.InfoItemStruct($"Bananas per grab:", $"{bananasPerReach}", true, bananasPerReach_price,
            () => { bananasPerReach += 1; bananasPerReach_price = Mathf.CeilToInt(bananasPerReach_price * 1.2f); }));
        items.Add(new UIManager.InfoItemStruct("Lifetime bananas:", $"{lifeTimeBananas}", false));
        items.Add(new UIManager.InfoItemStruct("Arm speed:", $"{armReachSpeed}", true, armReachSpeed_price,
            () => { armReachSpeed *= 0.8f; armReachSpeed_price = Mathf.CeilToInt(armReachSpeed_price * 1.5f); }));
        items.Add(new UIManager.InfoItemStruct("Remaining fuel:", $"{remainingFuel}", false));
        items.Add(new UIManager.InfoItemStruct("Fuel per grab:", $"{fuelConsumptionPerCollect}", true, fuelConsumptionPerCollect_price,
            () => { fuelConsumptionPerCollect = Mathf.Clamp(fuelConsumptionPerCollect - 1, 1, 4); fuelConsumptionPerCollect_price = Mathf.CeilToInt(fuelConsumptionPerCollect_price * 2) > 100 ? 0 : Mathf.CeilToInt(fuelConsumptionPerCollect_price * 2); }));
        items.Add(new UIManager.InfoItemStruct("Bananas in storage:", $"{BananaCount}", false));
        items.Add(new UIManager.InfoItemStruct("Reachable bananas:", $"{currentReachableBananas}", false));
        items.Add(new UIManager.InfoItemStruct("Fuel to collect all:", $"{(float)currentReachableBananas * (float)fuelConsumptionPerCollect / (float)bananasPerReach}", false));

        // public int BananaCount = 0;
        UIManager.Instance.ShowInfoPanel(this, name, items);
    }

    public void UpdateResourceCounter()
    {
        ShowInfo();
        FirstResourceText.text = $"{BananaCount}";
        SecondResourceText.text = $"{remainingFuel}";///{maxFuel}";
    }
}