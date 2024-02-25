using Unity.Entities;
using Unity.Burst;
using UnityEngine.SocialPlatforms;
using Unity.Transforms;

[BurstCompile]
public partial struct SpawnerSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (RefRW<SpawnerComponent> spawner in SystemAPI.Query<RefRW<SpawnerComponent>>())
        {
            UpdateSpawner(ref state, spawner);
        }
    }

    private void UpdateSpawner(ref SystemState state, RefRW<SpawnerComponent> spawner)
    {
        spawner.ValueRW.Timer += SystemAPI.Time.DeltaTime;
        if (spawner.ValueRO.Timer < spawner.ValueRO.SpawningFrequency) return;

        SpawnCube(ref state, spawner);
        spawner.ValueRW.Timer -= spawner.ValueRO.SpawningFrequency;
    }

    private void SpawnCube(ref SystemState state, RefRW<SpawnerComponent> spawner)
    {
        Entity TempCube = state.EntityManager.Instantiate(spawner.ValueRO.EnemyToSpawn);
        LocalTransform NewCubePos = LocalTransform.FromPosition(spawner.ValueRO.EnemyPosition);
        state.EntityManager.SetComponentData(TempCube, NewCubePos);
    }
}
