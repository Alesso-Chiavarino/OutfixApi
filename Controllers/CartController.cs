using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OutfixApi.Models;
using OutfixApi.Repositories;
using System.Security.Claims;

namespace OutfixApi.Controllers
{
    [Route("api/cart")]
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartCollection cartCollection = new CartCollection();
        private readonly IProductCollection productCollection = new ProductCollection();

        // GET api/cart (POPULADO)
        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var cart = await cartCollection.GetByUserId(userId)
                       ?? await cartCollection.Create(userId);

            var populatedItems = new List<object>();

            foreach (var item in cart.Items)
            {
                var product = await productCollection.GetProductById(item.ProductId);
                if (product == null)
                    continue;

                // VariantId = "M_693edcolorid"
                var parts = item.VariantId.Split('_');
                var size = parts[0];
                var colorId = parts.Length > 1 ? parts[1] : "";

                populatedItems.Add(new
                {
                    item.ProductId,
                    item.VariantId,
                    item.Quantity,
                    Product = new
                    {
                        product.Id,
                        product.Title,
                        product.Price,
                        product.Images,
                        product.Category
                    },
                    Variant = new
                    {
                        Size = size,
                        ColorId = colorId
                    }
                });
            }

            return Ok(new
            {
                cart.Id,
                cart.UserId,
                Items = populatedItems,
                cart.UpdatedAt
            });
        }

        // POST api/cart/items
        [HttpPost("items")]
        public async Task<IActionResult> AddItem([FromBody] CartItem item)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            await cartCollection.AddItem(userId, item);
            return Ok();
        }

        // PUT api/cart/items
        [HttpPut("items")]
        public async Task<IActionResult> UpdateItemQuantity(
            [FromBody] CartItem item
        )
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            await cartCollection.UpdateItemQuantity(
                userId,
                item.ProductId,
                item.VariantId,
                item.Quantity
            );

            return Ok();
        }

        // DELETE api/cart/items
        [HttpDelete("items")]
        public async Task<IActionResult> RemoveItem(
            [FromQuery] string productId,
            [FromQuery] string variantId
        )
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            await cartCollection.RemoveItem(userId, productId, variantId);
            return Ok();
        }

        // DELETE api/cart
        [HttpDelete]
        public async Task<IActionResult> ClearCart()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            await cartCollection.ClearCart(userId);
            return NoContent();
        }
    }
}