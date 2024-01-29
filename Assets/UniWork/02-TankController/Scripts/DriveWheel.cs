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
		m_NumGroundedWheels = 3;
		m_RB?.AddForceAtPosition(m_RB.transform.position, m_RB.transform.forward * (m_SuspensionWheels.Length/ m_NumGroundedWheels) * m_Acceleration, ForceMode.Acceleration);
	}
}