using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class SpawnerAuthoring : MonoBehaviour
{
    public GameObject EnemyPrefab;
    public string InputFile;
}

[BakingType]
public class SpawnerBaker : Baker<SpawnerAuthoring>
{
    public override void Bake(SpawnerAuthoring authoring)
    {
        Entity Spawner = GetEntity(TransformUsageFlags.None);
        AddComponent(Spawner, new SpawnerComponent
        {
            EnemyToSpawn = GetEntity(authoring.EnemyPrefab, TransformUsageFlags.Dynamic),
            EnemyPosition = authoring.EnemyPrefab.transform.position,
            Timer = 0f
        });
    }
}
public struct SpawnerComponent : IComponentData
{
    public Entity EnemyToSpawn;
    public float3 EnemyPosition;

    public float Timer;

}
