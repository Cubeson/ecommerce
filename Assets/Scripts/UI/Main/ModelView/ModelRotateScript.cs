using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelRotateScript : MonoBehaviour
{
    [SerializeField]
    Camera CamUI;
    [SerializeField]
    float rotationSpeed = 1f;
    [SerializeField]
    bool dragging = false;
    Renderer _renderer;
    Vector3 _orig_pos;
    Quaternion _orig_rot;
    private void Start()
    {
        _renderer= GetComponent<Renderer>();
        _orig_pos = transform.position;
        _orig_rot = transform.rotation;
    }
    public void Reset()
    {
        transform.position= _orig_pos;
        transform.rotation= _orig_rot;
    }
    void Update()
    {
        dragging = Input.GetMouseButton(0);
    }
    private void FixedUpdate()
    {
        if(dragging)
        {
            float x = Input.GetAxis("Mouse X") * rotationSpeed * Time.fixedDeltaTime;
            float y = Input.GetAxis("Mouse Y") * rotationSpeed * Time.fixedDeltaTime;
            transform.RotateAround(_renderer.bounds.center, new Vector3(y,x,0), rotationSpeed);

        }
    }


}
