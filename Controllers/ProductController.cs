using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OutfixApi.Models;
using OutfixApi.Repositories;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace OutfixApi.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductController : Controller
    {
        private IProductCollection db = new ProductCollection();
        private IUserCollection userCollection = new UserCollection();

        [HttpGet]
        public async Task<ActionResult> GetAllProducts(
            [FromQuery] int limit,
            [FromQuery] string? category,
            [FromQuery] int page = 1
            )
        {
            var products = await db.GetAllProducts(limit > 0 ? limit : 20, category, page);
            var response = new
            {
                products,
                page
            };
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetProducDetails(string id)
        {
            var product = await db.GetProductById(id);
            return Ok(product);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> CreateProduct([FromBody] Product product)
        {
            if (product == null)
            {
                return BadRequest();
            }

            if (string.IsNullOrEmpty(product.Title))
            {
                ModelState.AddModelError("message", "The product Title is empty");
                return BadRequest(ModelState);
            }

            if (product.Images.Length == 0)
            {
                ModelState.AddModelError("message", "The product Images are empty");
                return BadRequest(ModelState);
            }

            if (string.IsNullOrEmpty(product.Owner))
            {
                ModelState.AddModelError("message", "The product Owner is empty");
                return BadRequest(ModelState);
            }

            var user = await userCollection.GetUserByEmail(product.Owner);

            if (user == null)
            {
                ModelState.AddModelError("message", $"The user {product.Owner} not exists");
                return NotFound(ModelState);
            }

            if (user.Role != "admin" && user.Role != "seller")
            {
                ModelState.AddModelError("message", $"The user {product.Owner} must be a seller to create products.");
                return Unauthorized(ModelState);
            }

            await db.InsertProduct(product);

            return Created("Created", true);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateProduct([FromBody] Product product, string id)
        {
            if (product == null)
            {
                return BadRequest();
            }

            if (product.Title == string.Empty)
            {
                ModelState.AddModelError("Name", "The product Name is empty");
            }

            product.Id = id;
            await db.UpdateProduct(product);

            return Created("Created", true);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProduc(string id)
        {

            await db.DeleteProduct(id);

            return NoContent();
        }
    }
}
