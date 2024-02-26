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
		m_SpringArmTarget.transform.position = Vector3.MoveTowards(m_SpringArmTarget.transform.position, this.transform.position, 1f);
		
		
		float CameraDistance;
		Vector3 RayStart = transform.position + new Vector3(0,CameraOffset.y);
		Vector3 Direction = m_CameraMount.transform.position - RayStart;
		DebugLocationStart = RayStart;
		bool IsCollidingWithGround = Physics.Raycast(RayStart,Direction, out RaycastHit HitInfo, m_CameraDist);
        if (IsCollidingWithGround)
        {
			DebugLocationHit = HitInfo.point;
			CameraDistance = HitInfo.distance-5;
			Debug.DrawRay(RayStart, Direction);
        }
        else
        {
			CameraDistance = m_CameraDist;
        }
	
		m_CameraMount.localPosition = new Vector3(CameraOffset.x, CameraOffset.y, -CameraDistance);

	}

	Vector3 DebugLocationHit = Vector3.zero;
	Vector3 DebugLocationStart = Vector3.zero;
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
		Gizmos.DrawSphere(DebugLocationStart, 0.2f);

		Gizmos.color = Color.red;
		Gizmos.DrawSphere(DebugLocationHit,0.5f);
    }
}