using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    private static PrefabManager pm;
    public static PrefabManager GetInstance { get { return pm; } }
    private void Awake()
    {
        pm = this;
    }
    public GameObject Plane;
}
