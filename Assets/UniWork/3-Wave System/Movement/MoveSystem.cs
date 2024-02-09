using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
[BurstCompile]
public partial struct MoveSystem : ISystem
{
    [BurstCompile]

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<InputMoveComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        MoveJob Job = new MoveJob
        {
            DeltaTime = SystemAPI.Time.DeltaTime,
            Movespeed = 4f
        };

        Job.ScheduleParallel();
    }
    [BurstCompile]
    private partial struct MoveJob : IJobEntity
    {
        public float DeltaTime;
        public float Movespeed;
        public void Execute(ref LocalTransform LocTransform, in InputMoveComponent InMove)
        {
            float3 MoveVector = new float3(InMove.Value.x, 0, InMove.Value.y) * DeltaTime * Movespeed;
            LocTransform = LocTransform.Translate(MoveVector);
        }
    }
}
