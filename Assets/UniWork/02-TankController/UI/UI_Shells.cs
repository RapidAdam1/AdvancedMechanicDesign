using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UI_Shells : MonoBehaviour
{

    Image ShellImage;

    private void Awake()
    {
        ShellImage = GetComponent<Image>();

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
