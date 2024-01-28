using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="SuspensionData", menuName ="DataObject/Tank/SuspensionData")]
public class SuspensionSO : ScriptableObject
{
	public float WheelDiameter; 
	public float SuspensionDamper; //Damper Strength (Speed the suspension averages
	public float SuspensionStrength; //Force to apply Up Per X???
	public LayerMask SuspensionLayermask;
	public float MaximumSlope;
	public float HullTraverseDegrees;
}
