using Unity.Entities;
using Unity.Burst;
using UnityEngine.SocialPlatforms;
using Unity.Transforms;
using Unity.VisualScripting;
using System.Text.RegularExpressions;
using UnityEngine;
using Unity.Entities.UniversalDelegates;
using System.Collections.Generic;
using static Unity.Collections.AllocatorManager;
using System.Collections;

[BurstCompile]
public partial struct SpawnerSystem : ISystem
{


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

        //Is Current Wave Valid

        //


        ReadWaveAtStep(ref state, spawner, ReadBlocks, CurrentWave, spawner.ValueRO.CurrMatchIndex);
    }

    public void ReadWaveAtStep(ref SystemState state, RefRW<SpawnerComponent> spawner, List<ParsedBlock> Blocks, ParsedBlock CurrentWave, int Step)
    {
        Regex RegexPattern = new Regex(@"(?:(\w)(\d)*(?:\<(\d*)\>)*(?:\[(\d)\])*[^\!\?]*)");
        MatchCollection RegexMatch = RegexPattern.Matches(CurrentWave.content);
       
        
        //IfFinished
        if(Step > RegexMatch.Count)
        {
            //Next Wave
            return;
        }




        char Type = RegexMatch[Step].Groups[1].Value.ToString()[0];
        int ID = int.Parse(RegexMatch[Step].Groups[2].Value.ToString());

        bool WaitParam = RegexMatch[Step].Groups[3].Success;
        bool ThresholdParam = RegexMatch[Step].Groups[4].Success;
        float WaitTime = 0;
        int Threshold = 0;

        if (WaitParam)
        {
            spawner.ValueRW.Timer = int.Parse(RegexMatch[Step].Groups[3].ToString());
        }
        if (ThresholdParam)
        {
            spawner.ValueRW.Threshold = int.Parse(RegexMatch[Step].Groups[4].ToString());
        }

        if (Type == 'T')
            SpawnType(ref state,spawner,ECSTomBenBlockParser.GetBlockFromID(ID,"type",Blocks));
        else if (Type == 'C')
            ReadCluster(ref state,spawner,Blocks, ECSTomBenBlockParser.GetBlockFromID(ID, "type", Blocks));
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
        Entity TempCube = state.EntityManager.Instantiate(spawner.ValueRO.EnemyToSpawn);
        LocalTransform NewCubePos = LocalTransform.FromPosition(spawner.ValueRO.EnemyPosition);
        state.EntityManager.SetComponentData(TempCube, NewCubePos);
    }

    IEnumerator WaveSpawner()
    {
        while (true)
        {

        yield return new WaitForEndOfFrame();
        }
    }
}
