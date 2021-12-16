using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Identity.Utilities.Abstract;
using Identity.Model;
using Identity.Dtos;
namespace Identity.Business.Abstract
{
    public interface IUserService
    {
        Task<IResult<bool>> CreateUserAsync(UserModel userModel);
        Task<IResult<bool>> LoginAsync(UserLoginModel userLoginModel);
        Task<IResult<bool>> CheckUserAsync(string token);
        Task LogOutUser();
        Task<IResult<bool>> UpdatePasswordAsync(string password,string userId,string token);
        Task<IResult<bool>> CreateRoleAsync(string role);
        Task<IResult<string>> GetUserRoleAsync(string token);
        Task<IResult<string>> GetUserIdAsync(string token);
        Task<IResult<bool>> BanUserAsync(Guid id);
        Task<IResult<List<GetAllUsersDto>>> GetAllUserAsync();
        Task<IResult<bool>> UpdateUserRoleAsync(string Id,string userRole);
    }
}