using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private void Awake() => Instance = this;



    // public List<float3> SpawnPointList = new List<float3>();
    public float SpawnRadius = 20f;
    public Camera camera;

    int bananaBalance = 0;

    public int GetBananaBalance()
    {
        return bananaBalance;
    }
    public void SetBananaBalance(int delta)
    {
        bananaBalance += delta;
        //UPdate UIs
    }






    private void Start()
    {
        AudioManager.Instance.Init();
        camera = Camera.main;
        PopulateSpawnPoints();
        CameraControllerTopDown.Instance.InitCamera();
        MapController.Instance.TerrainMap = WorldGenerator.GenerateWorld(200, 50);
        MapController.Instance.BananaMap = WorldGenerator.GenerateBananas(200, 50);
        MapController.Instance.UpdateMap();
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
