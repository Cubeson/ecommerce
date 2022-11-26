using Cysharp.Threading.Tasks;
using UnityEngine;

public class ProductSlotScript : MonoBehaviour, Clickable
{
    [SerializeField] GameObject MainImage;
    MeshRenderer MeshRendererMainImage;

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
}
