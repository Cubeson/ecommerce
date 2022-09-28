using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentModel : MonoBehaviour
{
    public static GameObject GameObject { get; private set; }
    public void Start()
    {
        GameObject = this.gameObject;
    }
}
