using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using MonkeyComponents;
public class MonkeyRenderSystem : ComponentSystem
{
    Quaternion qrot;

    public Mesh mesh;
    public Material material;
    protected override void OnCreate()
    {
        qrot = Quaternion.Euler(0f, 180f, 0f);
    }

    protected override void OnUpdate()
    {
        if (mesh == null) return;
        Entities.WithAll<Monkey_Tag>().ForEach((
            Entity id,
            ref Translation translation,
            ref Rotation rotation,
            ref Mnk_Renderer renderer,
            ref Mnk_Physics_Component mnk) =>
            {
                Graphics.DrawMesh(
                        mesh,
                        translation.Value - 0.25f,
                        qrot,
                        material,
                        0
                        );
            }

        );
    }
}


