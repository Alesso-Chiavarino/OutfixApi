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

        [HttpGet]
        public async Task<ActionResult> GetAllProducts() 
        {
            return Ok(await db.GetAllProducts());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetProducDetails(string id)
        {
            return Ok(await db.GetProductById(id));
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> CreateProduct([FromBody] Product product)
        {
            if(product == null)
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
        public async Task<ActionResult> DeleteProduc(string id) {
        
            await db.DeleteProduct(id);

            return NoContent();
        }
    }
}
