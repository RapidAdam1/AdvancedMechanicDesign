using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class KillVolumeTagAuthoring : MonoBehaviour
{
    private class Baker : Baker<KillVolumeTagAuthoring>
    {
        public override void Bake(KillVolumeTagAuthoring authoring)
        {
            Entity E = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(E, new KillVolumeTagComponent { });
        }
    }
}
public struct KillVolumeTagComponent : IComponentData
{
    //Blank Tag
}