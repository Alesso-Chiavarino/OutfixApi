using Microsoft.AspNetCore.Mvc;
using OutfixApi.Models;
using OutfixApi.Repositories;

namespace OutfixApi.Controllers
{
    [Route("api/[controller]")]
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

        [HttpPost]
        public async Task<ActionResult> CreateProduct([FromBody] Product product)
        {
            if(product == null)
            {
                return BadRequest();
            }

            if(product.Name == string.Empty)
            {
                ModelState.AddModelError("Name", "The product Name is empty");
            }

            await db.InsertProduct(product);

            return Created("Created", true);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateProduct([FromBody] Product product, string id)
        {
            if (product == null)
            {
                return BadRequest();
            }

            if (product.Name == string.Empty)
            {
                ModelState.AddModelError("Name", "The product Name is empty");
            }

            product.Id = id;
            await db.UpdateProduct(product);

            return Created("Created", true);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProduc(string id) {
        
            await db.DeleteProduct(id);

            return NoContent();
        }
    }
}
