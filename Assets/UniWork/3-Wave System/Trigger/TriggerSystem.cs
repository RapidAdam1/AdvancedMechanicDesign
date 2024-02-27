using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

public partial struct TriggerSystem : ISystem
{
    private Entity KillVolume; 
    private ComponentLookup<LocalToWorld> LookupLocal2World;
    public void OnCreate(ref SystemState state)
    {
        LookupLocal2World = state.GetComponentLookup<LocalToWorld>();
        state.RequireForUpdate<KillVolumeTagComponent>();
    }
    public void OnUpdate(ref SystemState state)
    {
        if(KillVolume == Entity.Null)
        {
            KillVolume = SystemAPI.GetSingletonEntity<KillVolumeTagComponent>();
        }
    }

    private EntityCommandBuffer.ParallelWriter GetECB(ref SystemState state)
    {
        BeginSimulationEntityCommandBufferSystem.Singleton ECBSinglton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        EntityCommandBuffer ecb = ECBSinglton.CreateCommandBuffer(state.WorldUnmanaged);
        return ecb.AsParallelWriter();
    }

}

public struct OnTriggerOverlap : ITriggerEventsJob
{
    public void Execute(TriggerEvent triggerEvent)
    {
    }
}
