using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DriveWheel : MonoBehaviour
{
	public event Action<bool> OnGroundedChanged;

	[SerializeField] private Rigidbody m_RB;
	[SerializeField] private TankSO m_Data;
	[SerializeField] private Suspension[] m_SuspensionWheels;
	private int m_NumGroundedWheels;
	private bool m_Grounded;

	Vector3 GV_DrivePos;
	Vector3 GV_DriveForce;

    private float m_Acceleration;
	public void SetAcceleration(float amount)
	{
		m_Acceleration = amount;
	}

	public void Init(TankSO inData)
	{
		m_Data = inData;
		foreach(Suspension Spring in m_SuspensionWheels)
		{
			Spring.Init(m_Data.SuspensionData);
			Spring.OnGroundedChanged += Handle_WheelGroundedChanged;
		}
	}

	private void Handle_WheelGroundedChanged(bool newGrounded)
	{
		m_NumGroundedWheels += newGrounded?1:-1;
	}

	private void FixedUpdate()
	{

		Vector3 DriveForcePos = Vector3.zero;
		Vector3 DriveForce = Vector3.zero;

		//Drive Forces
		if (m_NumGroundedWheels == 0 || m_Acceleration == 0)
		{
			return;
		}
		for (int i = 0; i < m_SuspensionWheels.Length; i++)
		{
			m_SuspensionWheels[i].AnimateWheels(m_Acceleration);
			if (!m_SuspensionWheels[i].GetGrounded())
				continue;
            DriveForcePos += m_SuspensionWheels[i].transform.position;
		}

		float TankAcceleration = m_Data.EngineData.HorsePower / (m_RB.mass / 1000); //Acceleration = (HP/ (Weight(Tons) * velocity)) - Friction

        float Traction = (float)m_NumGroundedWheels/(float)m_SuspensionWheels.Length ;

		DriveForcePos = DriveForcePos / m_NumGroundedWheels;
		DriveForce = ((m_RB.transform.forward * m_Acceleration) * TankAcceleration) * Traction;

		m_RB?.AddForceAtPosition(DriveForce,DriveForcePos ,ForceMode.Acceleration);
        
		if(m_RB.velocity.magnitude > 24f)
		{
			m_RB.velocity = m_RB.velocity.normalized * 24 ;
		}
		
		//TODO - Delete After Testing 
        GV_DriveForce = DriveForce;
		GV_DrivePos = DriveForcePos;
	}
	private void OnDrawGizmos()
    {
		Gizmos.DrawSphere(GV_DrivePos, 0.4f);
		Gizmos.color = Color.red;
		Gizmos.DrawLine(GV_DrivePos, GV_DrivePos + GV_DriveForce);
    }
}