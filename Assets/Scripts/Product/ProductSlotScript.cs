using Cysharp.Threading.Tasks;
using Shared.DTO;
using System;
using UnityEngine;
using UnityEngine.Networking;

public class ProductSlotScript : MonoBehaviour, Clickable
{
    [SerializeField] GameObject MainImage;
    MeshRenderer MeshRendererMainImage;
    private ProductDTO ProductDTO;

    public void SetMainImage(Texture2D tex)
    {
        MeshRendererMainImage.material.mainTexture = tex;
    }

    void Start()
    {
        MeshRendererMainImage = MainImage.GetComponent<MeshRenderer>();
    }
    private UniTask coolTask;
    public void Click()
    {
        if (!(coolTask.Status == UniTaskStatus.Succeeded)) return;
        coolTask = UniTask.Create(async () =>
        {
            MeshRendererMainImage.material.color = Color.red;
            await UniTask.Delay(2500);
            MeshRendererMainImage.material.color = Color.white;
            return UniTask.CompletedTask;
        });
    }
    

    void Update()
    {
        
    }
    public void SetProduct(ProductDTO productDTO)
    {
        ProductDTO= productDTO;
    }
    public async void DownloadThumbnailAndSet()
    {
        var req = Network.ProductApi.GetThumbnail(ProductDTO.Id);
        var resp = await req.SendWebRequest().ToUniTask();
        var texture = ((DownloadHandlerTexture)resp.downloadHandler).texture;
        SetMainImage(texture);
    }
}
