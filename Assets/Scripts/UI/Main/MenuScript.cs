using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuScript : MonoBehaviour
{
    [SerializeField] GameObject GameObjectUI;
    PlayerInput playerInput;

    private void Awake()
    {
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
        isMenuOpen= true;
        Camera.main.eventMask = 1 << 5; // UI only
        playerInput.enabled = false;
        GameObjectUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;

    }
    private void CloseMenu()
    {
        isMenuOpen = false;
        Camera.main.eventMask = ~0; // ALl
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
    void Update1()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (Camera.main.eventMask == 0)
            {
                //Camera.main.eventMask = LayerMask.NameToLayer("UI");
                Camera.main.eventMask = 5;
            }
            else Camera.main.eventMask = 0;

            playerInput.enabled = !playerInput.enabled;
            GameObjectUI.SetActive(!GameObjectUI.activeSelf);
        }
    }
}

