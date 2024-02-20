using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class WaveData
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
        ProcessWave(Waves[0]);
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
        }
    }

    void ReadCluster(ParsedBlock Cluster)
    {
        Regex RegexPattern = new Regex(@"(\d*):(\d*)");
        MatchCollection RegexMatch = RegexPattern.Matches(Cluster.content);
        
        for (int i = 0; i < RegexMatch.Count; i++)
        {
            int ReadID = int.Parse(RegexMatch[i].Groups[1].ToString());
            int IDCount = int.Parse(RegexMatch[i].Groups[2].ToString());

            for(int j = 0; j < IDCount; j++)
            {
                SpawnType(GetBlockFromID(ReadID, Types));
            }
        }
    }

    void ProcessWave(ParsedBlock Wave)
    {
        Regex RegexPattern = new Regex(@"(?:(\w)(\d)*(?:\<(\d*)\>)*(?:\[(\d)\])*[^\!\?]*)");
        MatchCollection RegexMatch = RegexPattern.Matches(Wave.content);
        for (int i = 0; i < RegexMatch.Count; i++)
        {


            //TypeToRead
            char Type = RegexMatch[i].Groups[1].Value.ToString()[0];
            int ID = int.Parse(RegexMatch[i].Groups[2].Value.ToString());
            int WaitTime = 0;
            int Threshold = 0;

            WaitTime = RegexMatch[i].Groups[3].Success ? int.Parse(RegexMatch[i].Groups[3].ToString()) : 0;
            Threshold = RegexMatch[i].Groups[4].Success ? int.Parse(RegexMatch[i].Groups[4].ToString()) : 0;

            if (WaitTime != 0 || Threshold != 0)
            {
                float CurrWaitTime = 0;
                int CurrEnemies = 5;
            }
            //SpawnEnemy
            if (Type == 'T')
                SpawnType(GetBlockFromID(ID, Types));
            else if (Type == 'C')
                ReadCluster(GetBlockFromID(ID, Clusters));

            //NextRead
        }
    }
    IEnumerator ReadWave(ParsedBlock Wave)
    {
        Regex RegexPattern = new Regex(@"(?:(\w)(\d)*(?:\<(\d*)\>)*(?:\[(\d)\])*[^\!\?]*)");
        MatchCollection RegexMatch = RegexPattern.Matches(Wave.content);
        for (int i = 0; i < RegexMatch.Count; i++)
        {


            //TypeToRead
            char Type = RegexMatch[i].Groups[1].Value.ToString()[0];
            int ID = int.Parse(RegexMatch[i].Groups[2].Value.ToString());
            int WaitTime = 0;
            int Threshold = 0;

            if (RegexMatch[i].Groups[3].Success)
                WaitTime = int.Parse(RegexMatch[i].Groups[3].ToString());
            if (RegexMatch[i].Groups[4].Success)
                Threshold = int.Parse(RegexMatch[i].Groups[4].ToString());

            if (WaitTime != 0 || Threshold!= 0)
            {
                float CurrWaitTime = 0;
                int CurrEnemies = 5;
                /*                while (CurrWaitTime >= WaitTime || Threshold< CurrEnemies)
                                {
                                    CurrEnemies = 5;//Get Enemies
                                    CurrWaitTime += Time.deltaTime;
                                    yield return new WaitForFixedUpdate();
                                }*/

            }
            //SpawnEnemy
            if (Type == 'T')
                SpawnType(GetBlockFromID(ID, Types));
            else if (Type == 'C')
                ReadCluster(GetBlockFromID(ID, Clusters));
        }

        yield break;
    }
}
