using UnityEngine;
public class DisabledMenuInitScript : MonoBehaviour
{
    [SerializeField] GameObject ProductDetails;
    [SerializeField] GameObject CartMenu;
    [SerializeField] GameObject ProductFilter;
    void Start()
    {
        ProductDetails.GetComponent<ProductDetailsScript>().Init();
        CartMenu.GetComponent<CartMenuScript>().Init();
        ProductFilter.GetComponent<ProductFilterOptionsScript>().Init();
    }

}
