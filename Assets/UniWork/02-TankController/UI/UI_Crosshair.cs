using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UI_Crosshair : MonoBehaviour
{
    RectTransform UI_ElementPos;
    [SerializeField] Image CrosshairImage;
    [SerializeField] Rigidbody m_rb;
    [SerializeField] Camera cam;
    [SerializeField] Transform Barrel;
    private void Awake()
    {
        UI_ElementPos = transform.GetComponent<RectTransform>();
        CrosshairImage = GetComponentInChildren<Image>();
    }

    
    private void Update()
    {
        //Speed Scaling
        float Scale = Mathf.Clamp((m_rb.velocity.magnitude / 24f) + 1,1, 2);
        CrosshairImage.transform.localScale = new Vector2(Scale, Scale);

        //Screen Position
        Ray DrawRay = new Ray(Barrel.position, Barrel.forward);
        Vector3 Location = Barrel.position + (Barrel.forward * 50);
        if(Physics.Raycast(DrawRay, out RaycastHit Hit, 50))
        {
            Location = Hit.point;
        }
        transform.position = RectTransformUtility.WorldToScreenPoint(Camera.main, Location);
    }

    private void OnDrawGizmos()
    {
        Ray DrawRay = new Ray(Barrel.position, Barrel.forward);
        Debug.DrawLine(Barrel.position, Barrel.position + (Barrel.forward * 50));
        Vector3 Location = Barrel.position + (Barrel.forward * 50);
        if (Physics.Raycast(DrawRay, out RaycastHit Hit, 50))
        {
            Location = Hit.point;
        }
        Gizmos.DrawSphere(Location, 1);
    }
}
