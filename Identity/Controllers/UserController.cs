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
        public async Task<IActionResult> SignInAsync([FromBody] UserModel userModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.ErrorCount);
            }
            var result = await _userService.CreateUserAsync(userModel);
            if (result.Success)
            {
                return Ok();
            }
            return BadRequest();
        }

        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync([FromBody]UserLoginModel userLoginModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.ErrorCount);
            }
            var result = await _userService.LoginAsync(userLoginModel);
            if(result.Success)
            {
                return Ok(result.Message);
            }
            return BadRequest();            
        }

        [HttpPut("{Id}")]
        public async Task<IActionResult> BanUserAsync(Guid id)
        {
            var result = await _userService.BanUserAsync(id);
            if(result.Success == false)
            {
                return BadRequest();
            }
            return Ok();
        }


        [HttpPost("CheckUser")]
        public async Task<IActionResult> CheckUserIsLoggedInAsync([FromHeader] string authorization)
        {
            if(authorization == null) 
            {
                return Unauthorized();
            }
                if(AuthenticationHeaderValue.TryParse(authorization, out var headerVal))
                {
                var token = headerVal.Parameter;
                var result = await _userService.CheckUserAsync(token);
                if(result.Success)
                {
                    return Ok();
                }
            }               
            return Unauthorized();
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _userService.LogOutUser();
            return Ok();
        }
        //"[action]/{userId}/{token}"
        // [HttpGet("user/{userId}/{token}")]
        // public IActionResult UpdatePassword(string userId, string token)
        // {
        //     return Ok();
        // }

        // [HttpPost("user/{userId}/{token}")]
        // public async Task<IActionResult> UpdatePassword(string password, string userId, string token)
        // {
        //     var result = await _userService.UpdatePassword(password, userId, token);
        //     if(result.Success)
        //     {
        //         return Ok();
        //     }
        //     return BadRequest();
        // }
        [HttpPost("role")]
        public async Task<IActionResult> CreateRoleAsync([FromQuery]string role)
        {
            var result = await _userService.CreateRoleAsync(role);
            return Ok();
        }

        [HttpPost("getuserrole")]
        public async Task<IActionResult> GetUserRoleAsync([FromHeader] string authorization)
        {
            if(authorization == null) 
            {
                return Unauthorized();
            }
                if(AuthenticationHeaderValue.TryParse(authorization, out var headerVal))
                {
                var token = headerVal.Parameter;
                var result = await _userService.GetUserRoleAsync(token);
                if(result.Success)
                {
                    return Ok(result.Message);
                }
            }               
            return Unauthorized();
        }  

        [HttpPost("getuserid")]
        public async Task<IActionResult> GetUserIdAsync([FromHeader] string authorization)
        {
            if(authorization == null) 
            {
                return Unauthorized();
            }
            if(AuthenticationHeaderValue.TryParse(authorization, out var headerVal))
            {
                var token = headerVal.Parameter;
                var result = await _userService.GetUserIdAsync(token);
                if(result.Success)
                {
                    return Ok(result.Message);
                }
            }               
            return Unauthorized();
        }  
    }
}