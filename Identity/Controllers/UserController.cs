using System;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Identity.Model;
using Identity.Business.Concrete;
using Microsoft.AspNetCore.Cors;
namespace Identity.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("_myAllowSpecificOrigins")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        public UserController(UserService userService)
        {
            _userService = userService;
        }
        [HttpPost("Sign")]
        public async Task<IActionResult> SignIn([FromBody] UserModel userModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.ErrorCount);
            }
            var result = await _userService.CreateUser(userModel);
            if (result.Success)
            {
                return Ok();
            }
            return BadRequest();
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody]UserLoginModel userLoginModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.ErrorCount);
            }
            var result = await _userService.Login(userLoginModel);
            if(result.Success)
            {
                return Ok(result.Message);
            }
            return BadRequest();            
        }

        [HttpPost("CheckUser")]
        public async Task<IActionResult> CheckUserIsLoggedIn([FromHeader] string authorization)
        {
            if(authorization == null) 
            {
                return Unauthorized();
            }
                if(AuthenticationHeaderValue.TryParse(authorization, out var headerVal))
                {
                var token = headerVal.Parameter;
                var result = await _userService.CheckUser(token);
                if(result.Success)
                {
                    return Ok();
                }
            }               
            return Unauthorized();
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _userService.LogOutUser();
            return Ok();
        }
        //"[action]/{userId}/{token}"
        [HttpGet("user/{userId}/{token}")]
        public IActionResult UpdatePassword(string userId, string token)
        {
            return Ok();
        }

        [HttpPost("user/{userId}/{token}")]
        public async Task<IActionResult> UpdatePassword(string password, string userId, string token)
        {
            var result = await _userService.UpdatePassword(password, userId, token);
            if(result.Success)
            {
                return Ok();
            }
            return BadRequest();

        }
    
    }
}