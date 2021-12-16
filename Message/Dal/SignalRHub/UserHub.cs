using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Net.Http.Headers;
using System.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Collections.Generic;
using System.Threading.Tasks;
using Message.Dal.Concrete;
using Message.Dal.Abstract;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Distributed;
namespace Message.Dal.SignalRHub
{
    public class UserHub : Hub
    {
        private readonly IHubContext<UserHub> _userHub;    
        private readonly IHttpContextAccessor _httpContextAccessor; 
        private readonly IConfiguration _configuration; 
        private IRabbitMqUserRepository _userRepo;
        private readonly string _key;
        private readonly IServiceProvider _serviceProvider;
        private readonly IDistributedCache _distrubuted;
        public UserHub(IHubContext<UserHub> userHub,IConfiguration configuration,IServiceProvider serviceProvider,IDistributedCache distrubuted)
        {
            _configuration = configuration;
            _userHub = userHub;
            _httpContextAccessor = new HttpContextAccessor();  
            _serviceProvider=serviceProvider;
            _distrubuted = distrubuted;   
            _key=_configuration["rediscache:Key"].ToString();            
        }

        public override Task OnConnectedAsync()
	    {      
            _userRepo = new RabbitMqUserRepository(_configuration,_serviceProvider); 
           var token = string.Empty;           
           var httpContext = _httpContextAccessor.HttpContext.Request.Cookies;
            var userName = string.Empty;
           if(!httpContext.Any())
           {
               return Task.CompletedTask;
           }
           token = httpContext.FirstOrDefault(x => x.Key == "CodeChatCookie").Value;
           if(AuthenticationHeaderValue.TryParse(token,out var headerVal))
            {                
                var handler = new JwtSecurityTokenHandler();
                var val = handler.ReadJwtToken(headerVal.ToString());  
                userName = val.Claims.FirstOrDefault(x => x.Type == "Name").Value;          
            } 
            var getUsersSerialize = _distrubuted.GetString(_key);
            if(getUsersSerialize == null)
            {
                _distrubuted.SetString(_key,JsonConvert.SerializeObject(new List<string>(){"orhan"}));
            }
            var deSerializeUsers = JsonConvert.DeserializeObject<List<string>>(getUsersSerialize);           
            if(!deSerializeUsers.Contains(userName))
            {
                _distrubuted.Remove(_key);
                deSerializeUsers.Add(userName); 
                _userRepo.Consumer(deSerializeUsers);
                _userRepo.Reciever();
                var serializeUsers = JsonConvert.SerializeObject(deSerializeUsers);
                _distrubuted.SetString(_key,serializeUsers);
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
        if(!httpContext.Any())
        {
            return Task.CompletedTask;
        }
        token = httpContext.FirstOrDefault(x => x.Key == "CodeChatCookie").Value;
        if(AuthenticationHeaderValue.TryParse(token,out var headerVal))
            {                
                var handler = new JwtSecurityTokenHandler();
                var val = handler.ReadJwtToken(headerVal.ToString());    
                userName = val.Claims.FirstOrDefault(x => x.Type == "Name").Value;          
            }   
            var getUsersSerialize = _distrubuted.GetString(_key);
            var deSerializeUsers = JsonConvert.DeserializeObject<List<string>>(getUsersSerialize);  
            _distrubuted.Remove(_key);           
            deSerializeUsers.Remove(userName);
            _userRepo.Consumer(deSerializeUsers);
            _userRepo.Reciever(); 
            var serializeUsers =  JsonConvert.SerializeObject(deSerializeUsers);
            _distrubuted.SetString(_key,serializeUsers);  
            base.OnDisconnectedAsync(exception);  
	       return Task.CompletedTask;
	    }
    }
}