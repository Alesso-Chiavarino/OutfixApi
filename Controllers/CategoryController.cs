using Microsoft.AspNetCore.Mvc;
using OutfixApi.Models;
using OutfixApi.Repositories;

namespace OutfixApi.Controllers
{
    [Route("api/categories")]
    [ApiController]
    public class CategoryController : Controller
    {
        private ICategoryCollection db = new CategoryCollection();

        [HttpGet]
        public async Task<ActionResult> GetAllCategories()
        {
            return Ok(await db.GetAllCategories());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetCategoryDetails(string id)
        {
            return Ok(await db.GetCategoryById(id));
        }

        [HttpPost]
        public async Task<ActionResult> CreateCategory([FromBody] Category category)
        {
            if (category == null)
            {
                return BadRequest();
            }

            if (category.Title == string.Empty)
            {
                ModelState.AddModelError("Name", "The category Name is empty");
            }

            await db.InsertCategory(category);

            return Created("Created", true);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateCategory([FromBody] Category category, string id)
        {
            if (category == null)
            {
                return BadRequest();
            }

            if (category.Title == string.Empty)
            {
                ModelState.AddModelError("Name", "The category Name is empty");
            }

            category.Id = id;
            await db.UpdateCategory(category);

            return Created("Created", true);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCategory(string id)
        {

            await db.DeleteCategory(id);

            return NoContent();
        }
    }
}
