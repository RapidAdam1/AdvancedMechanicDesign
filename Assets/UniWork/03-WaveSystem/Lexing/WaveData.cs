using Codice.Client.Common;
using Codice.CM.Common;
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

        ReadCluster(Clusters[0]);
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
        /// REGEX PATTERN =    [^!?]+(\w)+\[(\d)\]
        /// REGEX GET DATA BETWEEN INTERROBANGS = (?:[^!?]*)
        /// REGEX GET TYPE/CLUSTER ID (?:[^!?]*(?:(\w)(\d)))

        Regex RegexPattern = new Regex(@"[^!?]+(\w)+\[(\d)\]");
    }
}
