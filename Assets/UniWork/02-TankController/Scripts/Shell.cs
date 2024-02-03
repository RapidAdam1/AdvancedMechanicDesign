using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Shell : MonoBehaviour
{
    ShellSO m_Data;
    Rigidbody m_Rb;

    public void Init(ShellSO inData,Vector3 ForwardVector)
    {
        m_Data = inData;
        m_Rb = GetComponent<Rigidbody>();
        m_Rb.velocity = ForwardVector * m_Data.Velocity;
    }
    
}
