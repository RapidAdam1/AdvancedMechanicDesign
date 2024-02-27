using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class EnemyAuthoring : MonoBehaviour
{
    float Damage = 0;
    float Speed = 1;
    float Health = 0;
    public void SetData(float InDamage, float InSpeed, float InHealth)
    {
        Damage = InDamage;
        Speed = InSpeed;
        Health = InHealth;
    }
    private class Baker : Baker<EnemyAuthoring>
    {
        public override void Bake(EnemyAuthoring authoring)
        {
            Entity E = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(E, new Enemy
            {
                m_Damage = authoring.Damage,
                m_Speed = authoring.Speed,
                m_Health = authoring.Health
            });
            AddComponent(E, new MarkForDeath { });
            SetComponentEnabled<MarkForDeath>(E,false);
        }
    }
}

public struct Enemy : IComponentData
{
    public float m_Damage;
    public float m_Speed;
    public float m_Health;
}

public struct MarkForDeath : IComponentData, IEnableableComponent
{

}