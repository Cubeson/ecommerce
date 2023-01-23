using Cysharp.Threading.Tasks;
using UnityEngine;

public class ButtonNextScript : MonoBehaviour, Clickable
{
    StallScript stallScript;
    void Awake()
    {
        stallScript = transform.parent.GetComponent<StallScript>();
    }
    public void Click()
    {
        stallScript.Next();
    }

    public void MouseOver()
    {
        
    }
}
