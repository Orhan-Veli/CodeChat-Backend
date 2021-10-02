using System;
using Microsoft.AspNetCore.Identity;
namespace Identity.Model.Auth
{
    public class CustomIdentityErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError DuplicateEmail(string email)
        {
            return base.DuplicateEmail(email);
        }
        public override IdentityError DuplicateUserName(string userName)
        {
            return base.DuplicateUserName(userName);
        }
        public override IdentityError InvalidEmail(string email)
        {
            return base.InvalidEmail(email);
        }
        public override IdentityError InvalidUserName(string userName)
        {
            return base.InvalidUserName(userName);
        }
    }
}