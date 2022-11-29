using Newtonsoft.Json;
using Shared.DTO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    void Start()
    {
        var data1 = new GenericResponseDTO() { Error = 0, Message = "Nice" };
        var json = JsonConvert.SerializeObject(data1);
        var data2 = JsonConvert.DeserializeObject<GenericResponseDTO>(json);
        Debug.Log(json);
        Debug.Log(data2.Error + " | " + data2.Message);
    }

}
