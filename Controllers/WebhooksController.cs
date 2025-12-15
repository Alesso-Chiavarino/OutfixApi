// MercadoPagoWebhookController.cs
using MercadoPago.Client.Payment;
using Microsoft.AspNetCore.Mvc;
using OutfixApi.Repositories;
using System.Text.Json;

[Route("api/webhooks/mercadopago")]
[ApiController]
public class MercadoPagoWebhookController : ControllerBase
{
    private readonly IOrderCollection _orderCollection;
    private readonly IProductCollection _productCollection;
    private readonly ICartCollection _cartCollection;

    public MercadoPagoWebhookController()
    {
        _orderCollection = new OrderCollection();
        _productCollection = new ProductCollection();
        _cartCollection = new CartCollection();
    }

    [HttpPost]
    public async Task<IActionResult> Handle([FromBody] JsonElement payload)
    {
        if (!payload.TryGetProperty("data", out var data))
            return Ok();

        if (!data.TryGetProperty("id", out var paymentIdElement))
            return Ok();

        var paymentIdStr = paymentIdElement.GetString();
        if (string.IsNullOrEmpty(paymentIdStr))
            return Ok();

        long paymentId = long.Parse(paymentIdStr);

        var paymentClient = new PaymentClient();
        var payment = await paymentClient.GetAsync(paymentId);

        var orderId = payment.ExternalReference;
        if (string.IsNullOrEmpty(orderId))
            return Ok();

        var order = await _orderCollection.GetById(orderId);
        if (order == null)
            return Ok();

        // ✅ Actualizar estado de la orden
        order.Status = payment.Status;
        order.PaymentId = payment.Id.ToString();
        order.UpdatedAt = DateTime.UtcNow;

        // ✅ SOLO si el pago fue aprobado
        if (payment.Status == "approved")
        {
            // 🔻 Descontar stock
            foreach (var item in order.Items)
            {
                await _productCollection.DecreaseStock(
                    item.ProductId,
                    item.VariantId,
                    item.Quantity
                );
            }

            // 🛒 Vaciar carrito del usuario
            await _cartCollection.ClearCart(order.UserId);
        }

        await _orderCollection.Update(order);

        return Ok();
    }
}