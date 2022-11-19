using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class RevokeAllSessionsScript : MonoBehaviour
{
    void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(() =>
        {

        });
    }
}
