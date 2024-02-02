using Unity.Entities;
using Unity.Mathematics;

public struct CubeSpawnerComponent : IComponentData
{
    public Entity CubeToSpawn;
    public float3 CubePosition;
    public float SpawningFrequency;
    public float Timer;
}
