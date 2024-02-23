using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct EnemySpawnerSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState State)
    {
        State.RequireForUpdate<EnemyComponent>();
    } 

    public void OnUpdate(ref SystemState state)
    {

    }

    private void SpawnEnemy(ref SystemState state, RefRW<EnemyComponent> EnemyToSpawn)
    {
        Entity TempEmemy = state.EntityManager.Instantiate(EnemyToSpawn.ValueRO.E_Enemy);
        LocalTransform NewPos = LocalTransform.FromPosition(EnemyToSpawn.ValueRO.Position);
        state.EntityManager.SetComponentData(TempEmemy, NewPos);
    }
}