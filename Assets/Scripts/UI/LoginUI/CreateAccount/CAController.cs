using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class CAController : MonoBehaviour
{
    [SerializeField] List<GameObject> ViewList;
    [SerializeField] Button ButtonBack;
    int index = 0;
    public void Back()
    {
        if (index > 0) {
            ViewList[index].SetActive(false);
            index--;
            ViewList[index].SetActive(true);
        }
        else
        {
            SceneManager.LoadScene("LoginScene", LoadSceneMode.Single);
        }
    }
    public void Next()
    { 
        if(index < ViewList.Count - 1) {
            CASaveData saveData = ViewList[index].GetComponent<CASaveData>();
            //if (!viewFunctions.allowNext()) return;
            saveData.saveData();
            ViewList[index].SetActive(false);
            index++;
            ViewList[index].SetActive(true);
        }
    }
    void Start()
    {
        if (ViewList.Count == 0) {
            throw new MissingReferenceException("CAController ViewList needs items added in editor");
        }
        ButtonBack.onClick.AddListener(Back);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
