using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Net.Http.Headers;
using System.Text.Json.Nodes;
using Newtonsoft.Json;
using Server.Models;
using System.Globalization;
using System.Dynamic;
using Newtonsoft.Json.Converters;

namespace Server.Services;

public class PayPalService
{
    private PayPalSettings _payPalSettings;
    private IHttpClientFactory _httpClientFactory;
    private string accessToken = null!;
    private DateTime expirationDate = DateTime.MinValue;

    private readonly string urlBase = "https://api-m.sandbox.paypal.com";
    private readonly string urlAuthenticate = "/v1/oauth2/token";
    private readonly string urlCreateOrder = "/v2/checkout/orders";
    private readonly string urlCapturePayment = "/v2/checkout/orders/{id}/capture";
    private readonly string urlGetOrderDetails = "/v2/checkout/orders/{id}";
    private string BasicAuth { get => $"{_payPalSettings.ClientId}:{_payPalSettings.ClientSecret}"; }

    public PayPalService([FromServices]PayPalSettings payPalSettings, [FromServices] IHttpClientFactory httpClientFactory) {
        _payPalSettings = payPalSettings;
        _httpClientFactory = httpClientFactory;
    }
    public async Task<HttpResponseMessage> CapturePayment(string orderId)
    {
        var token = await GetAuthToken();
        var client = _httpClientFactory.CreateClient();
        var urlCapture = urlCapturePayment.Replace("{id}", orderId);
        var req = new HttpRequestMessage(HttpMethod.Post, urlBase + urlCapture);
        req.Content = new StringContent("");
        req.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return await client.SendAsync(req);
    }
    public async Task<HttpResponseMessage> CreateOrder(Order order)
    {
        var token = await GetAuthToken();

        var client = _httpClientFactory.CreateClient();
        var req = new HttpRequestMessage(HttpMethod.Post, urlBase + urlCreateOrder);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        req.Content = JsonContent.Create(MakePayPalOrder(order));
        return await client.SendAsync(req);
    }
    public async Task<HttpResponseMessage> GetOrderDetails(string payPalOrderId)
    {
        var token = await GetAuthToken();
        var client = _httpClientFactory.CreateClient();
        var urlDetails = urlGetOrderDetails.Replace("{id}", payPalOrderId);
        var req = new HttpRequestMessage(HttpMethod.Get, urlBase + urlDetails);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //req.Content = JsonContent.Create(MakePayPalOrder(order));
        return await client.SendAsync(req);

    }
    public async Task<string> GetAuthToken()
    {
        if(accessToken == null || DateTime.Now >= expirationDate)
        {
            return await GetNewToken();
        }
        return accessToken;

    }

    private static PPJson.Models.CreateOrderRequest MakePayPalOrder(Order order)
    {
        var total = order.OrderItems.Aggregate(0, (decimal total, OrderItem next) => total + (next.ItemPrice * next.Quantity));
        
        PPJson.ModelComponents.Items[] items = order.OrderItems.Select(o => new PPJson.ModelComponents.Items
        {
            name = o.ProductName,
            quantity = o.Quantity,
            unit_amount = new PPJson.ModelComponents.UnitAmount { value = o.ItemPrice.ToString(CultureInfo.InvariantCulture) },
        }).ToArray();
        var purchase_units = new PPJson.ModelComponents.PurchaseUnits[1] { new PPJson.ModelComponents.PurchaseUnits 
        {
            reference_id = order.Id.ToString(),
            items = items,
            amount = new PPJson.ModelComponents.Amount
                {
                    value = total.ToString(CultureInfo.InvariantCulture),
                    breakdown = new PPJson.ModelComponents.Breakdown
                    {
                        item_total = new PPJson.ModelComponents.ItemTotal
                        {
                            value = total.ToString(CultureInfo.InvariantCulture),
                        }
                    },
                }
        }};
        PPJson.Models.CreateOrderRequest ppOrder = new()
        {
            purchase_units = purchase_units,
        };
        return ppOrder;
    }

    private async Task<string> GetNewToken()
    {
        var client = _httpClientFactory.CreateClient();
        var req = new HttpRequestMessage(HttpMethod.Post, urlBase + urlAuthenticate);
        var byteArray = new UTF8Encoding().GetBytes(BasicAuth);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
        var formData = new List<KeyValuePair<string, string>>
        {
            new("grant_type", "client_credentials")
        };
        req.Content = new FormUrlEncodedContent(formData);

        var resp = await client.SendAsync(req);
        resp.EnsureSuccessStatusCode();
        var text = await resp.Content.ReadAsStringAsync();
        var authResponse = JsonConvert.DeserializeObject<PPJson.Auth.AuthResponse>(text);
        var seconds = authResponse.expires_in;
        expirationDate = DateTime.Now.AddSeconds(seconds);
        accessToken = authResponse.access_token;
        return authResponse.access_token;
    }
}
