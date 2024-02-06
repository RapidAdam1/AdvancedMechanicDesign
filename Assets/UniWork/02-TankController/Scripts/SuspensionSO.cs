using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="SuspensionData", menuName ="DataObject/Tank/SuspensionData")]
public class SuspensionSO : ScriptableObject
{
	public float WheelDiameter; 
	public float SuspensionDamper; 
	public float SuspensionStrength;
	public LayerMask SuspensionLayermask;
	public float MaximumSlope;
	[Range (0.05f,1f)] public float FrictionCoefficient;
}
