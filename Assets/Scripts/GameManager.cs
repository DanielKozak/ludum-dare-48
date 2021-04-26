using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private void Awake() => Instance = this;

    public TMPro.TMP_Text BananaBalancelabel;

    // public List<float3> SpawnPointList = new List<float3>();
    public float SpawnRadius = 20f;

    public (int, int) TruckSpawnPoint = (24, 35);
    public Camera camera;

    int bananaBalance = 0;

    public int GetBananaBalance()
    {
        return bananaBalance;
    }
    public void SetBananaBalance(int delta)
    {
        if (bananaBalance + delta < 0)
        {
            //FUCK YOU EFFECT
        }
        bananaBalance += delta;
        BananaBalancelabel.text = $"{bananaBalance}";
        Vector3 end = BananaBalancelabel.transform.position;
        BananaBalancelabel.transform.DOShakeScale(0.3f);
    }


    private void Start()
    {
        AudioManager.Instance.Init();
        camera = Camera.main;
        PopulateSpawnPoints();
        CameraControllerTopDown.Instance.InitCamera();
        MapController.Instance.InitMap();
        // MapController.Instance.TerrainMap = WorldGenerator.GenerateWorld(200, 50);
        // MapController.Instance.UpdateMap();
        SetBananaBalance(1000000);
    }

    public void SpawnTruck()
    {
        var truckObject = Instantiate(DataContainer.Instance.TruckPrefab, gameObject.transform);
        truckObject.transform.position = MapUtils.GetWorldPosByCellWithLayer(TruckSpawnPoint);
    }
    void PopulateSpawnPoints()
    {
        // SpawnPointList = new List<float3>();
        // SpawnPointList.Add(new float3(50f, 50f, 0));
    }
    EntityManager entityManager => World.DefaultGameObjectInjectionWorld.EntityManager;

    private void LateUpdate()
    {
        float2 target = new float2();
        var vecttarget = camera.ScreenToWorldPoint(Input.mousePosition);
        target.x = vecttarget.x;
        target.y = vecttarget.y;

        World.DefaultGameObjectInjectionWorld.GetExistingSystem<MonkeyMoveSystem>().target = target;
    }

}
