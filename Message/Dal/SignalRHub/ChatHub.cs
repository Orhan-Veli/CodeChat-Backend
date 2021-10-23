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
        public ChatHub(IHubContext<ChatHub> chatHub)
        {
            _chatHub = chatHub; 
        }
        public async Task SendMessage(Guid categoryId,string message)
        {          
            await _chatHub.Clients.All.SendAsync(categoryId.ToString(),message);           
        }       
        
    }
}