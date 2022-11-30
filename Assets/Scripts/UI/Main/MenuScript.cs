using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuScript : MonoBehaviour
{
    static MenuScript instance;
    public static MenuScript Instance { get { return instance; } }

    [SerializeField] GameObject GameObjectUI;
    private Stack<GameObject> MenuStack = new();
    Camera cam;
    CameraRaycastScript cameraRaycastScript;
    PlayerInput playerInput;

    private void Awake()
    {
        instance = this;
        cam = Camera.main;
        cameraRaycastScript= cam.gameObject.GetComponent<CameraRaycastScript>();
        playerInput = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>();
    }
    private void OpenMenu()
    {
        cameraRaycastScript.enabled = false;
        playerInput.enabled = false;
        Cursor.lockState = CursorLockMode.None;

    }
    private void CloseMenu()
    {
        cameraRaycastScript.enabled = true;
        playerInput.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void PushMenu(GameObject menuGO)
    {
        if(MenuStack.Count> 0)
            MenuStack.Peek().SetActive(false);
        MenuStack.Push(menuGO);
        menuGO.SetActive(true);
        if (MenuStack.Count > 0)
        {
            OpenMenu();
        }
    }
    public void PopMenu()
    {
        if (MenuStack.Count == 0) return;
        var go = MenuStack.Pop();
        go.SetActive(false);
        if (MenuStack.Count > 0)
            MenuStack.Peek().SetActive(true);
        if (MenuStack.Count == 0)
        {
            CloseMenu();
        }
    }
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.Tab))
        {
            if(MenuStack.Count == 0)
            {
                PushMenu(GameObjectUI);
            }
            else
            {
                PopMenu();
            }
        }
    }
}

