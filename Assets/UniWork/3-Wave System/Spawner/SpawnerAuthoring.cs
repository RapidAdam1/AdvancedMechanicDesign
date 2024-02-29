using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class SpawnerAuthoring : MonoBehaviour
{
    public GameObject EnemyPrefab;
    [SerializeField] public string InputFile;
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
            Timer = 0f,
            Location = authoring.InputFile,

            Threshold = 0,
            CurrWaveIndex = 2,
            CurrMatchIndex = 0,
            IsFirstRead = true,
            TimerReq = false,
            ThresholdReq = false
        }) ;
        SetComponentEnabled<SpawnerComponent>(Spawner, true);
    }
}
public struct SpawnerComponent : IComponentData, IEnableableComponent
{
    public Entity EnemyToSpawn;
    public float3 EnemyPosition;
    public FixedString512Bytes Location;
    
    public float Timer;
    public int Threshold;
    public bool ThresholdReq;
    public bool TimerReq;

    public int CurrWaveIndex;
    public int CurrMatchIndex;

    public bool IsFirstRead;
}