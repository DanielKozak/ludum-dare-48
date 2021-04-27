// using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using MonkeyComponents;
using System.Collections.Generic;
public class MonkeySpawnSystem : ComponentSystem
{
    EntityManager e_manager;
    public List<float3> SpawnPointList;

    protected override void OnCreate()
    {
        SpawnPointList = new List<float3>();
        SpawnPointList.Add(new float3(25f, 25f, -22f));
        float spawnRadius = 20f;
        e_manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var monkey_type = e_manager.CreateArchetype(
            typeof(Translation),
            typeof(Rotation),
            typeof(Mnk_Physics_Component),
            //typeof(RenderBounds),
            typeof(Mnk_Renderer),
            typeof(Monkey_Tag),
            typeof(IsActive_Tag),
            typeof(LocalToWorld)
        );
        for (int i = 0; i < 100; i++)
        {
            var unit = e_manager.CreateEntity(monkey_type);
            e_manager.SetName(unit, $"monke {i}");
        }
        Random rng = new Random();
        rng.InitState(42);
        Entities.WithAll<Monkey_Tag>().ForEach((Entity id, ref Translation translation) =>
                {
                    int spawnIndex = rng.NextInt(0, SpawnPointList.Count);
                    translation.Value = new float3(
                        SpawnPointList[spawnIndex].x + rng.NextFloat(0, spawnRadius),
                        SpawnPointList[spawnIndex].y + rng.NextFloat(0, spawnRadius),
                        -22f);
                });
    }

    protected override void OnUpdate()
    {
    }
}