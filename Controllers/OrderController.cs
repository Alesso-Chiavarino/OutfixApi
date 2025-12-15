using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OutfixApi.Repositories;
using System.Security.Claims;

namespace OutfixApi.Controllers;

[Route("api/orders")]
[ApiController]
[Authorize]
public class OrderController : ControllerBase
{
    private readonly IOrderCollection _orderCollection = new OrderCollection();

    // GET api/orders/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrder(string id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var order = await _orderCollection.GetById(id);

        if (order == null || order.UserId != userId)
            return NotFound();

        return Ok(order);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetMyOrders()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var orders = await _orderCollection.GetByUserId(userId);
        return Ok(orders);
    }
}