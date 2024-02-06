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
	public void Init(SuspensionSO inData)
	{
		m_Data = inData;
		m_SpringSize = Mathf.Abs(m_Wheel.localPosition.y) + m_Data.WheelDiameter/2f;
    }

	public bool GetGrounded()
	{
		return m_Grounded;
	}

	private void FixedUpdate()
	{
		
		bool NewGrounded = Physics.Raycast(transform.position, -transform.up, out RaycastHit HitInfo, m_SpringSize, m_Data.SuspensionLayermask);
        if (NewGrounded != m_Grounded) 
		{
			m_Grounded = NewGrounded;
			OnGroundedChanged?.Invoke(m_Grounded);
		}
		if(m_Grounded)
		{
			Vector3 LocalDown = transform.TransformDirection(Vector3.down);
			Vector3 WorldVelocity = m_RB.GetPointVelocity(transform.position);

			//Suspension Forces
			float SuspensionOffset = m_SpringSize -HitInfo.distance;
			float SuspensionVelocity = Vector3.Dot(-LocalDown, WorldVelocity);
			float SuspensionForce = (SuspensionOffset * m_Data.SuspensionStrength) - (SuspensionVelocity * m_Data.SuspensionDamper);
			m_RB.AddForceAtPosition(-LocalDown * (SuspensionForce),transform.position,ForceMode.Acceleration);
			
			//Wheel Suspension Animation
			m_Wheel.localPosition = new Vector3(0, m_Data.WheelDiameter / 2f - HitInfo.distance, 0);

			
			//Friction
			float FloorAngle = Vector3.Angle(HitInfo.normal, Vector3.up);
			if(FloorAngle < m_Data.MaximumSlope)
            {
				float ObjectDownForce = 9.8f * m_RB.mass * Mathf.Cos(FloorAngle*Mathf.Deg2Rad);
				float FrictionForce = m_Data.FrictionCoefficient * ObjectDownForce;
				Vector3 Friction = -m_RB.velocity.normalized * FrictionForce;

				m_RB?.AddForceAtPosition(Friction, transform.position, ForceMode.Force);
            }

		}

	}

	public void AnimateWheels(float Turning)
    {

    }
    private void OnDrawGizmos()
    {
        if (m_Data!=null)
        {
			Gizmos.DrawRay(transform.position, -transform.up *m_SpringSize);
        }
    }
}
