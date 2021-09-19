using Microsoft.AspNetCore.SignalR;
using System.Text;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;
namespace Message.Dal.SignalRHub
{
    public class ChatHub : Hub
    {
        private readonly IHubContext<ChatHub> _chatHub;
        public ChatHub(IHubContext<ChatHub> chatHub)
        {
            _chatHub = chatHub;
        }
        public async Task SendMessage(string message)
        {            
            await _chatHub.Clients.All.SendAsync("ReceiveMessage",message);
        }       
    }
}