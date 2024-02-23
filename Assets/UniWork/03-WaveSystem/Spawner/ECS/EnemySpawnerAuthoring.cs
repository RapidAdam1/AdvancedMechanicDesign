using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class EnemyAuthoring : MonoBehaviour
{
    public GameObject cubeToSpawn;
    public int damage;
    public int health;
    public int speed;
    private class Baker : Baker<EnemyAuthoring>
    {
        public override void Bake(EnemyAuthoring Authoring)
        {
            Entity NewEnemy = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(NewEnemy, new EnemyComponent
            {
                Damage = Authoring.damage,
                Health = Authoring.health,
                Speed = Authoring.speed,
            });
            SetComponentEnabled<EnemyComponent>(NewEnemy, false);
        }
    }
}

public struct EnemyComponent : IComponentData, IEnableableComponent
{
    public Entity E_Enemy;
    public float3 Position;
    public int Damage;
    public int Health;
    public int Speed;
}