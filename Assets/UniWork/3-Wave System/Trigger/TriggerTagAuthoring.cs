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
        }
    }

}
public struct TriggerTagComponent : IComponentData
{
    //Blank Tag
}
