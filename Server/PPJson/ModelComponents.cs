namespace Server.PPJson.ModelComponents;
#pragma warning disable IDE1006
//#pragma warning disable C
public class PurchaseUnits
{
    public Amount amount { get; set; }

    public Items[] items { get; set; }
    public string reference_id { get; set; }
}
public class Amount
{
    public string currency_code { get; set; } = "PLN";
    public string value { get; set; }
    public Breakdown breakdown { get; set; }

}
public class Breakdown
{
    public ItemTotal item_total { get; set; }
}
public class ItemTotal
{
    public string currency_code { get; set; } = "PLN";
    public string value { get; set; }
}
public class Items
{
    public string name { get; set; }
    public int quantity { get; set; }
    public UnitAmount unit_amount { get; set; }
}
public class UnitAmount
{
    public string currency_code { get; set; } = "PLN";
    public string value { get; set; }
}

public class Link
{
    public string href { get; set; }
    public string rel { get; set; }
    public string method { get; set; }
}
public class Payer
{
    public Address address { get; set; }
    public Name name { get; set; }
    public string payer_id { get; set; }
}
public class Address
{
    public string country_code { get; set; } = "PLN";
}
public class Name
{
    public string given_name { get; set; }
    public string surname { get; set; }
}
public class PaymentSource
{

}
#pragma warning restore IDE1006