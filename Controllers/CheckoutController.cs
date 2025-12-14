using MercadoPago.Client.Preference;
using MercadoPago.Resource.Preference;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OutfixApi.Repositories;
using System.Security.Claims;

[Route("api/checkout")]
[ApiController]
[Authorize]
public class CheckoutController : ControllerBase
{
    private readonly ICartCollection _cartCollection;
    private readonly IProductCollection _productCollection;

    public CheckoutController(IConfiguration config)
    {
        MercadoPago.Config.MercadoPagoConfig.AccessToken =
            "TEST-7302928272959470-070518-c255a0a5951e0f5fe50e5d753e0ea0e6-227952207";

        _cartCollection = new CartCollection();
        _productCollection = new ProductCollection();
    }

    [HttpPost]
    public async Task<IActionResult> CreateCheckout()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var cart = await _cartCollection.GetByUserId(userId);
        if (cart == null || cart.Items.Count == 0)
            return BadRequest("Cart is empty");

        var preferenceItems = new List<PreferenceItemRequest>();

        foreach (var item in cart.Items)
        {
            var product = await _productCollection.GetProductById(item.ProductId);
            if (product == null)
                continue;

            preferenceItems.Add(new PreferenceItemRequest
            {
                Title = product.Title,
                Quantity = item.Quantity,
                UnitPrice = Convert.ToDecimal(product.Price),
                CurrencyId = "ARS"
            });
        }

        if (preferenceItems.Count == 0)
            return BadRequest("No valid products in cart");

        var request = new PreferenceRequest
        {
            Items = preferenceItems,
            BackUrls = new PreferenceBackUrlsRequest
            {
                Success = "https://outfix.com/checkout/success",
                Failure = "https://outfix.com/checkout/failure",
                Pending = "https://outfix.com/checkout/pending"
            },
            AutoReturn = "approved"
        };

        var client = new PreferenceClient();
        Preference preference = await client.CreateAsync(request);

        return Ok(new
        {
            initPoint = preference.InitPoint
        });
    }
}