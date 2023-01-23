using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeResolutionScript : MonoBehaviour
{
    public void SetResolution(int width, int heigth)
    {
        Screen.SetResolution(width, heigth, false);
    }
}
