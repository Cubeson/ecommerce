using UnityEngine;
using UnityEngine.UI;

public class RequestSent : MonoBehaviour
{
    [SerializeField] Button ButtonOK;
    void Start()
    {
        ButtonOK.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
        });
    }
}
