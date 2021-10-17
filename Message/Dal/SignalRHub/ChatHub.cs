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
using Microsoft.Extensions.Configuration;
namespace Message.Dal.SignalRHub
{
    public class ChatHub : Hub
    {
        private readonly IHubContext<ChatHub> _chatHub;    
        private readonly IHttpContextAccessor _httpContextAccessor; 
        private readonly IElasticRepository<OnlineUserModel> _elasticRepository;  
        private readonly string _indexName;
        public ChatHub(IHubContext<ChatHub> chatHub,IElasticRepository<OnlineUserModel> elasticRepository,IConfiguration configuration)
        {
            _chatHub = chatHub; 
            _httpContextAccessor = new HttpContextAccessor();
            _elasticRepository = elasticRepository;
            _indexName = configuration["elasticsearchserver:User"].ToString();
        }
        public async Task SendMessage(Guid categoryId,RabbitMqModel rabbitMqModel)
        {          
            await _chatHub.Clients.All.SendAsync(categoryId.ToString(),rabbitMqModel);           
        }        
        
        public override Task OnConnectedAsync()
	    {      
            
           var token = string.Empty;           
           var httpContext = _httpContextAccessor.HttpContext.Request.Cookies;
           var onlineUserModel = new OnlineUserModel();
           onlineUserModel.Id = Context.ConnectionId;           
           if(!httpContext.Any())
           {
               return Task.CompletedTask;
           }
           foreach (var item in httpContext)
                {
                    if(item.Key == "CodeChatCookie")
                    {
                        token = item.Value;
                        break;
                    }
                }
           if(AuthenticationHeaderValue.TryParse(token,out var headerVal))
            {                
                var handler = new JwtSecurityTokenHandler();
                var val = handler.ReadJwtToken(headerVal.ToString());  
                onlineUserModel.UserName = val.Claims.FirstOrDefault(x => x.Type == "Name").Value;           
            } 
            var checkOnlineUser = _elasticRepository.GetUserAsync(onlineUserModel.UserName,_indexName); 
            if(checkOnlineUser.Result == null)
            {
                _elasticRepository.CreateUserAsync(onlineUserModel.Id,onlineUserModel,_indexName);
            }           
           base.OnConnectedAsync();         
           return Task.CompletedTask;
	    }	  

	     public override Task OnDisconnectedAsync(Exception exception)
	    {
            var httpContext = _httpContextAccessor.HttpContext.Request.Cookies;
            var token = string.Empty;          
            var result = _elasticRepository.DeleteUserAsync(Context.ConnectionId,_indexName);       
            base.OnDisconnectedAsync(exception);  
	       return Task.CompletedTask;
	    }
        
    }
}