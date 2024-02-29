using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Collections;
using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;
using System;
using static UnityEngine.Rendering.DebugUI;

[BurstCompile]
public partial struct SpawnerSystem : ISystem
{

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {

        foreach (RefRW<SpawnerComponent> spawner in SystemAPI.Query<RefRW<SpawnerComponent>>())
        {
            spawner.ValueRW.CurrWaveIndex = ECSTomBenBlockParser.GetFirstWaveID(ECSTomBenBlockParser.InitialDataRead(spawner.ValueRO.Location.ToString()));
            Debug.Log("First Read, Set the Wave");

        }
    }

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
        List<ParsedBlock> ReadBlocks = ECSTomBenBlockParser.InitialDataRead(spawner.ValueRO.Location.ToString());
        ParsedBlock CurrentWave = ECSTomBenBlockParser.GetBlockFromID(spawner.ValueRO.CurrWaveIndex, "wave", ReadBlocks);

        if (CurrentWave.id == -1)
        {
            Debug.LogError("INVALID WAVE");
            return;
        }
        
        if(spawner.ValueRO.IsFirstRead)
        {
            ReadWaveAtStep(ref state, spawner, ReadBlocks, CurrentWave, spawner.ValueRO.CurrMatchIndex);
            return;
        }

        if (spawner.ValueRO.ThresholdReq || spawner.ValueRO.TimerReq)
        {
            int Enemies = 3; // Get Enemies
            spawner.ValueRW.Timer -= Time.deltaTime;
            if (spawner.ValueRO.ThresholdReq && (Enemies <= spawner.ValueRO.Threshold) || spawner.ValueRO.TimerReq && (spawner.ValueRO.Timer <= 0))
            {
                spawner.ValueRW.ThresholdReq = false;
                spawner.ValueRW.TimerReq = false;
            }
            else
                return;
        }
        ReadWaveAtStep(ref state, spawner, ReadBlocks, CurrentWave, spawner.ValueRO.CurrMatchIndex);

    }

    public void ReadWaveAtStep(ref SystemState state, RefRW<SpawnerComponent> spawner, List<ParsedBlock> Blocks, ParsedBlock CurrentWave, int Step)
    {
        Regex RegexPattern = new Regex(@"(?:(\w)(\d)*(?:\<(\d*)\>)*(?:\[(\d)\])*[^\!\?]*)");
        MatchCollection RegexMatch = RegexPattern.Matches(CurrentWave.content);

        Debug.Log(RegexMatch[Step]);
        //IfFinished

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
            SpawnType(ref state,spawner,ECSTomBenBlockParser.GetBlockFromID(ID,"type",Blocks));
        else if (Type == 'C')
            ReadCluster(ref state,spawner,Blocks, ECSTomBenBlockParser.GetBlockFromID(ID, "cluster", Blocks));

        spawner.ValueRW.CurrMatchIndex++;
        spawner.ValueRW.IsFirstRead = true;
        spawner.ValueRW.ThresholdReq = false;
        spawner.ValueRW.TimerReq = false;

        if(spawner.ValueRW.CurrMatchIndex > RegexMatch.Count)
        {
            spawner.ValueRW.CurrMatchIndex = 0;
            spawner.ValueRW.CurrWaveIndex = ECSTomBenBlockParser.GetNextWaveID(CurrentWave.id, Blocks);
            if(spawner.ValueRO.CurrWaveIndex == -1)
            {
                //Disable Spawner Component
            }
        }
    }


    void ReadCluster(ref SystemState state, RefRW<SpawnerComponent> spawner, List<ParsedBlock> Blocks,ParsedBlock Cluster)
    {
        Regex RegexPattern = new Regex(@"(\d*):(\d*)");
        MatchCollection RegexMatch = RegexPattern.Matches(Cluster.content);
        for (int i = 0; i < RegexMatch.Count; i++)
        {
            int ReadID = int.Parse(RegexMatch[i].Groups[1].ToString());
            int IDCount = int.Parse(RegexMatch[i].Groups[2].ToString());

            for (int j = 0; j < IDCount; j++)
            {
                SpawnType(ref state,spawner, ECSTomBenBlockParser.GetBlockFromID(ReadID, "type", Blocks));
            }
        }
    }


    public void SpawnType(ref SystemState state, RefRW<SpawnerComponent> spawner, ParsedBlock Type)
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
                    Health = int.Parse(RegexMatch[i].Groups[2].ToString());
                    break;

            }

        }
        Debug.Log($"Spawned {Type.id}");
        Entity TempCube = state.EntityManager.Instantiate(spawner.ValueRO.EnemyToSpawn);
        float3 RandomOffset = new float3 (0,0, UnityEngine.Random.Range(-5, 6));
        LocalTransform NewCubePos = LocalTransform.FromPosition(spawner.ValueRO.EnemyPosition + RandomOffset);
        state.EntityManager.SetComponentData(TempCube, NewCubePos);

        // Set Enemy Component data
    }
}
