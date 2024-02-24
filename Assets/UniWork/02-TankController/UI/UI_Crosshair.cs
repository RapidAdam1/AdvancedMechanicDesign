using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UI_Crosshair : MonoBehaviour
{
    RectTransform Position;

    [SerializeField] Image CrosshairImage;
    [SerializeField] UI_Speed SpeedCounter;
    [SerializeField] Camera cam;
    [SerializeField] Transform Barrel;
    private void Awake()
    {
        Position = transform.GetComponent<RectTransform>();
        CrosshairImage = GetComponentInChildren<Image>();
    }

    private void Update()
    {
        float Scale = Mathf.Lerp(1, 2, ((float)SpeedCounter.Speed / 24) + 1);
        Debug.Log(Scale);
        //Debug.DrawLine(Barrel.position, Barrel.position + (Barrel.forward * 50));
        CrosshairImage.transform.localScale = new Vector2 (Scale,Scale);
        Vector3 Location = Barrel.position+(Barrel.forward * 50);
        Position.anchoredPosition = cam.WorldToViewportPoint(Location);
    }
}
