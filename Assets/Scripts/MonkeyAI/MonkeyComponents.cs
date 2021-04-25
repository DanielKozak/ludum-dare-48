using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

namespace MonkeyComponents
{
    public struct Monkey_Tag : IComponentData { }
    public struct IsActive_Tag : IComponentData { }
    public struct IsVisible_Tag : IComponentData { }
    public struct Attractor_Component : IComponentData
    {
        public float3 pos;
        public int str;
        public Attractor_Component(float3 pos, int str)
        {
            this.pos = pos;
            this.str = str;
        }
    }
    public struct Repulsor_Component : IComponentData
    {
        public float3 pos;
        public int str;
        public Repulsor_Component(float3 pos, int str)
        {
            this.pos = pos;
            this.str = str;
        }
    }

    public struct Mnk_Physics_Component : IComponentData
    {
        public float speed;
        public float2 vel;
        public Mnk_Physics_Component(bool amIaMonke)
        {
            speed = 1f;
            vel = new float2();
        }
    }
    public struct Mnk_Renderer : IComponentData
    {
    }
    public struct Mnk_target : IComponentData
    {
        public float3 pos;
        public Mnk_target(float3 pos)
        {
            this.pos = pos;
        }
    }
}
