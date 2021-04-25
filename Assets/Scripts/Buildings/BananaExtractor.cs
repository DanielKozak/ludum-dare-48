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
    public float armReachSpeed = 6f;

    int currentArms = 0;

    public int maxFuel = 100;
    [NonSerialized]
    public int remainingFuel = 100;
    [NonSerialized]

    public int BananaCount = 0;

    (int x, int y) dumpPosition;
    public bool isWorking = true;

    public bool isSelected = false;
    List<(int, int)> occupiedTiles = new List<(int, int)>();


    Camera camera;
    private void Start()
    {
        id = GetHashCode();
        camera = Camera.main;
        OnRightClick = new IActions.Action_Generic(() =>
        {
            ShowContextMenu();
        });

    }

    public void Place((int x, int y) pos)
    {
        position = pos;
        Vector3 offsetPositionV3 = MapUtils.GetWorldPosByCell(pos);
        offsetPositionV3.y += 0.2f;
        transform.position = offsetPositionV3;
        transform.DOMoveY(offsetPositionV3.y - 0.2f, 0.1f);
        AudioManager.PlaySound("placeBuilding");
        //EFFECT

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
                StartCoroutine(CollectBanana(chosenBanana));

            }
        }
    }
    public GameObject ArmPrefab;

    public List<GameObject> ArmObjects = new List<GameObject>();
    IEnumerator CollectBanana((int, int) bananaTile)
    {
        yield return null;
        GameObject lineObject = Instantiate(ArmPrefab, transform);

        var line = lineObject.GetComponent<Shapes.Line>();
        line.Start = Vector3.zero;

        var wPos = MapUtils.GetWorldPosByCell(bananaTile);
        Vector3 end = transform.worldToLocalMatrix * new Vector4(wPos.x, wPos.y, wPos.z, 1f);
        line.End = Vector3.zero;
        DOTween.To(() => line.End, x => line.End = x, end, armReachSpeed * 0.5f).OnComplete(() => DOTween.To(() => line.End, x => line.End = x, Vector3.zero, armReachSpeed * 0.5f));
        // DOTween.To(() => line.DashOffset, x => line.DashOffset = x, line.DashOffset - 4, armReachSpeed);
        yield return new WaitForSecondsRealtime(armReachSpeed);

        Destroy(lineObject);
        MapController.Instance.BananaMap.SetValueAdditive(bananaTile, -bananasPerReach);
        Debug.Log(MapController.Instance.BananaMap.GetValue(bananaTile));
        if (MapController.Instance.BananaMap.GetValue(bananaTile) <= 0)
        {
            MapController.Instance.RemoveJungle(bananaTile);
        }
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
        UIManager.Instance.ShowContextMenu("test", () => { Debug.Log("test"); });
    }

    public override void ShowInfo()
    {

    }
}