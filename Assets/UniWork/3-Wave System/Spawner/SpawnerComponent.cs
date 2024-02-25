using Unity.Entities;
using Unity.Mathematics;

public struct SpawnerComponent : IComponentData
{
    public Entity EnemyToSpawn;
    public float3 EnemyPosition;

    public float SpawningFrequency;
    public float Timer;
}
