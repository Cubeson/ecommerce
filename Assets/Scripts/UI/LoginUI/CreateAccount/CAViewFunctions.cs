using UnityEngine;
public class CAViewFunctions : MonoBehaviour
{
    public delegate bool AllowNext();
    public AllowNext allowNext;
    public delegate void SaveData();
    public SaveData saveData;
}
