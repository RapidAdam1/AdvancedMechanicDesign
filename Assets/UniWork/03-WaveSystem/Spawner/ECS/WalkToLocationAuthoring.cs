using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class WalkToLocation : MonoBehaviour
{
    private class Baker : Baker<WalkToLocation>
    {
        public override void Bake(WalkToLocation authoring)
        {
            Entity E = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(E, new DesiredLocation());
        }
    }
}

public struct DesiredLocation : IComponentData
{

}