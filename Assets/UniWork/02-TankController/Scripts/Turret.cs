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
		m_Data = inData;
		m_RotationDirty = true;
	}

	public void SetRotationDirty()
	{
		if(m_RotationDirty)
		{
			if(m_CRAimingTurret == null)
			{
				m_CRAimingTurret = StartCoroutine(C_AimTurret());
			}
		}
		else
		{
			if(m_CRAimingTurret != null)
			{
				StopCoroutine(m_CRAimingTurret);
				m_CRAimingTurret = null;
			}
		}
		
	}

	private IEnumerator C_AimTurret()
	{
		while (m_RotationDirty)
		{
			//Aim Turret Bulk
            Vector3 TurretProjection = Vector3.ProjectOnPlane(m_CameraMount.forward,m_Turret.up);
			Quaternion TurretTargetRot = Quaternion.LookRotation(TurretProjection, m_Turret.up);
			m_Turret.rotation = Quaternion.RotateTowards(m_Turret.rotation,TurretTargetRot,m_Data.TurretData.TurretTraverseSpeed * Time.deltaTime);


			Debug.Log(m_Turret.forward);
            yield return new WaitForFixedUpdate();
			
		}
		yield break;
	}
}
