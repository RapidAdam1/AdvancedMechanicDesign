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

    void Init(int speed,int damage,int health)
    {
        m_Speed = speed;
        m_Health = health;
        m_Damage = damage;
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
