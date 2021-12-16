using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net.Http.Headers;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
namespace Core.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class CustomAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private string[] _requiredRoleNames;
        public CustomAuthorizeAttribute(string[] requiredRoleNames)
        {
            _requiredRoleNames = requiredRoleNames;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var authorization = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            if(AuthenticationHeaderValue.TryParse(authorization, out var headerVal))
            {
                var token = headerVal.Parameter;
                if(token != "null")
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var securityToken = (JwtSecurityToken)tokenHandler.ReadToken(token);
                    var claimValue = securityToken.Claims.FirstOrDefault(c => c.Type == "Role")?.Value;
                    if(_requiredRoleNames != null && !_requiredRoleNames.Contains(claimValue))
                    {
                        context.Result = new ForbidResult();
                    }
                }               
            }
        }
    }
}