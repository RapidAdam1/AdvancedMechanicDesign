using PlasticGui.WebApi.Responses;
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

	[SerializeField] private Barrel m_BarrelScript;
	
	private TankSO m_Data;
	private bool m_RotationDirty;
	private Coroutine m_CRAimingTurret;

	public void Init(TankSO inData)
	{
		m_Data = inData;
		m_BarrelScript.Init(inData);
		m_RotationDirty = true;
	}

	public void CallFire()
	{
		m_BarrelScript.Fire();
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
			//Aim Turret
            Vector3 TurretProjection = Vector3.ProjectOnPlane(m_CameraMount.forward,m_Turret.up);
			Quaternion TurretTargetRot = Quaternion.LookRotation(TurretProjection, m_Turret.up);
			m_Turret.rotation = Quaternion.RotateTowards(m_Turret.rotation,TurretTargetRot,m_Data.TurretData.TurretTraverseSpeed * Time.deltaTime);

			
			
            //Aim Barrel

            Vector3 BarrelProjection = m_Turret.InverseTransformVector(Vector3.ProjectOnPlane(m_CameraMount.forward, m_Turret.right).normalized);

			//Get Pitch Value
			float targetPitch = Mathf.Atan(BarrelProjection.y / BarrelProjection.z) * Mathf.Rad2Deg;
            targetPitch = Mathf.Clamp(targetPitch.Remap180(),-m_Data.TurretData.DepressionLimit,m_Data.TurretData.ElevationLimit);
			targetPitch *= -1;
            
			Quaternion NewLocalRoation = Quaternion.Euler(targetPitch, 0,0);
			m_Barrel.localRotation = Quaternion.RotateTowards(m_Barrel.localRotation, NewLocalRoation, m_Data.TurretData.BarrelTraverseSpeed * Time.deltaTime);
            yield return new WaitForFixedUpdate();
			
		}
		yield break;
	}
}
