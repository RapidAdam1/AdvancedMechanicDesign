using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EngineData", menuName ="DataObject/Tank/EngineData")]
public class EngineSO : ScriptableObject
{
	public float HorsePower;
	[Range(0.1f,1)] public float RotatePivotOffset;
}
