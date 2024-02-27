using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(PhysicsSystemGroup))]
public partial struct TriggerSystem : ISystem
{
    private Entity KillVolume; 
    private ComponentLookup<LocalToWorld> LookupLocal2World;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<KillVolumeTagComponent>();
        state.RequireForUpdate<SimulationSingleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        state.Dependency = new TriggerOverlapJob
        {

        }.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);
    }

    private EntityCommandBuffer.ParallelWriter GetECB(ref SystemState state)
    {
        BeginSimulationEntityCommandBufferSystem.Singleton ECBSinglton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        EntityCommandBuffer ecb = ECBSinglton.CreateCommandBuffer(state.WorldUnmanaged);
        return ecb.AsParallelWriter();
    }

    public struct TriggerOverlapJob : ITriggerEventsJob
    {
        //Lookup 
        [ReadOnly] public ComponentLookup<KillVolumeTagComponent> KillVolumeLookup;
        public ComponentLookup<Enemy> EnemyTag;

        [BurstCompile]
        public void Execute(TriggerEvent triggerEvent)
        {
            Entity entityA = triggerEvent.EntityA;
            Entity entityB = triggerEvent.EntityB;

            bool IsEntityATrigger = KillVolumeLookup.HasComponent(entityA);
            bool IsEntityBTrigger = KillVolumeLookup.HasComponent(entityB);

            bool IsEntityAEnemy = EnemyTag.HasComponent(entityA);
            bool IsEntityBEnemy = EnemyTag.HasComponent(entityB);


            UnityEngine.Debug.Log($"{IsEntityATrigger}+{IsEntityBTrigger}+{IsEntityAEnemy}+{IsEntityBEnemy}");
        }
    }
}

