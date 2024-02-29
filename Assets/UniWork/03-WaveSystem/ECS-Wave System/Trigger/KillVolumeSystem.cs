using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct KillVolumeSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        EndSimulationEntityCommandBufferSystem.Singleton ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        foreach ((RefRO<MarkForDeath> tag, Entity entity) in SystemAPI.Query<RefRO<MarkForDeath>>().WithEntityAccess())
        {
            ecb.DestroyEntity(entity);
            Entity ECounter = SystemAPI.GetSingletonEntity<EnemyPopulationComp>();
            EnemyPopulationComp Counter = SystemAPI.GetSingleton<EnemyPopulationComp>();
            Counter.Value--;
            state.EntityManager.SetComponentData(ECounter, Counter);
        }
    }
}

