using Microsoft.AspNetCore.Mvc;
using OutfixApi.Models;
using OutfixApi.Repositories;

namespace OutfixApi.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : Controller
    {
        private IUserCollection db = new UserCollection();

        [HttpGet]
        public async Task<ActionResult> GetAllUsers()
        {
            return Ok(await db.GetAllUsers());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetUserDetails(string id)
        {
            return Ok(await db.GetUserById(id));
        }

        [HttpPost]
        public async Task<ActionResult> CreateUser([FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest();
            }

            if (user.Name == string.Empty)
            {
                ModelState.AddModelError("Name", "The user Name is empty");
            }

            await db.CreateUser(user);

            return Created("Created", true);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateUser([FromBody] User user, string id)
        {
            if (user == null)
            {
                return BadRequest();
            }

            if (user.Name == string.Empty)
            {
                ModelState.AddModelError("Name", "The product Name is empty");
            }

            user.Id = id;
            await db.UpdateUser(user);

            return Created("Created", true);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(string id)
        {

            await db.DeleteUser(id);

            return NoContent();
        }

    }
}
