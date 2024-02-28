using Unity.Entities;
using Unity.Burst;
using UnityEngine.SocialPlatforms;
using Unity.Transforms;
using Unity.VisualScripting;

[BurstCompile]
public partial struct SpawnerSystem : ISystem
{
    /*    public void OnCreate(ref SystemState state)
        {
            foreach (RefRW<SpawnerComponent> spawner in SystemAPI.Query<RefRW<SpawnerComponent>>())
            {
                ReadWave(ref state, spawner);
            }
        }

        public void ReadWave(ref SystemState state, RefRW<SpawnerComponent> spawner)
        {

        }

        public void OnUpdate(ref SystemState state) 
        {


        }*/



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
        if (spawner.ValueRO.Timer < 1) return;

        SpawnCube(ref state, spawner);
        spawner.ValueRW.Timer -= 1;
    }

    private void SpawnCube(ref SystemState state, RefRW<SpawnerComponent> spawner)
    {
        Entity TempCube = state.EntityManager.Instantiate(spawner.ValueRO.EnemyToSpawn);
        LocalTransform NewCubePos = LocalTransform.FromPosition(spawner.ValueRO.EnemyPosition);
        state.EntityManager.SetComponentData(TempCube, NewCubePos);
    }
}
