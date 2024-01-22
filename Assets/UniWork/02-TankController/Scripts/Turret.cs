using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Turret : MonoBehaviour
{
	[SerializeField] private Transform m_CameraMount;
	[SerializeField] private Transform m_Turret;
	[SerializeField] private Transform m_Barrel;

	private TankSO m_Data;
	private bool m_RotationDirty;
	private Coroutine m_CRAimingTurret;

	private void Awake()
	{
	}

	public void Init(TankSO inData)
	{
		
	}

	public void SetRotationDirty()
	{
		Vector3 ProjectedVector = Vector3.ProjectOnPlane(transform.position,transform.forward);

		Quaternion DesiredRotation = Quaternion.LookRotation(ProjectedVector, Vector3.up);

	}

	private IEnumerator C_AimTurret()
	{
		yield return null;
	}
}
