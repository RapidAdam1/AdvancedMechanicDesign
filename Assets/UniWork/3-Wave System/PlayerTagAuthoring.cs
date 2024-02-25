using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class PlayerTagAuthoring : MonoBehaviour
{
    private class Baker : Baker<PlayerTagAuthoring>
    {
        public override void Bake(PlayerTagAuthoring authoring)
        {
            Entity E = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(E, new PlayerTag());
        }
    }
}

public struct PlayerTag : IComponentData
{

}