using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemImageScript : MonoBehaviour
{
    private Material mat;
    // Start is called before the first frame update
    void Start()
    {
        mat = GetComponent<MeshRenderer>().material;

    }
    private float timer = 1f;
    // Update is called once per frame
    void Update()
    {

    }
}
