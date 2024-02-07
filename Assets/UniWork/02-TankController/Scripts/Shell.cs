using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Shell : MonoBehaviour
{
    ShellSO m_Data;
    Rigidbody m_Rb;
    Collider m_Collider;
    public void Init(ShellSO inData,Vector3 ForwardVector)
    {
        m_Data = inData;
        m_Collider = GetComponent<Collider>(); 
        m_Rb = GetComponent<Rigidbody>();
        m_Rb.velocity = ForwardVector * m_Data.Velocity;
        m_Rb.useGravity = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(this.gameObject);
    }

}
