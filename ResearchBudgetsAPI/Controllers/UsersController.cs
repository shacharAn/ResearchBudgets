using System;
using Microsoft.AspNetCore.Mvc;
using RuppinResearchBudget.BL;
using RuppinResearchBudget.Models;
using System.ComponentModel.DataAnnotations;

namespace RuppinResearchBudget.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UsersBl _usersBl = new UsersBl();

        public class RegisterUserRequest
        {
            [Required]
            public string IdNumber { get; set; }
            [Required]
            public string UserName { get; set; }
            [Required]
            public string Email { get; set; }
            [Required]
            public string Password { get; set; }
            [Required]
            public string ConfirmPassword { get; set; }
            [Required]
            public string FirstName { get; set; }
            [Required]
            public string LastName { get; set; }
        }
        public class LoginRequest
        {
            [Required]
            public string UserName { get; set; }
            [Required]
            public string Password { get; set; }
        }

        // POST: api/users/register
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterUserRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                Users user = _usersBl.RegisterUser(
                    request.IdNumber,
                    request.UserName,
                    request.Email,
                    request.Password,
                    request.ConfirmPassword,
                    request.FirstName,
                    request.LastName
                );

                return Ok(user);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // POST: api/users/login
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                Users user = _usersBl.Login(request.UserName, request.Password);
                return Ok(user);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/users/{userName}/roles
        [HttpGet("{userName}/roles")]
        public IActionResult GetUserWithRoles(string userName)
        {
            try
            {
                var result = _usersBl.GetUserWithRoles(userName);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
