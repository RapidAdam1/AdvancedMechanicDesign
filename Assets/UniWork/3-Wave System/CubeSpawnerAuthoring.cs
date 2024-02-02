using Unity.Entities;
using UnityEngine;

public class CubeSpawnerAuthoring : MonoBehaviour
{
    public GameObject CubeToSpawn;
    
    [Range(0.1f,1f)] 
    public float SpawnFrequency = 0.1f;
}

[BakingType]
public class CubeSpawnerBaker : Baker<CubeSpawnerAuthoring>
{
    public override void Bake(CubeSpawnerAuthoring authoring)
    {
        Entity CubeEntity = GetEntity(TransformUsageFlags.None);
        AddComponent(CubeEntity, new CubeSpawnerComponent
        {
            CubeToSpawn = GetEntity(authoring.CubeToSpawn, TransformUsageFlags.Dynamic),
            CubePosition = authoring.CubeToSpawn.transform.position,
            SpawningFrequency = authoring.SpawnFrequency,
            Timer = 0f
        });
    }
}