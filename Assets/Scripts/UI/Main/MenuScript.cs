using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuScript : MonoBehaviour
{
    [SerializeField] GameObject GameObjectUI;
    Camera cam;
    CameraRaycastScript cameraRaycastScript;
    PlayerInput playerInput;

    private void Awake()
    {
        cam = Camera.main;
        cameraRaycastScript= cam.gameObject.GetComponent<CameraRaycastScript>();
        playerInput = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>();
    }
    void Start()
    {
        OpenMenu();
        CloseMenu();
    }
    private bool isMenuOpen = false;
    private void OpenMenu()
    {
        cameraRaycastScript.enabled = false;
        isMenuOpen= true;
        //Camera.main.eventMask = 1 << 5; // UI only
        playerInput.enabled = false;
        GameObjectUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;

    }
    private void CloseMenu()
    {
        cameraRaycastScript.enabled = true;
        isMenuOpen = false;
        //Camera.main.eventMask = ~0; // ALl
        playerInput.enabled = true;
        GameObjectUI.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
    }
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.Tab))
        {
            if(isMenuOpen) CloseMenu();
            else OpenMenu();
        }
    }
}

