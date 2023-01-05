using Assets.Scripts.Network;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainUIScript : MonoBehaviour
{
    [SerializeField] GameObject CartGO;
    [SerializeField] GameObject CategoryFilters;
    [SerializeField] Button ButtonOpenCart;
    [SerializeField] Button ButtonFilterCategories;
    [SerializeField] Button ButtonLogout;
    [SerializeField] Button ButtonRevokeSessions;
    void Start()
    {
        ButtonOpenCart.onClick.AddListener(() =>
        {
            MenuScript.Instance.PushMenu(CartGO);
        });
        ButtonFilterCategories.onClick.AddListener(() =>
        {
            MenuScript.Instance.PushMenu(CategoryFilters);
        });
        ButtonLogout.onClick.AddListener(() => {
            var req = Network.TokenApi.RevokeToken(CurrentSession.GetInstance().GetToken());
            req.SendWebRequest();
            SceneManager.LoadScene("LoginScene", LoadSceneMode.Single);
        });
    }

}
