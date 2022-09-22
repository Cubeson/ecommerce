using Network.DTO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GalleryMainScript : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> slots = new List<GameObject> (18);

    void Start()
    {
        foreach (Transform child in transform)
        {
            if (child.tag.Equals("GallerySlot"))
            {
                slots.Add(child.gameObject);
            }
        }

    }
    private void OnEnable()
    {
        
    }

    private void fun()
    {
    
    }
    private void SetSlotName(GameObject slot, string name)
    {
        slot.transform.Find("Name").gameObject.GetComponent<Text>().text = name;

    }
}
