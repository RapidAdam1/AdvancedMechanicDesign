using Unity.Entities;
using UnityEngine;

public class SpawnerAuthoring : MonoBehaviour
{
    public GameObject EnemyPrefab;
    
    [Range(0.1f,1f)] 
    public float SpawnFrequency = 0.1f;
}

[BakingType]
public class SpawnerBaker : Baker<SpawnerAuthoring>
{
    public override void Bake(SpawnerAuthoring authoring)
    {
        Entity CubeEntity = GetEntity(TransformUsageFlags.None);
        AddComponent(CubeEntity, new SpawnerComponent
        {
            EnemyToSpawn = GetEntity(authoring.EnemyPrefab, TransformUsageFlags.Dynamic),
            EnemyPosition = authoring.EnemyPrefab.transform.position,
            SpawningFrequency = authoring.SpawnFrequency,
            Timer = 0f
        });
    }
}