namespace Server.PPJson.Models;
using Server.PPJson.ModelComponents;
#pragma warning disable IDE1006
public class CreateOrderRequest
{
    public string intent { get; set; } = "CAPTURE";
    public PurchaseUnits[] purchase_units { get; set; }
}
public class CreateOrderResponse
{
    public string id { get; set; }
    public string status { get; set; }
    public Link[] links { get; set; }
}
#pragma warning restore IDE1006