using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TexturesManager : MonoBehaviour
{
    private static TexturesManager tm;
    public static TexturesManager GetInstance { get { return tm; } }
    private void Awake()
    {
        tm = this;
    }
    private List<Texture2D> texture2Ds = new List<Texture2D>();
    public void Add(Texture2D tx2d)
    {
        texture2Ds.Add(tx2d);
    }
    public void Add(List<Texture2D> texture2Ds)
    {
        this.texture2Ds.AddRange(texture2Ds);
    }
    public void Add(Texture2D[] texture2Ds)
    {
        this.texture2Ds.AddRange(texture2Ds);
    }
    public Texture2D Get(int idx)
    {
        if(i >= texture2Ds.Count) throw new System.IndexOutOfRangeException(i.ToString());
        return texture2Ds[idx];
    }
    public void Clear()
    {
        texture2Ds.Clear();
    }
    private int i = 0;
    public Texture2D getNext()
    {
        if(texture2Ds.Count == 0) throw new System.IndexOutOfRangeException(i.ToString());
        if ( i >= texture2Ds.Count) i = 0;

        return texture2Ds[i++];
    }
}
