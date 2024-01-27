using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Suspension : MonoBehaviour
{
	public event Action<bool> OnGroundedChanged; 

	[SerializeField] private Transform m_Wheel;
	[SerializeField] private Rigidbody m_RB;

	private SuspensionSO m_Data;
	private float m_SpringSize;
	private bool m_Grounded;

	RaycastHit HitResult;
	public void Init(SuspensionSO inData)
	{
		m_Data = inData;
    }

	public bool GetGrounded()
	{
		m_SpringSize = m_Data.WheelDiameter;
		Ray CheckRay = new Ray(m_Wheel.transform.position,-m_Wheel.transform.up);
		Physics.Raycast(CheckRay,out HitResult,m_SpringSize,m_Data.SuspensionLayermask);
		
		if (HitResult.distance > m_SpringSize)
		{
			return false;
		}
		return true;
	}

	private void FixedUpdate()
	{
        if (GetGrounded()!= m_Grounded) 
		{
			m_Grounded=!m_Grounded;
			OnGroundedChanged?.Invoke(m_Grounded);
		}
	}
}
