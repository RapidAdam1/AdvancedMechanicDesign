using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class TriggerTagAuthoring : MonoBehaviour
{
    private class Baker : Baker<TriggerTagAuthoring>
    {
        public override void Bake(TriggerTagAuthoring authoring)
        {
            Entity E = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(E, new TriggerTagComponent { });
        }
    }

}
public struct TriggerTagComponent : IComponentData
{
    //Blank Tag
}