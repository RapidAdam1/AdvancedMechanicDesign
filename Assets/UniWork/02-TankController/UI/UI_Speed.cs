using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class UI_Speed : MonoBehaviour
{
    Text SpeedText;
    public int Speed;
    [SerializeField] Rigidbody TankRB;

    private void Awake()
    {
        SpeedText = GetComponent<Text>();
    }

    private void FixedUpdate()
    {
        Speed = (int)TankRB.velocity.magnitude;
        SpeedText.text = Speed + " mph";
        
    }
}
