using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CAController : MonoBehaviour
{
    [SerializeField] List<GameObject> ViewList;
    [SerializeField] Button ButtonBack;
    [SerializeField] Button ButtonNext;

    Text TextButtonNext;
    int index = 0;
    void ChangeButtonText()
    {
        if(index == 2) // Password
        {
            TextButtonNext.text = "Create";
        }
        else
        {
            TextButtonNext.text = "Next";
        }
    }
    void Back()
    {
        if(ViewList.Count> 1) ButtonNext.gameObject.SetActive(true);
        if (index > 0) {
            ViewList[index].SetActive(false);
            index--;
            ViewList[index].SetActive(true);
        }
        else
        {
            SceneManager.LoadScene("LoginScene", LoadSceneMode.Single);
            //ViewList[index].SetActive(false);
            //gameObject.SetActive(false);
            //LoginView.SetActive(true);
        }
    }
    void Next()
    { 
        if(index < ViewList.Count - 1) {
            CAViewFunctions viewFunctions = ViewList[index].GetComponent<CAViewFunctions>();
            if (!viewFunctions.allowNext()) return;
            viewFunctions.saveData();
            ViewList[index].SetActive(false);
            index++;
            ViewList[index].SetActive(true);
            ChangeButtonText();
            if(index == ViewList.Count - 1)
            {
                //ButtonNext.gameObject.SetActive(false);
            }

        }
    }
    void Start()
    {
        TextButtonNext = ButtonNext.GetComponentInChildren<Text>();
        if (ViewList.Count == 0) {
            throw new MissingReferenceException("CAController ViewList needs items added in editor");
        }
        ButtonNext.onClick.AddListener(Next);
        ButtonBack.onClick.AddListener(Back);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
