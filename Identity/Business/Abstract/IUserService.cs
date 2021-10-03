using System;
using System.Threading.Tasks;
using Identity.Utilities.Abstract;
using Identity.Model;
namespace Identity.Business.Abstract
{
    public interface IUserService
    {
        Task<IResult<bool>> CreateUser(UserModel userModel);

        Task<IResult<bool>> Login(UserLoginModel userLoginModel);

        Task<IResult<bool>> CheckUser(string token);

        Task LogOutUser();
        
        Task<IResult<bool>> UpdatePassword(string password,string userId,string token);
    }
}