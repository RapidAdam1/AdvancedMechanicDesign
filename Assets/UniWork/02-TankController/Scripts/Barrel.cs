using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Barrel : MonoBehaviour
{
	[SerializeField] private TankSO m_Data;
	[SerializeField] private Shell m_ShellPrefab;
	[SerializeField] private ShellSO[] m_AmmoTypes;
	[SerializeField] private int[] m_AmmoCounts;

	[SerializeField] Transform FireTransform;

	private int m_SelectedShell;
	private float m_CurrentDispersion;

	public void Init(TankSO inData)
	{
		m_Data = inData;
	}

	public void Fire()
	{
		Instantiate(m_ShellPrefab, FireTransform.position, FireTransform.rotation);
		m_ShellPrefab.Init(m_Data.ShellData, FireTransform.forward);
	}

}
