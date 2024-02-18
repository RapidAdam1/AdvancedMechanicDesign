using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	[SerializeField] private Transform m_SpringArmTarget;
	[SerializeField] private Transform m_CameraMount;
	[SerializeField] private Camera m_Camera;

	//Zoom && Collision
	private float m_CameraDist = 5f;
	[SerializeField] private float m_MaxDist;
	[SerializeField] private float m_MinDist;

	[SerializeField] private float m_YawSensitivity;
	[SerializeField] private float m_PitchSensitivity;
	[SerializeField] private float m_ZoomSensitivity;

	[SerializeField] private float m_CameraProbeSize; 
	[SerializeField] private Vector3 m_TargetOffset;
	Vector3 CameraOffset;
	[SerializeField] float MaxY = 50;
    private void Awake()
    {
		CameraOffset = m_CameraMount.localPosition;
    }
    public void RotateSpringArm(Vector2 change)
	{
		m_TargetOffset.x += change.x * m_YawSensitivity;
		m_TargetOffset.y -= change.y * m_PitchSensitivity;

		m_TargetOffset.y = Mathf.Clamp(m_TargetOffset.y, -MaxY, MaxY);

		m_SpringArmTarget.transform.rotation = Quaternion.Euler(m_TargetOffset.y, m_TargetOffset.x, 0);
	}

	public void ChangeCameraDistance(float amount)
	{
		m_CameraDist = Mathf.Clamp(m_CameraDist + amount*m_ZoomSensitivity, m_MinDist, m_MaxDist);
	}


	private void LateUpdate()
	{
		float CameraDistance;
		Vector3 RayStart = m_SpringArmTarget.position + CameraOffset;
		Vector3 Direction = m_CameraMount.transform.position - RayStart;

		bool IsCollidingWithGround = Physics.Raycast(RayStart,Direction, out RaycastHit HitInfo, m_CameraDist);
        if (IsCollidingWithGround)
        {
			CameraDistance = HitInfo.distance;
        }
        else
        {
			CameraDistance = m_CameraDist;
        }
		m_CameraMount.localPosition = new Vector3(CameraOffset.x, CameraOffset.y, -CameraDistance);

		m_SpringArmTarget.transform.position = Vector3.MoveTowards(m_SpringArmTarget.transform.position, this.transform.position, 1f);
	}

}