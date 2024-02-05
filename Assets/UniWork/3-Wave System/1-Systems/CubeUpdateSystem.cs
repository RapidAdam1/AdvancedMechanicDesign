using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


[BurstCompile]
public partial struct CubeUpdateSystem : ISystem
{
    private ComponentLookup<LocalToWorld> LookupLocal2World;

    private Entity PlayerEntity;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        LookupLocal2World = state.GetComponentLookup<LocalToWorld>();
        state.RequireForUpdate<PlayerTag>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if(PlayerEntity == Entity.Null)
        {
            PlayerEntity = SystemAPI.GetSingletonEntity<PlayerTag>();
        }

        EntityCommandBuffer.ParallelWriter ecb = GetECB(ref state);
        
        LookupLocal2World.Update(ref state);

        CubeUpdateJob Job = new CubeUpdateJob
        {
            ECB = ecb,
            LookupLocal2World = LookupLocal2World,
            deltaTime = SystemAPI.Time.DeltaTime,
            PlayerEntity = PlayerEntity,
        };

        Job.ScheduleParallel();
    }

    private EntityCommandBuffer.ParallelWriter GetECB(ref  SystemState state)
    {
        BeginSimulationEntityCommandBufferSystem.Singleton ECBSinglton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        EntityCommandBuffer ecb = ECBSinglton.CreateCommandBuffer(state.WorldUnmanaged);
        return ecb.AsParallelWriter();
    }
}


[BurstCompile]
public partial struct CubeUpdateJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter ECB;
    public Entity PlayerEntity;
    public float deltaTime;

    [ReadOnly]
    public ComponentLookup<LocalToWorld> LookupLocal2World;

    public void Execute([ChunkIndexInQuery] int index, in CubeComponent2 CubeComp,in Entity e,in LocalToWorld CubeL2W)
    {
        LocalToWorld PlayerL2W = LookupLocal2World[PlayerEntity];
        float3 playerWorldPos = PlayerL2W.Position;

        float3 TargetVector = playerWorldPos - CubeL2W.Position;
        TargetVector = math.normalizesafe(TargetVector) * deltaTime * CubeComp.Speed;
        
        float3 TargetPos = CubeL2W.Position + TargetVector;
        
        LocalTransform transform = LocalTransform.FromPosition(TargetPos);
        ECB.SetComponent<LocalTransform>(index, e, transform);
    }
}