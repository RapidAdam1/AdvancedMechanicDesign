using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class TankController : MonoBehaviour
{
	private AM_02Tank m_ActionMap;
	[SerializeField] private TankSO m_Data;
	[SerializeField] private Rigidbody m_RB;
	[SerializeField] private CameraController m_CameraController;
	[SerializeField] private Turret m_TurretController;
	[SerializeField] private DriveWheel[] m_DriveWheels;
	private float m_InAccelerate;
	private float m_InSteer;
	private bool m_IsSteering;
	private Coroutine m_CRSteer;

	private bool m_IsFiring;
	private Coroutine m_CRFire;

    #region Init
    private void Awake()
	{
		m_ActionMap = new AM_02Tank();
		m_RB = GetComponent<Rigidbody>();
		m_CameraController = GetComponent<CameraController>();
		m_TurretController = GetComponent<Turret>();
		foreach (DriveWheel wheel in m_DriveWheels)
		{
			wheel.Init(m_Data);
		}
		m_TurretController.Init(m_Data);
	}
    private void OnEnable()
	{
		m_ActionMap.Enable();

		m_ActionMap.Default.Accelerate.performed += Handle_AcceleratePerformed;
		m_ActionMap.Default.Accelerate.canceled += Handle_AccelerateCanceled;
		m_ActionMap.Default.Steer.performed += Handle_SteerPerformed;
		m_ActionMap.Default.Steer.canceled += Handle_SteerCanceled;
		m_ActionMap.Default.Fire.performed += Handle_FirePerformed;
		m_ActionMap.Default.Fire.canceled += Handle_FireCanceled;
		m_ActionMap.Default.Aim.performed += Handle_AimPerformed;
		m_ActionMap.Default.Zoom.performed += Handle_ZoomPerformed;
	}
	private void OnDisable()
	{
		m_ActionMap.Disable();

		m_ActionMap.Default.Accelerate.performed -= Handle_AcceleratePerformed;
		m_ActionMap.Default.Accelerate.canceled -= Handle_AccelerateCanceled;
		m_ActionMap.Default.Steer.performed -= Handle_SteerPerformed;
		m_ActionMap.Default.Steer.canceled -= Handle_SteerCanceled;
		m_ActionMap.Default.Fire.performed -= Handle_FirePerformed;
		m_ActionMap.Default.Fire.canceled -= Handle_FireCanceled;
		m_ActionMap.Default.Aim.performed -= Handle_AimPerformed;
		m_ActionMap.Default.Zoom.performed -= Handle_ZoomPerformed;
	}
    #endregion

    #region Input Handling
    private void Handle_AcceleratePerformed(InputAction.CallbackContext context)
	{
		m_InAccelerate = context.ReadValue<float>();
		foreach (DriveWheel wheel in m_DriveWheels)
		{
			wheel.SetAcceleration(m_InAccelerate);
		}
		m_TurretController.SetRotationDirty();
	}
	private void Handle_AccelerateCanceled(InputAction.CallbackContext context)
	{
		m_InAccelerate = context.ReadValue<float>();
		foreach (DriveWheel wheel in m_DriveWheels)
		{
			wheel.SetAcceleration(m_InAccelerate);
		}
		m_TurretController.SetRotationDirty();
	}
	private void Handle_SteerPerformed(InputAction.CallbackContext context)
	{
		m_InSteer = context.ReadValue<float>();

		if (m_IsSteering) return;

		m_IsSteering = true;

		m_CRSteer = StartCoroutine(C_SteerUpdate());
	}
	private void Handle_SteerCanceled(InputAction.CallbackContext context)
	{
		m_InSteer = context.ReadValue<float>();

		if (!m_IsSteering) return;

		m_IsSteering = false;
		foreach (DriveWheel wheel in m_DriveWheels)
		{
			wheel.SetAcceleration(m_InAccelerate);
		}
		StopCoroutine(m_CRSteer);
	}
    private void Handle_FirePerformed(InputAction.CallbackContext context)
    {
        if (m_IsFiring) return;

        m_IsFiring = true;

        m_CRFire = StartCoroutine(C_FireUpdate());
    }
    private void Handle_FireCanceled(InputAction.CallbackContext context)
    {
        if (!m_IsFiring) return;

        m_IsFiring = false;

        StopCoroutine(m_CRFire);
    }
    private void Handle_AimPerformed(InputAction.CallbackContext context)
    {
        m_CameraController.RotateSpringArm(context.ReadValue<Vector2>());
        m_TurretController.SetRotationDirty();
    }
    private void Handle_ZoomPerformed(InputAction.CallbackContext context)
    {
        m_CameraController.ChangeCameraDistance(context.ReadValue<float>());
        m_TurretController.SetRotationDirty();
    }
    #endregion

    private IEnumerator C_SteerUpdate()
	{
		while (m_IsSteering)
		{
			if (m_InAccelerate == 0) //Stationary WORKS DONT DELETE
			{
                m_DriveWheels[0].SetAcceleration(m_InSteer/2);
				m_DriveWheels[1].SetAcceleration(-m_InSteer/2);

			}
			else
			{

				float Dir = (m_InAccelerate * m_Data.EngineData.RotatePivotOffset);

		/*		//*
				FORWARD = 0.5
				REVERSE = -0.5f
				//*
				// FORWARD RIGHT
				// R = DIR L = m_InAccelerate
				// L = DIR R = m_InAccelerate*/

				if(m_InSteer == 1) 
				{
                    m_DriveWheels[1].SetAcceleration(0);
                }
				else if(m_InSteer == -1)
				{
                    m_DriveWheels[0].SetAcceleration(0);

                }
            }
            yield return null;
		}
		
	}

	bool CanFire = true;
	public event Action<bool> OnReloadChange;
	private IEnumerator C_FireUpdate()
	{
		while (m_IsFiring)
		{
			if (CanFire)
			{
				m_TurretController.CallFire();
				StartCoroutine(C_Reload());
			}
			yield return null;
		}
		yield break;
	}
	
	private IEnumerator C_Reload()
    {
		CanFire = false;
		OnReloadChange?.Invoke(CanFire);

		yield return new WaitForSeconds(m_Data.BarrelData.ReloadTime);

		CanFire = true;
		OnReloadChange?.Invoke(CanFire);

		yield break;

	}

}