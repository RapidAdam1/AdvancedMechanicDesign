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

        EntityCommandBuffer ecb = GetECB(ref state);
        state.Dependency = new TriggerOverlapJob
        {
            ECB = ecb,
            KillVolumeLookup = SystemAPI.GetComponentLookup<KillVolumeTagComponent>(),
            EnemyTag = SystemAPI.GetComponentLookup<Enemy>(),
            DeathTag = SystemAPI.GetComponentLookup<MarkForDeath>()
        }.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);
    }


    private EntityCommandBuffer GetECB(ref SystemState state)
    {
        EndSimulationEntityCommandBufferSystem.Singleton ECBSinglton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        EntityCommandBuffer ecb = ECBSinglton.CreateCommandBuffer(state.WorldUnmanaged);
        return ecb;
    }

    public struct TriggerOverlapJob : ITriggerEventsJob
    {


        //Lookup 
        [ReadOnly] public ComponentLookup<KillVolumeTagComponent> KillVolumeLookup;
        public ComponentLookup<Enemy> EnemyTag;
        public ComponentLookup<MarkForDeath> DeathTag;
        public EntityCommandBuffer ECB;

        public void Execute(TriggerEvent triggerEvent)
        {
            Entity entityA = triggerEvent.EntityA;
            Entity entityB = triggerEvent.EntityB;

            bool IsEntityATrigger = KillVolumeLookup.HasComponent(entityA);
            bool IsEntityBTrigger = KillVolumeLookup.HasComponent(entityB);

            if (IsEntityATrigger && IsEntityBTrigger)
                return;

            
            bool IsEntityAEnemy = EnemyTag.HasComponent(entityA);
            bool IsEntityBEnemy = EnemyTag.HasComponent(entityB);

            if (IsEntityATrigger && !IsEntityBEnemy ||
                IsEntityBTrigger && !IsEntityAEnemy)
                return;

            if(entityA!=null && entityB !=null)
            {
                ECB.SetComponentEnabled<MarkForDeath>(entityB, true);
            }
            

        }
    }
}

