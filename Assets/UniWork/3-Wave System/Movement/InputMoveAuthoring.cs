using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class InputMoveAuthoring : MonoBehaviour
{
    private class Baker : Baker<InputMoveAuthoring>
    {
        public override void Bake(InputMoveAuthoring authoring)
        {
            Entity E = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<InputMoveComponent>(E, new InputMoveComponent());
            SetComponentEnabled<InputMoveComponent>(E, false);
        }
    }
}

public struct InputMoveComponent : IComponentData , IEnableableComponent
{
    public float2 Value;
}