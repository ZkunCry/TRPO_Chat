using Microsoft.AspNetCore.SignalR;
using WebApplication1.Controllers;


namespace WebApplication1.Hubs
{
    public class ChatHub : Hub
    {

        public void SendMessageToRoom(string roomId, string senderId, string messageText)
        {
            Clients.All.SendAsync("ReceiveMessage", roomId, senderId, messageText);
        }

    }
}
