using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ModelRotateScript : MonoBehaviour
{
    [SerializeField]Camera CamUI;
    [SerializeField]GameObject ModelView;
    [SerializeField]float rotationSpeed = 5f;
    [SerializeField]float movementSpeed = 100f;
    [SerializeField]float zoomSpeed = 100f;
    [SerializeField]float moveBoundsXMax = 100f;
    [SerializeField]float moveBoundsXMin = -100f;
    [SerializeField]float moveBoundsYMax = 125f;
    [SerializeField]float moveBoundsYMin = -15f;
    //[SerializeField]float moveBoundsZMax = 100f;
    //[SerializeField]float moveBoundsZMin = -100f;
    Renderer _renderer;
    Vector3 _origPosition;
    Quaternion _origRotation;
    bool rotatinng = false;
    bool moving = false;
    float MouseWheelDelta;

    private void Start()
    {
        _renderer= GetComponent<Renderer>();
        _origPosition = transform.position;
        _origRotation = transform.rotation;
    }
    public void ResetPosition()
    {
        ModelView.transform.localScale = Vector3.one;
        transform.position= _origPosition;
        transform.rotation= _origRotation;
    }
    void Update()
    {
        rotatinng = Input.GetMouseButton(1);
        moving = Input.GetMouseButton(0);
        //MouseWheelDelta = Input.mouseScrollDelta.y;
        MouseWheelDelta = Input.GetAxis("Mouse ScrollWheel");
        Zoom();
    }
    private void FixedUpdate()
    {
        Rotate();
        Move();
        
    }
    private void Move()
    {
        if (moving)
        {
            float deltaX = Input.GetAxis("Mouse X") * movementSpeed * Time.fixedDeltaTime;
            float deltaY = Input.GetAxis("Mouse Y") * movementSpeed * Time.fixedDeltaTime;

            transform.localPosition += new Vector3(deltaX, deltaY, 0);

            transform.localPosition = new Vector3(Mathf.Clamp(transform.localPosition.x, moveBoundsXMin, moveBoundsXMax),
                                                Mathf.Clamp(transform.localPosition.y, moveBoundsYMin, moveBoundsYMax),
                                                transform.localPosition.z);
        }
    }
    private void Rotate()
    {
        if (rotatinng)
        {
            float x = Input.GetAxis("Mouse X") * rotationSpeed * Time.fixedDeltaTime;
            float y = Input.GetAxis("Mouse Y") * rotationSpeed * Time.fixedDeltaTime;
            transform.RotateAround(_renderer.bounds.center, new Vector3(y, x, 0), rotationSpeed);
        }
    }
    private void Zoom()
    {
        float correctedMouseWheelDelta = MouseWheelDelta * zoomSpeed * Time.deltaTime;
        if (correctedMouseWheelDelta != 0)
        {
            //ModelView.transform.localScale += new Vector3(correctedMouseWheelDelta,correctedMouseWheelDelta,correctedMouseWheelDelta);
            //ModelView.transform.localScale = new Vector3(ModelView.transform.localScale.x)

            CamUI.fieldOfView += correctedMouseWheelDelta;
            CamUI.fieldOfView = Mathf.Clamp(CamUI.fieldOfView, 10, 50);


            //transform.localPosition += new Vector3(0, 0, correctedMouseWheelDelta);
            //transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y,
            //                                        Mathf.Clamp(transform.localPosition.z, moveBoundsZMin, moveBoundsZMax));
        }
    }


}
