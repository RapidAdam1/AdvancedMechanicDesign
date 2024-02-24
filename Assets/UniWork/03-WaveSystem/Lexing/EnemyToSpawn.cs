using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class EnemyToSpawn : MonoBehaviour
{
    CapsuleCollider Coll;
    [SerializeField] Vector3 Target;

    public int m_Speed;
    public int m_Health;
    public int m_Damage;
    public int m_ID;
    public void Init(int speed,int damage,int health, int TypeID)
    {
        m_Speed = speed;
        m_Health = health;
        m_Damage = damage;
        m_ID = TypeID;
    }

    private void Awake()
    {
        Coll = GetComponent<CapsuleCollider>();
    }

    void FixedUpdate()
    {
        Vector3.MoveTowards(transform.position, Target, m_Speed * Time.deltaTime);
    }
}
