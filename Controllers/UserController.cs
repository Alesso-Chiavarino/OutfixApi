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

            if (userFounded == null)
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

            if (userFounded != null)
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
        [HttpPut]
        public async Task<ActionResult> UpdateUser([FromBody] UpdateUserDto request, string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("Id was not provided");

            var user = await db.GetUserById(id);

            if (user == null)
                return NotFound("User not found");

            // Solo actualiza lo que venga (PARCHADO)
            if (!string.IsNullOrEmpty(request.Name))
                user.Name = request.Name;

            if (!string.IsNullOrEmpty(request.Email))
                user.Email = request.Email;

            if (!string.IsNullOrEmpty(request.Password))
                user.Password = request.Password;

            if (!string.IsNullOrEmpty(request.Role))
                user.Role = request.Role;

            await db.UpdateUser(user);

            return Ok(new { message = "User updated successfully" });
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