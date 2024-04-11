using Microsoft.AspNetCore.Mvc;
using OutfixApi.Models;
using OutfixApi.Repositories;
using System.Data;

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
            var userFounded = await db.GetUserById(id);

            if (userFounded == null)
            {
                ModelState.AddModelError("message", "User not found");
                return StatusCode(StatusCodes.Status404NotFound, ModelState);
            }

            return Ok(userFounded);
        }

        [HttpGet("/api/users/name/{name}")]
        public async Task<ActionResult> GetUserByName(string name)
        {
            var userFounded = await db.GetUserByName(name);

            if(userFounded == null)
            {
                ModelState.AddModelError("message", "User not found");
                return StatusCode(StatusCodes.Status404NotFound, ModelState);
            }

            return Ok(userFounded);
        }

        [HttpPost]
        public async Task<ActionResult> CreateUser([FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest();
            }

            if (string.IsNullOrEmpty(user.Name))
            {
                ModelState.AddModelError("message", "The user Name is empty");
                return BadRequest(ModelState);
            }

            if (string.IsNullOrEmpty(user.Email))
            {
                ModelState.AddModelError("message", "The user Email is empty");
                return BadRequest(ModelState);
            }

            if (string.IsNullOrEmpty(user.Password))
            {
                ModelState.AddModelError("message", "The user Password is empty");
                return BadRequest(ModelState);
            }

            if (string.IsNullOrEmpty(user.Role))
            {
                ModelState.AddModelError("message", "The user Role is empty");
                return BadRequest(ModelState);
            }

            if (user.Role != "admin" && user.Role != "customer" && user.Role != "seller") 
            {
                ModelState.AddModelError("message", "User role can be admin or customer or seller");
                return BadRequest(ModelState);
            }

            var userFounded = await db.GetUserByEmail(user.Email);

            if(userFounded != null)
            {
                if (userFounded.Email == user.Email)
                {
                    ModelState.AddModelError("message", $"The user with {user.Email} already exists");
                    return BadRequest(ModelState);
                }
            }

            await db.CreateUser(user);

            return Created("User created successfully", true);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateUser([FromBody] User user, string id)
        {
            if (user == null)
            {
                ModelState.AddModelError("message", "The user can´t be null");
                return BadRequest(ModelState);
            }

            if (string.IsNullOrEmpty(user.Name))
            {
                ModelState.AddModelError("message", "The user Name is empty");
                return BadRequest(ModelState);
            }

            if (string.IsNullOrEmpty(user.Email))
            {
                ModelState.AddModelError("message", "The user Email is empty");
                return BadRequest(ModelState);
            }

            if (string.IsNullOrEmpty(user.Password))
            {
                ModelState.AddModelError("message", "The user Password is empty");
                return BadRequest(ModelState);
            }

            if(string.IsNullOrEmpty(id))
            {
                ModelState.AddModelError("message", "Id was not provided");
                return BadRequest(ModelState);
            }

            var userFounded = await db.GetUserById(id);

            if (userFounded == null)
            {
                ModelState.AddModelError("message", "User not found");
                return StatusCode(StatusCodes.Status404NotFound, ModelState);
            }

            user.Id = id;
            await db.UpdateUser(user);

            return Created("User updated successfully", true);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(string id)
        {

            var userFounded = await db.GetUserById(id);

            if (userFounded == null)
            {
                ModelState.AddModelError("message", "User not found");
                return StatusCode(StatusCodes.Status404NotFound, ModelState);
            }

            await db.DeleteUser(id);

            return NoContent();
        }

    }
}
