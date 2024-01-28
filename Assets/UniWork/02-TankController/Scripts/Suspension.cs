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
	[SerializeField] private float m_SpringSize;
	private bool m_Grounded;

	public void Init(SuspensionSO inData)
	{
		m_Data = inData;
		Debug.Log(m_Data.WheelDiameter);

    }

	public bool GetGrounded()
	{
		if (Physics.Raycast(m_Wheel.transform.position, -m_Wheel.transform.up, m_Data.WheelDiameter,  m_Data.SuspensionLayermask))
		{
			return true;
		}
		return false;
	}

	private void FixedUpdate()
	{
        if (GetGrounded()!= m_Grounded) 
		{
			m_Grounded = !m_Grounded;
			OnGroundedChanged?.Invoke(m_Grounded);
		}
		if(m_Grounded)
		{
			Vector3 LocalDir = transform.TransformDirection(Vector3.down);
			Vector3 WorldVelocity = m_RB.GetPointVelocity(transform.position);
			Vector3 SpringVector = transform.position - transform.parent.position;

			float SuspensionOffset = m_SpringSize - Vector3.Dot(SpringVector, LocalDir);
			float SuspensionVelocity = Vector3.Dot(LocalDir, WorldVelocity);
			float SuspensionForce = (SuspensionOffset * m_Data.SuspensionStrength) - (SuspensionVelocity * m_Data.SuspensionDamper);

			m_RB.AddForce(LocalDir * (SuspensionForce / m_RB.mass));
			
		}

	}

    private void OnDrawGizmos()
    {
        if (m_Data!=null)
        {
			Gizmos.DrawRay(m_Wheel.transform.position, -m_Wheel.transform.up * m_Data.WheelDiameter);
        }
    }
}
