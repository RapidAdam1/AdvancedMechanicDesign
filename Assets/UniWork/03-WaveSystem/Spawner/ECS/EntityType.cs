using Unity.Entities;
using Unity.Mathematics;

public struct EntityType : IComponentData
{
    public Entity m_TypeToSpawn;
    public float3 CubePosition;
    public int Damage;
    public int Health;
    public int Speed;
}
