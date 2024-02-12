using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{

    
    List<Type> Types = new List<Type>();
    List<Cluster> Clusters = new List<Cluster>();
    List<Wave> Waves = new List<Wave>();


    ///IDENTIFIER
    struct Identifier
    {
        public int KeyInt;
        public string KeyName;
    };
    
    ///Type///
    class Type
    {
        Identifier TypeSignature;
        float Damage;
        float Speed;
        float Health;

    }

    ///Cluster///
    class Cluster
    {
       //Type ID
       //Count
    }

    class Wave
    {  
        //ClusterIDs
        //Count
    }

    ///Wave//

    

    private void CreateType(float Health, float Speed, float Damage)
    {

    }

}
