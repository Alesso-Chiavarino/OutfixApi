using MercadoPago.Client.Preference;
using MercadoPago.Resource.Preference;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OutfixApi.Models;
using OutfixApi.Repositories;
using System.Security.Claims;
using System.Collections.Concurrent;

[Route("api/checkout")]
[ApiController]
[Authorize]
public class CheckoutController : ControllerBase
{
    private readonly ICartCollection _cartCollection;
    private readonly IProductCollection _productCollection;
    private readonly IOrderCollection _orderCollection;

    // 🔒 Cooldown en memoria (UserId → DateTime)
    private static readonly ConcurrentDictionary<string, DateTime> _checkoutLocks = new();

    public CheckoutController(IConfiguration config)
    {
        MercadoPago.Config.MercadoPagoConfig.AccessToken =
            config["settings:mercadoPagoAccessToken"];

        _cartCollection = new CartCollection();
        _productCollection = new ProductCollection();
        _orderCollection = new OrderCollection();
    }

    [HttpPost]
    public async Task<IActionResult> CreateCheckout()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        // ⏱️ COOLDOWN 1 MINUTO
        if (_checkoutLocks.TryGetValue(userId, out var lastTime))
        {
            if (DateTime.UtcNow - lastTime < TimeSpan.FromMinutes(1))
            {
                return BadRequest("Ya existe un checkout en proceso. Esperá un minuto.");
            }
        }

        // 🔐 Bloqueamos
        _checkoutLocks[userId] = DateTime.UtcNow;

        var cart = await _cartCollection.GetByUserId(userId);
        if (cart == null || cart.Items.Count == 0)
            return BadRequest("Cart is empty");

        var orderItems = new List<OrderItem>();
        var preferenceItems = new List<PreferenceItemRequest>();
        decimal total = 0;

        foreach (var item in cart.Items)
        {
            var product = await _productCollection.GetProductById(item.ProductId);
            if (product == null)
                continue;

            orderItems.Add(new OrderItem
            {
                ProductId = item.ProductId,
                VariantId = item.VariantId,
                Quantity = item.Quantity,
                UnitPrice = Convert.ToDecimal(product.Price),
                Title = product.Title,
                Image = product.Images[0]
            });

            preferenceItems.Add(new PreferenceItemRequest
            {
                Title = product.Title,
                Quantity = item.Quantity,
                UnitPrice = Convert.ToDecimal(product.Price),
                CurrencyId = "ARS"
            });

            total += Convert.ToDecimal(product.Price) * item.Quantity;
        }

        if (orderItems.Count == 0)
            return BadRequest("No valid products in cart");

        // ✅ SE CREA UNA SOLA ORDEN
        var order = new Order
        {
            UserId = userId,
            Status = "pending",
            Items = orderItems,
            Total = total,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _orderCollection.Create(order);

        var request = new PreferenceRequest
        {
            Items = preferenceItems,
            ExternalReference = order.Id, // 🔗 clave para webhook
            BackUrls = new PreferenceBackUrlsRequest
            {
                Success = "https://outfix.vercel.app/checkout/success",
                Failure = "https://outfix.vercel.app/checkout/failure",
                Pending = "https://outfix.vercel.app/checkout/pending"
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