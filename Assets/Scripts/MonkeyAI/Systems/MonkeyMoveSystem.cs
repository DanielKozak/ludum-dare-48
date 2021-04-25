using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using MonkeyComponents;

public class MonkeyMoveSystem : ComponentSystem
{
    public float2 target;
    float monkeSpeed = 5f;

    float closeAssertDistance = 2f;
    float cohesionDistance = 5f;
    float separationDistance = 2f;

    float v1W = 0.1f;
    float v2W = 0.2f;
    float v3W = 1f;
    float v4W = 1f;
    float vttW = 0.05f;
    protected override void OnUpdate()
    {
        float time = Time.DeltaTime;
        Entities.WithAll<Monkey_Tag, Mnk_Physics_Component>().ForEach((
            Entity id,
            ref Translation translation,
            ref Mnk_Physics_Component phys
            ) =>
            {

                // float2 vtt = phys.vel;
                // if (math.distance(target, translation.Value.xy) < closeAssertDistance * 2f)
                // {
                //     vtt = math.normalize(target - translation.Value.xy);
                // }
                float2 vtt = math.normalize(target - translation.Value.xy);

                Debug.DrawLine(translation.Value, translation.Value + new float3(vtt, -22), Color.cyan);
                float2 v1 = Cohesion(id, translation.Value);
                Debug.DrawLine(translation.Value, translation.Value + new float3(v1, -22), Color.magenta);
                float2 v2 = Separation(id, translation.Value);
                Debug.DrawLine(translation.Value, translation.Value + new float3(v2, -22), Color.yellow);
                float2 v3 = Alignement(id, translation.Value);
                //phys.vel = math.normalize(vtt + v2);
                phys.vel = math.normalize(vtt * vttW + v1 * v1W + v2 * v2W);
                Debug.DrawLine(translation.Value, translation.Value + new float3(phys.vel, -22), Color.green);

                translation.Value.xy = translation.Value.xy + phys.vel * monkeSpeed * time;
                translation.Value.z = -22f;
            }

         );
    }

    float2 Cohesion(Entity myId, float3 myPos)
    {
        float3 coordBuffer = new float3();
        int count = 0;
        Entities.WithAll<Monkey_Tag>().ForEach((
              Entity id,
              ref Translation translation
              ) =>
              {
                  if (math.distance(myPos, translation.Value) < closeAssertDistance && myId != id)
                  {
                      coordBuffer.xy += translation.Value.xy;
                      count++;
                  }
              }

          );
        if (count == 0) return new float2(0, 0);
        // Debug.Log(count);

        coordBuffer.x *= 1f / count;
        coordBuffer.y *= 1f / count;
        // Debug.Log(coordBuffer.xy);
        return math.normalize(coordBuffer.xy - myPos.xy);
    }

    float2 Separation(Entity myId, float3 myPos)
    {
        float3 coordBuffer = new float3();
        int count = 0;
        Entities.WithAll<Monkey_Tag>().ForEach((
              Entity id,
              ref Translation translation
              ) =>
              {
                  if (math.distance(myPos, translation.Value) < closeAssertDistance && myId != id)
                  {
                      coordBuffer.xy += translation.Value.xy - myPos.xy;
                      count++;
                  }
              }

          );

        if (count == 0) return new float2(0, 0);
        // Debug.Log(count);

        coordBuffer.x *= 1f / count;
        coordBuffer.y *= 1f / count;
        coordBuffer.xy *= -1;
        // Debug.Log(coordBuffer.xy);

        return coordBuffer.xy * 0.5f;
    }
    float2 Alignement(Entity myId, float3 myPos)
    {
        float3 coordBuffer = new float3();
        int count = 0;
        Entities.WithAll<Monkey_Tag>().ForEach((
              Entity id,
              ref Translation translation
              ) =>
              {
                  if (math.distance(myPos, translation.Value) < closeAssertDistance && myId != id)
                  {
                      coordBuffer.xy += translation.Value.xy;
                      count++;

                  }
              }

          );
        // Debug.Log(coordBuffer.xy);
        if (count == 0) return new float2(0, 0);
        // Debug.Log(count);

        coordBuffer.x *= 1f / count;
        coordBuffer.y *= 1f / count;
        return math.normalize(coordBuffer.xy);
    }
    float3 Attraction()
    {
        return new float3();
    }
    float3 Repulsion()
    {
        return new float3();
    }
}
