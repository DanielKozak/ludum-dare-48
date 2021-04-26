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
    [NonSerialized]
    public int bananasPerReach = 1;
    [NonSerialized]
    public float armReachSpeed = 3f;

    int currentArms = 0;
    [NonSerialized]
    public int fuelConsumptionPerCollect = 2;
    public int maxFuel = 100;
    public int remainingFuel { get; private set; }

    public void SetFuel(int delta)
    {
        remainingFuel += delta;
        UpdateResourceCounter();
    }

    public int BananaCount { get; private set; }
    public void SetBananaCount(int delta)
    {
        BananaCount += delta;
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
        remainingFuel = 100;
        position = pos;
        Vector3 offsetPositionV3 = MapUtils.GetWorldPosByCellWithLayer(pos);
        offsetPositionV3.y += 0.2f;
        transform.position = offsetPositionV3;
        transform.DOMoveY(offsetPositionV3.y - 0.2f, 0.1f);
        AudioManager.PlaySound("placeBuilding");
        //EFFECT
        GameManager.Instance.SetBananaBalance(-price);
        StartCoroutine(DoWork());
    }

    IEnumerator DoWork()
    {
        while (true)
        {
            // Debug.Log($"Extractor {id} cycle");
            yield return new WaitForSecondsRealtime(0.2f);
            if (remainingFuel <= 0)
            {
                //Toast 
                remainingFuel = 0;
                yield return new WaitForRefuel(this);
            }
            if (remainingFuel <= 20)
            {
                //Toast 
            }
            if (armsOut <= maxArms)
            {
                var tileList = MapUtils.GetNeighboursByDistance(position.x, position.y, MapController.Instance.MapW, MapController.Instance.MapH,
                                                                    armDistance, false);
                List<(int, int)> bananaTiles = new List<(int, int)>();
                foreach (var item in tileList)
                {
                    if (MapController.Instance.BananaMap.GetValue(item) > 0)
                    {
                        bananaTiles.Add(item);
                    }
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
    public GameObject ArmPrefab;

    public List<GameObject> ArmObjects = new List<GameObject>();
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
        yield return new WaitForSecondsRealtime(armReachSpeed * 0.5f);
        MapController.Instance.BananaMap.SetValueAdditive(bananaTile, -bananasPerReach);
        // Debug.Log(MapController.Instance.BananaMap.GetValue(bananaTile));
        if (MapController.Instance.BananaMap.GetValue(bananaTile) <= 0)
        {
            MapController.Instance.RemoveJungle(bananaTile);
        }
        yield return new WaitForSecondsRealtime(armReachSpeed * 0.5f);
        lifeTimeBananas += bananasPerReach;
        BananaCount += bananasPerReach;
        UpdateResourceCounter();
        Destroy(lineObject);
        occupiedTiles.Remove(bananaTile);
        armsOut--;

    }



    public class WaitForRefuel : CustomYieldInstruction
    {
        BananaExtractor extractor;
        public WaitForRefuel(BananaExtractor parent)
        {
            extractor = parent;
            Debug.Log("Waiting for refuel");
        }
        public override bool keepWaiting
        {
            get
            {
                return extractor.remainingFuel > 0;
            }
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
                    UIManager.Instance.ShowContextMenu("Load Bananas", () =>
           {
               truck.AddOrder(new TruckActions.Load(truck, this, interactPos, 0));
           });
                    break;
                case 0:
                    UIManager.Instance.ShowContextMenu("Load MORE Bananas", () =>
           {
               truck.AddOrder(new TruckActions.Load(truck, this, interactPos, 0));
           });
                    break;
                case 1:
                    UIManager.Instance.ShowContextMenu("Refuel", () =>
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

    [NonSerialized]
    public int lifeTimeBananas = 0;
    public override void ShowInfo()
    {
        List<string> items = new List<string>();
        items.Add($"Cost : {cost}");
        items.Add($"Arm reach distance : {armDistance} ");
        items.Add($"Bananas per grab: {bananasPerReach}");
        items.Add($"Lifetime bananas : {lifeTimeBananas}");
        items.Add($"Arm speed : {armReachSpeed} sec/grab");
        items.Add($"Max fuel : {maxFuel} sec/banana");
        items.Add($"Remaining fuel : {remainingFuel} ");
        items.Add($"Fuel  per grab : {remainingFuel} ");
        items.Add($"Bananas in storage : {BananaCount}");
        items.Add($"Collects bananas, destroying jungle in the process. needs to be supplied from oil refineries");

        // public int BananaCount = 0;
        UIManager.Instance.ShowInfoPanel(name, items);
    }

    public void SetResource(int resourceType, int countDelta)
    {
        if (resourceType == 1) //fuel
        {
            remainingFuel += countDelta;
            SecondResourceText.text = $"{remainingFuel}/{maxFuel}";
            return;
        }
        if (resourceType == 0) //bananas
        {
            BananaCount += countDelta;
            FirstResourceText.text = $"{BananaCount}";
            return;
        }
    }
    public void UpdateResourceCounter()
    {
        FirstResourceText.text = $"{BananaCount}";
        SecondResourceText.text = $"{remainingFuel}/{maxFuel}";
    }
}