using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UI_Shells : MonoBehaviour
{
    [SerializeField]TankController controller;
    Image ShellImage;

    private void Awake()
    {
        ShellImage = GetComponent<Image>();
        controller.OnReloadChange += OnShellUpdated;
    }

    void OnShellUpdated(bool ShellState)
    {
        if(ShellState)
        {
            ShellImage.color = Color.white;
        }
        else
        {
            ShellImage.color = Color.grey;
        }
    }
}
