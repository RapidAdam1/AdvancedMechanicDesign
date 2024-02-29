using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Collections;
using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;
using System;

[BurstCompile]
public partial class SpawnerSystem : SystemBase
{

    [BurstCompile]
    protected override void OnCreate()
    {
        RequireForUpdate<EnemyPopulationComp>();
        RequireForUpdate<SpawnerComponent>();

    }
    [BurstCompile]
    protected override void OnStartRunning()
    {
        Debug.Log("StartRunning");

        foreach (RefRW<SpawnerComponent> spawner in SystemAPI.Query<RefRW<SpawnerComponent>>())
        {
            spawner.ValueRW.CurrWaveIndex = ECSTomBenBlockParser.GetFirstWaveID(ECSTomBenBlockParser.InitialDataRead(spawner.ValueRO.Location.ToString()));
            Debug.Log("First Read, Set the Wave");
        }
    }

    [BurstCompile]
    protected override void OnUpdate()
    {
        foreach (RefRW<SpawnerComponent> spawner in SystemAPI.Query<RefRW<SpawnerComponent>>())
        {
            UpdateSpawner(spawner);
        }
    }

    private void UpdateSpawner(RefRW<SpawnerComponent> spawner)
    {
        if (spawner.ValueRO.WaitForNextWave)
        {
            if (GetEnemiesInScene() == 0)
                spawner.ValueRW.WaitForNextWave = false;
        }

        //Replace ReadBlocks With a Buffer to improve performance, Didnt have time to do this
        List<ParsedBlock> ReadBlocks = ECSTomBenBlockParser.InitialDataRead(spawner.ValueRO.Location.ToString());
        ParsedBlock CurrentWave = ECSTomBenBlockParser.GetBlockFromID(spawner.ValueRO.CurrWaveIndex, "wave", ReadBlocks);

        if (CurrentWave.id == -1)
        {

            EntityManager.SetComponentEnabled<SpawnerComponent>(spawner.ValueRO.ThisSpawner, false);
            Debug.Log("Waves Complete Disabling The Spawner");

            return;
        }


        if(spawner.ValueRO.IsFirstRead)
        {
            ReadWaveAtStep(spawner, ReadBlocks, CurrentWave, spawner.ValueRO.CurrMatchIndex);
            return;
        }

        if (spawner.ValueRO.ThresholdReq || spawner.ValueRO.TimerReq)
        {

            spawner.ValueRW.Timer -= SystemAPI.Time.DeltaTime;
            if (spawner.ValueRO.ThresholdReq && (GetEnemiesInScene() <= spawner.ValueRO.Threshold) || spawner.ValueRO.TimerReq && (spawner.ValueRO.Timer <= 0))
            {
                spawner.ValueRW.ThresholdReq = false;
                spawner.ValueRW.TimerReq = false;
            }
            else
                return;
        }
        ReadWaveAtStep(spawner, ReadBlocks, CurrentWave, spawner.ValueRO.CurrMatchIndex);

    }

    public void ReadWaveAtStep(RefRW<SpawnerComponent> spawner, List<ParsedBlock> Blocks, ParsedBlock CurrentWave, int Step)
    {
        Regex RegexPattern = new Regex(@"(?:(\w)(\d)*(?:\<(\d*)\>)*(?:\[(\d)\])*[^\!\?]*)");
        MatchCollection RegexMatch = RegexPattern.Matches(CurrentWave.content);


        char Type = RegexMatch[Step].Groups[1].Value.ToString()[0];
        int ID = int.Parse(RegexMatch[Step].Groups[2].Value.ToString());
        if(spawner.ValueRO.IsFirstRead)
        {
            spawner.ValueRW.TimerReq = RegexMatch[Step].Groups[3].Success;
            spawner.ValueRW.ThresholdReq = RegexMatch[Step].Groups[4].Success;
        

            if(spawner.ValueRO.ThresholdReq||spawner.ValueRO.TimerReq)
            {
                if (spawner.ValueRO.TimerReq)
                {
                    spawner.ValueRW.Timer = int.Parse(RegexMatch[Step].Groups[3].ToString());
                }
                if (spawner.ValueRO.ThresholdReq)
                {
                    spawner.ValueRW.Threshold = int.Parse(RegexMatch[Step].Groups[4].ToString());
                
                }

                spawner.ValueRW.IsFirstRead = false;
                return;   
            }
        }

        if (Type == 'T')
            SpawnType(spawner,ECSTomBenBlockParser.GetBlockFromID(ID,"type",Blocks));
        else if (Type == 'C')
            ReadCluster(spawner,Blocks, ECSTomBenBlockParser.GetBlockFromID(ID, "cluster", Blocks));

        spawner.ValueRW.CurrMatchIndex++;
        spawner.ValueRW.IsFirstRead = true;
        spawner.ValueRW.ThresholdReq = false;
        spawner.ValueRW.TimerReq = false;

        if(spawner.ValueRW.CurrMatchIndex > RegexMatch.Count-1)
        {
            spawner.ValueRW.WaitForNextWave = true;
            spawner.ValueRW.CurrMatchIndex = 0;
            spawner.ValueRW.CurrWaveIndex = ECSTomBenBlockParser.GetNextWaveID(CurrentWave.id, Blocks);
            if(spawner.ValueRO.CurrWaveIndex == -1 && spawner.ValueRO.Loopable)
            {
                spawner.ValueRW.CurrWaveIndex = ECSTomBenBlockParser.GetFirstWaveID(Blocks);
            }
            Debug.Log("Next Wave ID: "+spawner.ValueRO.CurrWaveIndex);
        }
    }


    void ReadCluster(RefRW<SpawnerComponent> spawner, List<ParsedBlock> Blocks,ParsedBlock Cluster)
    {
        Regex RegexPattern = new Regex(@"(\d*):(\d*)");
        MatchCollection RegexMatch = RegexPattern.Matches(Cluster.content);
        for (int i = 0; i < RegexMatch.Count; i++)
        {
            int ReadID = int.Parse(RegexMatch[i].Groups[1].ToString());
            int IDCount = int.Parse(RegexMatch[i].Groups[2].ToString());

            for (int j = 0; j < IDCount; j++)
            {
                SpawnType(spawner, ECSTomBenBlockParser.GetBlockFromID(ReadID, "type", Blocks));
            }
        }
    }


    public void SpawnType( RefRW<SpawnerComponent> spawner, ParsedBlock Type)
    {
        int Damage = 0;
        int Speed = 0;
        int Health = 0;

        Regex RegexPattern = new Regex(@"(\w+)=>(\d*)");
        MatchCollection RegexMatch = RegexPattern.Matches(Type.content);
        for (int i = 0; i < RegexMatch.Count; i++)
        {
            switch (RegexMatch[i].Groups[1].ToString())
            {
                case "health":
                    Health = int.Parse(RegexMatch[i].Groups[2].ToString());
                    break;
                case "speed":
                    Speed = int.Parse(RegexMatch[i].Groups[2].ToString());
                    break;
                case "damage":
                    Damage = int.Parse(RegexMatch[i].Groups[2].ToString());
                    break;

            }
        }
        Entity TempCube = EntityManager.Instantiate(spawner.ValueRO.EnemyToSpawn);
        float3 RandomOffset = new float3 (0,0, UnityEngine.Random.Range(-5, 6));
        LocalTransform NewCubePos = LocalTransform.FromPosition(spawner.ValueRO.EnemyPosition + RandomOffset);
        EntityManager.SetComponentData(TempCube, NewCubePos);

        Enemy DataSet = EntityManager.GetComponentData<Enemy>(TempCube);
        DataSet.m_Health = Health;
        DataSet.m_Speed = Speed;
        DataSet.m_Damage= Damage;
        EntityManager.SetComponentData(TempCube, DataSet);

        Entity ECounter = SystemAPI.GetSingletonEntity<EnemyPopulationComp>();
        EnemyPopulationComp Counter = SystemAPI.GetSingleton<EnemyPopulationComp>();
        Counter.Value++;
        EntityManager.SetComponentData(ECounter, Counter);

    }

    int GetEnemiesInScene()
    {
        return SystemAPI.GetSingleton<EnemyPopulationComp>().Value;
    }
}
