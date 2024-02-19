using System.Collections;
using System.Collections.Generic;
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


        Debug.Log("Sorted Lists");
        Debug.Log($"Waves:{Waves.Count}");
        Debug.Log($"Clusters:{Clusters.Count}");
        Debug.Log($"Types:{Types.Count}");

    }

    ParsedBlock GetBlockFromID(int ID, List<ParsedBlock> List)
    {
        for (int i = 0; i < List.Count; i++)
        {
            if (List[i].id == ID)
                return List[i];
        }
        Debug.LogError("NO FOUND ID");
        return List[0];
    }
}
