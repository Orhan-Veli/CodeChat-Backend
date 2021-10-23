using Microsoft.AspNetCore.SignalR;
using System.Text;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.Http;
using System;
using System.Web;
using System.Net.Http.Headers;
using System.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Threading.Tasks;
using Message.Dal.Concrete;
using Message.Dal.Abstract;
using Message.Dal.Model;
using Message.Business.Abstract;
using Microsoft.Extensions.Configuration;
namespace Message.Dal.SignalRHub
{
    public class UserHub : Hub
    {
        private readonly IHubContext<UserHub> _userHub;    
        private readonly IHttpContextAccessor _httpContextAccessor; 
        private readonly IConfiguration _configuration; 
        private IRabbitMqUserRepository _userRepo;
        private static List<string> users = new List<string>(); 
    private readonly IServiceProvider _serviceProvider;
        public UserHub(IHubContext<UserHub> userHub,IConfiguration configuration,IServiceProvider serviceProvider)
        {
            _configuration = configuration;
            _userHub = userHub;
            _httpContextAccessor = new HttpContextAccessor();  
            _serviceProvider=serviceProvider;
               
        }

        public override Task OnConnectedAsync()
	    {      
            _userRepo = new RabbitMqUserRepository(_configuration,_serviceProvider); 
           var token = string.Empty;           
           var httpContext = _httpContextAccessor.HttpContext.Request.Cookies;
            var userName = string.Empty;
           //var onlineUserModel = new OnlineUserModel();   
           //onlineUserModel.UserConnection = "online";    
           if(!httpContext.Any())
           {
               return Task.CompletedTask;
           }
           token = httpContext.FirstOrDefault(x => x.Key == "CodeChatCookie").Value;
           if(AuthenticationHeaderValue.TryParse(token,out var headerVal))
            {                
                var handler = new JwtSecurityTokenHandler();
                var val = handler.ReadJwtToken(headerVal.ToString());  
                //onlineUserModel.UserName = val.Claims.FirstOrDefault(x => x.Type == "Name").Value;    
                userName = val.Claims.FirstOrDefault(x => x.Type == "Name").Value;          
            } 
            if(!users.Contains(userName))
            {
                users.Add(userName); 
                _userRepo.Consumer(users);
                _userRepo.Reciever();
            }              
           base.OnConnectedAsync();         
           return Task.CompletedTask;
	    }	
	    public override Task OnDisconnectedAsync(Exception exception)
	    {      
        _userRepo = new RabbitMqUserRepository(_configuration,_serviceProvider); 
        var token = string.Empty;           
        var httpContext = _httpContextAccessor.HttpContext.Request.Cookies;
        var userName = string.Empty;
        // var onlineUserModel = new OnlineUserModel();
        //  onlineUserModel.UserConnection = "offline";  
        if(!httpContext.Any())
        {
            return Task.CompletedTask;
        }
        token = httpContext.FirstOrDefault(x => x.Key == "CodeChatCookie").Value;
        if(AuthenticationHeaderValue.TryParse(token,out var headerVal))
            {                
                var handler = new JwtSecurityTokenHandler();
                var val = handler.ReadJwtToken(headerVal.ToString());  
                //onlineUserModel.UserName = val.Claims.FirstOrDefault(x => x.Type == "Name").Value;  
                userName = val.Claims.FirstOrDefault(x => x.Type == "Name").Value;          
            }               
            users.Remove(userName);
                _userRepo.Consumer(users);
                _userRepo.Reciever();   
            base.OnDisconnectedAsync(exception);  
	       return Task.CompletedTask;
	    }
    }
}