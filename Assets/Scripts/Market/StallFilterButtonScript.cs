using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StallFilterButtonScript : MonoBehaviour, Clickable
{
    [SerializeField] GameObject Stall;
    private StallScript stallScript;
    private void Awake()
    {
        stallScript = Stall.GetComponent<StallScript>();
    }
    public void Click()
    {
        ProductFilterOptionsScript.Instance.OpenMenu(stallScript);
    }

    public void MouseOver(){}
}
