using Unity.Entities;
using Unity.Burst;
using UnityEngine.SocialPlatforms;
using Unity.Transforms;

[BurstCompile]
public partial struct CubeSpawnerSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (RefRW<CubeSpawnerComponent> spawner in SystemAPI.Query<RefRW<CubeSpawnerComponent>>())
        {
            UpdateSpawner(ref state, spawner);
        }
    }

    private void UpdateSpawner(ref SystemState state, RefRW<CubeSpawnerComponent> spawner)
    {
        spawner.ValueRW.Timer += SystemAPI.Time.DeltaTime;
        if (spawner.ValueRO.Timer < spawner.ValueRO.SpawningFrequency) return;

        SpawnCube(ref state, spawner);
        spawner.ValueRW.Timer -= spawner.ValueRO.SpawningFrequency;
    }

    private void SpawnCube(ref SystemState state, RefRW<CubeSpawnerComponent> spawner)
    {
        Entity TempCube = state.EntityManager.Instantiate(spawner.ValueRO.CubeToSpawn);
        LocalTransform NewCubePos = LocalTransform.FromPosition(spawner.ValueRO.CubePosition);
        state.EntityManager.SetComponentData(TempCube, NewCubePos);
    }
}
