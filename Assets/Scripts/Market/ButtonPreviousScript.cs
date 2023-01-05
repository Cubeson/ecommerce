using UnityEngine;
using Cysharp.Threading.Tasks;
public class ButtonPreviousScript : MonoBehaviour, Clickable
{
    StallScript stallScript;
    void Awake()
    {
        stallScript = transform.parent.GetComponent<StallScript>();
    }
    public void Click()
    {
        stallScript.Previous();
    }

    public void MouseOver()
    {

    }
}
