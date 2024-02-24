using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class EnemyToSpawn : MonoBehaviour
{
    CapsuleCollider Coll;
    [SerializeField] Transform Target;

    public int m_Speed;
    public int m_Health;
    public int m_Damage;
    public int m_ID;
    bool Active;
    public void Init(int speed,int damage,int health, int TypeID)
    {
        m_Speed = speed;
        m_Health = health;
        m_Damage = damage;
        m_ID = TypeID;
        Active = true;
    }

    private void Awake()
    {
        Coll = GetComponent<CapsuleCollider>();
    }

    void FixedUpdate()
    {
        if(Active)
            transform.position = Vector3.MoveTowards(transform.position, Target.position, m_Speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Death")
            Destroy(gameObject);
    }
}
