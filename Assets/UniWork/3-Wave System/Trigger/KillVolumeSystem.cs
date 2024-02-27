using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(LateSimulationSystemGroup), OrderLast = true)]
public partial struct KillVolumeSystem : ISystem
{
    private ComponentLookup<MarkForDeath> DeathTagLookup;
    private Entity KillEntity;

    public void OnCreate(ref SystemState state)
    {
        DeathTagLookup = state.GetComponentLookup<MarkForDeath>();
        state.RequireForUpdate<MarkForDeath>();
    }


    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer ecb = GetECB(ref state);
     
        DestroyJob job = new DestroyJob
        {
            ECB = ecb
        };
        job.Schedule();
    }

    private EntityCommandBuffer GetECB(ref SystemState state)
    {
        EndSimulationEntityCommandBufferSystem.Singleton ECBSinglton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        EntityCommandBuffer ecb = ECBSinglton.CreateCommandBuffer(state.WorldUnmanaged);
        return ecb;
    }
}

public partial struct DestroyJob : IJobEntity 
{
    public EntityCommandBuffer ECB;
    public Entity KillEntity;

    public void Execute()
    {
        ECB.DestroyEntity(KillEntity);
    }

}
