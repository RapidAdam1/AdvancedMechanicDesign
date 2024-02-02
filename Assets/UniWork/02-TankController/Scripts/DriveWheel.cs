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

	private float m_Acceleration;
	public void SetAcceleration(float amount)
	{
		m_Acceleration = amount;
		Debug.Log(amount);
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
		if (newGrounded)
			m_NumGroundedWheels += 1;
		else
			m_NumGroundedWheels -= 1;
	}

	private void FixedUpdate()
	{
		if (m_NumGroundedWheels == 0 || m_Acceleration == 0)
			return;

		Vector3 AveragePos = Vector3.zero;
		for (int i = 0; i < m_SuspensionWheels.Length; i++)
		{
			
		}
		AveragePos = AveragePos / m_SuspensionWheels.Length;
		float Traction = m_SuspensionWheels.Length / m_NumGroundedWheels;
		m_RB?.AddForceAtPosition(AveragePos*m_Acceleration, transform.forward * m_Acceleration * Traction, ForceMode.Acceleration);
	}
}