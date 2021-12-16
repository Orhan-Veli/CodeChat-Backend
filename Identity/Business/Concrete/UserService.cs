using System;
using Microsoft.EntityFrameworkCore;
using Identity.Model;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Identity.Business.Abstract;
using Identity.Business.Concrete;
using Identity.Utilities.Abstract;
using Identity.Utilities.Concrete;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Net;
using System.Net.Http.Headers;
using System.Web;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Identity.Dtos;
namespace Identity.Business.Concrete
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        public UserService(UserManager<AppUser> userManager,SignInManager<AppUser> signInManager,IConfiguration configuration,RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _roleManager = roleManager;
        }

        public async Task<IResult<bool>> CreateUserAsync(UserModel userModel)
        {
            AppUser appUser = new AppUser
            {
                UserName = userModel.UserName,
                Email = userModel.Email
            };
            IdentityResult result = await _userManager.CreateAsync(appUser, userModel.Password);
            var roles = _roleManager.Roles.ToList();
            var role = await _roleManager.GetRoleNameAsync(roles.FirstOrDefault(x => x.Name == "User"));
            if(result.Succeeded)
            {
                await _userManager.AddToRoleAsync(appUser,role);
                return new Result<bool>(true);
            }
            return new Result<bool>(false);
        }
        public async Task<IResult<bool>> LoginAsync(UserLoginModel userLoginModel)
        {
            AppUser user = await _userManager.FindByEmailAsync(userLoginModel.Email);
            var userRole = await _userManager.GetRolesAsync(user);         
            if(user != null)
            {
                await _signInManager.SignOutAsync();
                SignInResult result = await _signInManager.PasswordSignInAsync(user,userLoginModel.Password, true, true);
                if(result.Succeeded)
                {
                    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);   
                    var claims = new Claim[] 
                    {
                        new Claim("id",user.Id.ToString()),
                        new Claim("Name",user.UserName.ToString()),
                        new Claim("Role",userRole.FirstOrDefault())
                    };

                    var securityToken = new JwtSecurityToken(
                        _configuration["Jwt:Issuer"], 
                        null,
                        claims,                      
                        expires:DateTime.Now.AddHours(3),
                        signingCredentials:credentials);
                    var token = new JwtSecurityTokenHandler().WriteToken(securityToken);    
                    return new Result<bool>(result.Succeeded, token);
                }
                return new Result<bool>(result.Succeeded,"user sign error"); 
            }
            return new Result<bool>(false,"user is null");
        }

        public async Task<IResult<string>> GetUserRoleAsync(string token)
        {
            try
            {
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = (JwtSecurityToken)tokenHandler.ReadToken(token);
            var claimValue = securityToken.Claims.FirstOrDefault(c => c.Type == "Role")?.Value;
                return new Result<string>(true,claimValue);
            }
            catch (Exception ex)
            {
                return new Result<string>(false);
            }
        }

        public async Task<IResult<string>> GetUserIdAsync(string token)
        {
            try
            {
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = (JwtSecurityToken)tokenHandler.ReadToken(token);
            var claimValue = securityToken.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
                return new Result<string>(true,claimValue);
            }
            catch (Exception ex)
            {
                return new Result<string>(false);
            }
        }

        public async Task<IResult<bool>> BanUserAsync(Guid id)
        {
            AppUser user = await _userManager.FindByIdAsync(id.ToString());
            if(user != null)
            {   
                await _userManager.SetLockoutEnabledAsync(user,true);
                await _userManager.SetLockoutEndDateAsync(user,new DateTime(2099,01,01));
                return new Result<bool>(true);
            }         
            return new Result<bool>(false);
        }

        public async Task<IResult<bool>> CheckUserAsync(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

                tokenHandler.ValidateToken(token,new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                },out var validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                return new Result<bool>(true);
            }
            catch (Exception ex)
            {
                return new Result<bool>(false);
            }
        }

        public async Task LogOutUser()
        {
            await _signInManager.SignOutAsync();
        }


        public async Task<IResult<bool>> UpdatePasswordAsync(string password, string userId,string token)
        {
            AppUser user = await _userManager.FindByIdAsync(userId);
            IdentityResult result = await _userManager.ResetPasswordAsync(user, HttpUtility.UrlDecode(token), password);
            if(result.Succeeded)
            {
                await _userManager.UpdateSecurityStampAsync(user);
                return new Result<bool>(true);
            }
            return new Result<bool>(false);
        }

        public async Task<IResult<bool>> CreateRoleAsync(string role)
        {
            var checkRoleIsExist = await _roleManager.RoleExistsAsync(role);
            if(!checkRoleIsExist)
            {
                var newRole = new IdentityRole();
                newRole.Name = role;
                await _roleManager.CreateAsync(newRole);
            }
            return new Result<bool>(true);
        }

        public async Task<IResult<List<GetAllUsersDto>>> GetAllUserAsync()
        {
            var usersDto = new List<GetAllUsersDto>();
            var userList = await _userManager.Users.ToListAsync();     
            if(userList != null)
            {
                foreach (var user in userList)
                {
                    var userDto = new GetAllUsersDto
                    {
                        UserId = Guid.Parse(user.Id),
                        Email = user.Email,
                        LockoutDate = user.LockoutEnd?.ToString("MM/dd/yyyy"),
                        UserName = user.UserName,
                        UserRole = _userManager.GetRolesAsync(user).Result?.FirstOrDefault()
                    };
                    usersDto.Add(userDto);
                }
            return new Result<List<GetAllUsersDto>>(true,usersDto);
            }
            return new Result<List<GetAllUsersDto>>(true,new List<GetAllUsersDto>());
        }

        public async Task<IResult<bool>> UpdateUserRoleAsync(string Id,string userRole)
        {
            var user = await _userManager.FindByIdAsync(Id);
            if(user != null)
            {
                var removeRole = await _userManager.RemoveFromRoleAsync(user,userRole);
                if(removeRole.Succeeded)
                {
                    var newRoleName = (userRole == "User") ? "Admin" : "User";
                    await _userManager.AddToRoleAsync(user,newRoleName);                       
                    return new Result<bool>(true);
                }
            }
            return new Result<bool>(false);
        }
    }
}

