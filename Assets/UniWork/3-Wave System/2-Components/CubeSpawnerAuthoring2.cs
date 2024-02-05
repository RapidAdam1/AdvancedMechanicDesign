using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class CubeSpawnerAuthoring2 : MonoBehaviour
{
    [Range(0.1f, 15f)] public float Speed = 1f;

    private class Baker : Baker<CubeSpawnerAuthoring2>
    {
        public override void Bake(CubeSpawnerAuthoring2 authoring)
        {
            Entity E = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(E, new CubeComponent2
            {
                Speed = authoring.Speed
            });
        }
    }
}

public struct CubeComponent2 : IComponentData
{
    public float Speed;
    
}
