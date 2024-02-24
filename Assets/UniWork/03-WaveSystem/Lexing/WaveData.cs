using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class WaveData : MonoBehaviour 
{
    [SerializeField] EnemyToSpawn EnemyPrefab;
    List<ParsedBlock> Types = new List<ParsedBlock>(); 
    List<ParsedBlock> Waves = new List<ParsedBlock>();
    List<ParsedBlock> Clusters = new List<ParsedBlock>();

    int EnemiesSpawned = 0;
    ParsedBlock m_CurrentWave;
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
        m_CurrentWave = Waves[0];
        StartCoroutine(ReadWave());
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
            Instantiate(EnemyPrefab,transform.position,transform.rotation);
        }
        EnemiesSpawned++;
    }

    void ReadCluster(ParsedBlock Cluster)
    {
        Regex RegexPattern = new Regex(@"(\d*):(\d*)");
        MatchCollection RegexMatch = RegexPattern.Matches(Cluster.content);
        Debug.Log($"{Cluster.id}-Number of Groups: {RegexMatch.Count}\n");
        for (int i = 0; i < RegexMatch.Count; i++)
        {
            int ReadID = int.Parse(RegexMatch[i].Groups[1].ToString());
            int IDCount = int.Parse(RegexMatch[i].Groups[2].ToString());
            Debug.Log($"Index {i} - {ReadID} for {IDCount}");

            for(int j = 0; j < IDCount; j++)
            {
                SpawnType(GetBlockFromID(ReadID, Types));
            }
        }
    }



    IEnumerator ReadWave()
    {
        Regex RegexPattern = new Regex(@"(?:(\w)(\d)*(?:\<(\d*)\>)*(?:\[(\d)\])*[^\!\?]*)");
        MatchCollection RegexMatch = RegexPattern.Matches(m_CurrentWave.content);
        for (int i = 0; i < RegexMatch.Count; i++)
        {


            yield return new WaitForSeconds(0.5f);
            //TypeToRead
            char Type = RegexMatch[i].Groups[1].Value.ToString()[0];
            int ID = int.Parse(RegexMatch[i].Groups[2].Value.ToString());
            

            float WaitTime = RegexMatch[i].Groups[3].Success ? int.Parse(RegexMatch[i].Groups[3].ToString()) : 999;
            int Threshold = RegexMatch[i].Groups[4].Success ? int.Parse(RegexMatch[i].Groups[4].ToString()) : 0;
            
            
            float CurrentWaitTime = 0;
            int CurrentEnemiesInScene = 5;
            while (CurrentWaitTime < WaitTime && Threshold < CurrentEnemiesInScene)
            {
                
                CurrentWaitTime += Time.deltaTime;
                CurrentEnemiesInScene = 5;
                yield return new WaitForFixedUpdate();
                break;
            }

            //SpawnEnemy
            if (Type == 'T')
                SpawnType(GetBlockFromID(ID, Types));
            else if (Type == 'C')
                ReadCluster(GetBlockFromID(ID, Clusters));
            
        }
        Debug.Log(EnemiesSpawned);
        yield return null;
    }
    IEnumerator SpawnWaves()
    {
        yield return null;
    }
    /*    void ProcessWave(ParsedBlock Wave)
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
    }*/
}
