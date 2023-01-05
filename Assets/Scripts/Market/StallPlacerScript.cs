using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StallPlacerScript : MonoBehaviour
{
    [SerializeField] GameObject StallPrefab;
    [SerializeField] List<GameObject> StallSlots;
    public int GetSlotCount() { return StallSlots.Count; }
    void Awake()
    {
        StallSlots = FindSlots();
    }
    int index = 0;
    
    public GameObject PlaceNextStall()
    {
        if (index > StallSlots.Count) return null;
        return Instantiate(StallPrefab, StallSlots[index++].transform,false);
    }
    public void ClearAll()
    {
        foreach(var slot in StallSlots)
        {
            if(slot.transform.childCount != 0)
                Destroy(slot.transform.GetChild(0).gameObject);
        }
        index = 0;
    }

    private List<GameObject> FindSlots()
    {
        var capacity = transform.childCount;
        var list = new List<GameObject>(capacity);
        foreach(Transform child in transform)
        {
            if (child.CompareTag("StallSlot"))
                list.Add(child.gameObject);
        }
        return list;
    }
}
