using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class EnemyPopulationAuthoring : MonoBehaviour
{
    private class Baker : Baker<EnemyPopulationAuthoring>
    {
        public override void Bake(EnemyPopulationAuthoring authoring)
        {
            Entity e = GetEntity(TransformUsageFlags.None);
            AddComponent(e, new EnemyPopulationComp());
        }
    }
}

public struct EnemyPopulationComp : IComponentData
{
    public int Value;
}