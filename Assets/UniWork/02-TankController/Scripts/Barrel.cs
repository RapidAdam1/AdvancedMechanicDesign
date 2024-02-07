using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Barrel : MonoBehaviour
{
	[SerializeField] private TankSO m_Data;
	[SerializeField] private Shell m_ShellPrefab;
	[SerializeField] Transform FireTransform;
	[SerializeField] Rigidbody m_RB;
	private float m_CurrentDispersion;

	public void Init(TankSO inData)
	{
		m_Data = inData;
	}

	public void Fire()
	{
		Vector3 FireDir = FireTransform.forward;
		if (m_RB.velocity.magnitude > 0.5f)
        {
			m_CurrentDispersion = m_RB.velocity.magnitude;
			m_CurrentDispersion /= 100;
			FireDir += new Vector3(Random.Range(-m_CurrentDispersion, m_CurrentDispersion),Random.Range(-m_CurrentDispersion,m_CurrentDispersion));
		}
			



		Shell NewShell = Instantiate(m_ShellPrefab, FireTransform.position, FireTransform.rotation);
		NewShell.Init(m_Data.ShellData, FireDir);
	}

}
