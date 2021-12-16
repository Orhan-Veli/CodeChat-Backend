using System;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Identity.Model;
using Identity.Business.Concrete;
using Microsoft.AspNetCore.Cors;
using Core.Filters;
using Identity.Dtos;
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
        [AllowAnonymous]
        [HttpPost("sign")]
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
        [AllowAnonymous]
        [HttpPost("login")]
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
        
        [CustomAuthorizeAttribute(new string[]{"Admin"})]
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

        [CustomAuthorizeAttribute(new string[]{"Admin", "User"})]
        [HttpPost("checkuser")]
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
        [CustomAuthorizeAttribute(new string[]{"Admin", "User"})] 
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
        [CustomAuthorizeAttribute(new string[]{"Admin"})]
        [HttpPost("role")]
        public async Task<IActionResult> CreateRoleAsync([FromQuery]string role)
        {
            var result = await _userService.CreateRoleAsync(role);
            return Ok();
        }
        
        [CustomAuthorizeAttribute(new string[]{"Admin", "User"})]       
        [HttpGet("getuserrole")]
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
        [CustomAuthorizeAttribute(new string[]{"Admin", "User"})] 
        [HttpGet("getuserid")]
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
        [CustomAuthorizeAttribute(new string[]{"Admin"})] 
        [HttpGet("getalluser")]
        public async Task<IActionResult> GetAllUserAsync()
        {
            var result = await _userService.GetAllUserAsync();
            return Ok(result);
        } 
        [CustomAuthorizeAttribute(new string[]{"Admin"})] 
        [HttpPut("updateuserrole")]
        public async Task<IActionResult> UpdateUserRoleAsync([FromBody] UpdateUserRoleDto updateUserRoleDto)
        {
            var result = await _userService.UpdateUserRoleAsync(updateUserRoleDto.UserId,updateUserRoleDto.UserRole);
            return Ok(result);
        } 
    }
}