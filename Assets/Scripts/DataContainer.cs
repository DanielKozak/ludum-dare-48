using UnityEngine;
using Unity.Entities;
using System.Collections.Generic;

public class DataContainer : MonoBehaviour
{

    public static DataContainer Instance;


    public List<Sprite> TruckSprites;
    public Sprite ProbeGhostSprite;
    public Sprite ExtractorGhostSprite;
    public Sprite OilGhostSprite;
    public Sprite TrapGhostSprite;
    public Sprite ConcreteGhostSprite;


    public GameObject TruckPrefab;
    public GameObject ProbePrefab;
    public GameObject RefineryPrefab;
    public GameObject ExtractorPrefab;

    public Mesh MonkeyMesh;
    public Material MonkeyMaterial;


    public GameObject ContextMenuPrefab;
    public GameObject ContextMenuButtonPrefab;

    public float TruckTimer = 1f;
    private void Awake() => Instance = this;
    EntityManager entityManager => World.DefaultGameObjectInjectionWorld.EntityManager;

    public void SetMonkeyData()
    {

        World.DefaultGameObjectInjectionWorld.GetExistingSystem<MonkeyRenderSystem>().mesh = MonkeyMesh;
        World.DefaultGameObjectInjectionWorld.GetExistingSystem<MonkeyRenderSystem>().material = MonkeyMaterial;
    }
}