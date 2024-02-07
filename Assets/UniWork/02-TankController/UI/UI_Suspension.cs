using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UI_Suspension : MonoBehaviour
{
    [SerializeField] Suspension TrackedSuspension;
    Image SuspensionImage;

    private void Awake()
    {
        SuspensionImage = GetComponent<Image>();
        SuspensionImage.color = Color.red;
        TrackedSuspension.OnGroundedChanged += OnGroundedChanged;
    }

    void OnGroundedChanged(bool Grounded)
    {
        Color WheelSuspensionColor;
        if (Grounded)
            WheelSuspensionColor = Color.green;
        else
            WheelSuspensionColor = Color.red;

        SuspensionImage.color = WheelSuspensionColor;
    }
}
