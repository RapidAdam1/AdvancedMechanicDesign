using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class WaveData : MonoBehaviour
{
    List<ParsedBlock> Types = new List<ParsedBlock>(); 
    List<ParsedBlock> Waves = new List<ParsedBlock>();
    List<ParsedBlock> Clusters = new List<ParsedBlock>();
    
    /// Class to Manage Spawning Logic of Waves
    public void SortToLists(List<ParsedBlock> list)
    {
        for (int i = 0; i < list.Count; ++i)
        {
            switch (list[i].type)
            {
                case "wave":
                    Waves.Add(list[i]);
                    break;
                case "cluster":
                    Clusters.Add(list[i]);
                    break;
                case "type":
                    Types.Add(list[i]);
                    break;
            }
        }

        ReadWave(Waves[0]);
    }

    ParsedBlock GetBlockFromID(int ID, List<ParsedBlock> List)
    {
        for (int i = 0; i < List.Count; i++)
        {
            if (List[i].id == ID)
                return List[i];
        }
        Debug.LogError("NO FOUND ID");
        return new ParsedBlock();
    }

    void SpawnType(ParsedBlock Type)
    {
        float Damage = 0;
        float Speed = 0;
        float Health = 0;

        Regex RegexPattern = new Regex(@"(\w+)=>(\d*)");
        MatchCollection RegexMatch = RegexPattern.Matches(Type.content);
        for (int i = 0; i < RegexMatch.Count; i++)
        {   
            switch (RegexMatch[i].Groups[1].ToString())
            {
                case "health":
                    Health = float.Parse(RegexMatch[i].Groups[2].ToString());
                    break;
                case "speed":
                    Speed = float.Parse(RegexMatch[i].Groups[2].ToString());
                    break;
                case "damage":
                    Health = float.Parse(RegexMatch[i].Groups[2].ToString());
                    break;

            }
            Debug.Log($"Spawned type {Type.id}");
        }

    }

    void ReadCluster(ParsedBlock Cluster)
    {
        Regex RegexPattern = new Regex(@"(\d*):(\d*)");
        MatchCollection RegexMatch = RegexPattern.Matches(Cluster.content);
        Debug.Log($"Cluster {Cluster.id}");
        
        for (int i = 0; i < RegexMatch.Count; i++)
        {
            int ReadID = int.Parse(RegexMatch[i].Groups[1].ToString());
            int IDCount = int.Parse(RegexMatch[i].Groups[2].ToString());

            Debug.Log($"Spawn ID:{ReadID} for {IDCount}");
            for(int j = 0; j < IDCount; j++)
            {
                //GetTypeFromID
                //SpawnID
            }
        }
    }

    void ReadWave(ParsedBlock Wave)
    {
        /// REGEX Capture All NOT Interrobang (?:(\w)(\d)[^\!\?]*);
        //  (?:(\w)(\d)*(?:\<(\d*)\>)*(?:\[(\d)\])*[^\!\?]*)
        Regex RegexPattern = new Regex(@"(?:(\w)(\d)*(?:\<(\d*)\>)*(?:\[(\d)\])*[^\!\?]*)");
        MatchCollection RegexMatch = RegexPattern.Matches(Wave.content);

        Debug.Log($"Wave ID{Wave.id}");
        for (int i = 0; i < RegexMatch.Count; i++)
        {
            //TypeToRead
            char Type = RegexMatch[i].Groups[1].Value.ToString()[0];
            int ID = int.Parse(RegexMatch[i].Groups[2].Value.ToString());
            int WaitTime = 0;
            int Threshold = 0;

            if (RegexMatch[i].Groups[3].Success)
            WaitTime = int.Parse(RegexMatch[i].Groups[3].ToString());
            if(RegexMatch[i].Groups[4].Success)
            Threshold = int.Parse(RegexMatch[i].Groups[4].ToString());

            if(WaitTime ==0 || Threshold == 0)
            {
                if (Type == 'C')
                {
                    //SpawnType(GetBlockFromID(ID, Clusters));
                }
                else if (Type == 'T')
                {
                    //SpawnType(GetBlockFromID(ID, Types));
                }
                else
                    Debug.LogError($"WrongDataFormatPassed {Type}");
            }

            Debug.Log($"Spawn: {Type} {ID} With Parameters:\nThreshold:{Threshold} WaitTime:{WaitTime}");

        }
    }

    IEnumerator WaitForSpawn()
    {
        float CurrWaitTime = 0;
        int WaitTime = 5;
        int ThresholdEnemies = 5;
        int CurrEnemies= 5;
        
        if(WaitTime != 0 || ThresholdEnemies != 0)
        {
            while (CurrWaitTime >= WaitTime || ThresholdEnemies < CurrEnemies)
            {
                CurrEnemies = 5;//Get Enemies
                CurrWaitTime += Time.deltaTime;
                yield return new WaitForFixedUpdate();
            }
        }
        //SpawnEnemy
        //NextRead
    }
}
