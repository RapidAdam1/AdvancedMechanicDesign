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

        }
    }
}
public struct KillVolumeTagComponent : IComponentData
{
    //Blank Tag
}