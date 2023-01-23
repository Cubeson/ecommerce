using Cysharp.Threading.Tasks;
using Shared.DTO;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ProductSlotScript : MonoBehaviour, Clickable
{
    [SerializeField] GameObject MainImage;
    [SerializeField] GameObject TextTitleGO;
    [SerializeField] GameObject TextPriceGO;
    TextMeshPro TextTitle;
    TextMeshPro TextPrice;
    MeshRenderer MeshRendererMainImage;
    private ProductDTO productDTO;

    public void SetMainImage(Texture2D tex)
    {
        MeshRendererMainImage.material.mainTexture = tex;
    }

    void Start()
    {
        MeshRendererMainImage = MainImage.GetComponent<MeshRenderer>();
        TextTitle = TextTitleGO.GetComponent<TextMeshPro>();
        TextPrice = TextPriceGO.GetComponent<TextMeshPro>();
    }
    //private UniTask coolTask;
    public void Click()
    {
        ProductDetailsScript.Instance.OpenMenu(productDTO);

    }
    private UniTask MouseOverTask;
    public void MouseOver()
    {
        if (!(MouseOverTask.Status == UniTaskStatus.Succeeded)) return;
        MouseOverTask = UniTask.Create(async () =>
        {
            TextTitleGO.SetActive(true);
            TextPriceGO.SetActive(true);
            await UniTask.Delay(200);
            TextTitleGO.SetActive(false);
            TextPriceGO.SetActive(false);
            return UniTask.CompletedTask;

        });
    }


    void Update()
    {
        
    }
    public void SetProduct(ProductDTO productDTO)
    {
        this.productDTO = productDTO;
        TextTitle.text = this.productDTO.Title;
        TextPrice.text = this.productDTO.Price.ToString();
    }
    public async UniTask DownloadThumbnailAndSet()
    {
        UnityWebRequest req = Network.ProductApi.GetThumbnail(productDTO.Id);
        UnityWebRequest resp = null;
        try
        {
            resp = await req.SendWebRequest().ToUniTask();
            var texture = ((DownloadHandlerTexture)resp.downloadHandler).texture;
            SetMainImage(texture);
        }
        catch (UnityWebRequestException)
        {
            throw;
        }
        finally
        {
            req?.Dispose();
            resp?.Dispose();
        }

    }
}
