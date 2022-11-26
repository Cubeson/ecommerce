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
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit HitInfo;
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out HitInfo, 100.0f))
            {
                Clickable clickable;
                HitInfo.transform.gameObject.TryGetComponent<Clickable>(out clickable);
                clickable?.Click();
            }
        }

    }
}
