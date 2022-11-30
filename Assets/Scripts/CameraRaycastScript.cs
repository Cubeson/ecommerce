using UnityEngine;
[RequireComponent(typeof(Camera))]
public class CameraRaycastScript : MonoBehaviour
{
    
    Camera cam;
    void Start()
    {
        cam = GetComponent<Camera>();
    }
    void Update()
    {
        RaycastHit HitInfo;
        Physics.Raycast(cam.transform.position, cam.transform.forward, out HitInfo, 100.0f);
        if (HitInfo.collider == null) return;
        Clickable clickable;  
        HitInfo.transform.gameObject.TryGetComponent<Clickable>(out clickable);
        if (Input.GetMouseButtonDown(0))
        {
            
            clickable?.Click();
        }
        clickable?.MouseOver();


    }
}
