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
        }
    }


  /*  [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer.ParallelWriter ecb = GetECB(ref state);

        DestroyJob job = new DestroyJob
        {
            ECB = ecb
        };
        job.ScheduleParallel();
    }

    private EntityCommandBuffer.ParallelWriter GetECB(ref SystemState state)
    {
        BeginSimulationEntityCommandBufferSystem.Singleton ECBSinglton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        EntityCommandBuffer ecb = ECBSinglton.CreateCommandBuffer(state.WorldUnmanaged);
        return ecb.AsParallelWriter();
    }*/
}

public partial struct DestroyJob : IJobEntity 
{
    public EntityCommandBuffer.ParallelWriter ECB;

    public void Execute([ChunkIndexInQuery]int index,Entity e)
    {
        UnityEngine.Debug.Log($"Kill Object {e}");
        ECB.DestroyEntity(index, e);
    }

}
