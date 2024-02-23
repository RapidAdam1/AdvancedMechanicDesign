using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


[BurstCompile]
public partial struct EnemyUpdateSystem: ISystem
{
    private ComponentLookup<LocalToWorld> LookupLocal2World;

    private Entity PlayerEntity;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        LookupLocal2World = state.GetComponentLookup<LocalToWorld>();
        state.RequireForUpdate<DesiredLocation>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (PlayerEntity == Entity.Null)
        {
            PlayerEntity = SystemAPI.GetSingletonEntity<DesiredLocation>();
        }

        EntityCommandBuffer.ParallelWriter ecb = GetECB(ref state);

        LookupLocal2World.Update(ref state);

        EnemyUpdateJob UpdatePosJob = new EnemyUpdateJob
        {
            ECB = ecb,
            LookupLocal2World = LookupLocal2World,
            deltaTime = SystemAPI.Time.DeltaTime,
            PlayerEntity = PlayerEntity,
        };

        UpdatePosJob.ScheduleParallel();
    }

    private EntityCommandBuffer.ParallelWriter GetECB(ref SystemState state)
    {
        BeginSimulationEntityCommandBufferSystem.Singleton ECBSinglton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        EntityCommandBuffer ecb = ECBSinglton.CreateCommandBuffer(state.WorldUnmanaged);
        return ecb.AsParallelWriter();
    }
}

[BurstCompile]
public partial struct EnemyUpdateJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter ECB;
    public Entity PlayerEntity;
    public float deltaTime;

    [ReadOnly]
    public ComponentLookup<LocalToWorld> LookupLocal2World;

    public void Execute([ChunkIndexInQuery] int index, in EnemyComponent EnemyComp, in Entity e, in LocalToWorld EnemyL2W)
    {
        LocalToWorld TargetL2W = LookupLocal2World[PlayerEntity];
        float3 DesiredPos = TargetL2W.Position;

        float3 TargetVector = DesiredPos - EnemyL2W.Position;
        TargetVector = math.normalizesafe(TargetVector) * deltaTime * EnemyComp.Speed;

        float3 TargetPos = EnemyL2W.Position + TargetVector;

        LocalTransform transform = LocalTransform.FromPosition(TargetPos);
        ECB.SetComponent<LocalTransform>(index, e, transform);
    }
}