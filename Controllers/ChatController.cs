using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatApplication.Database;
using ChatApplication.Hubs;
using ChatApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace ChatApplication.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class ChatController : Controller
    {
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatController(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }

        [HttpPost("[action]/{ConnectionId}/{chatId}")]
        public async Task<IActionResult> JoinRoom(string ConnectionId, string chatId)
        {
            await _hubContext.Groups.AddToGroupAsync(ConnectionId, chatId);
            return Ok();
        }

        [HttpPost("[action]/{ConnectionId}/{chatId}")]
        public async Task<IActionResult> LeaveRoom(string ConnectionId, string chatId)
        {
            await _hubContext.Groups.RemoveFromGroupAsync(ConnectionId, chatId);
            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> SendMessage(int chatId, string message, [FromServices] AppDbContext _context)
        {
            var Message = new Message
            {
                Name = User.Identity.Name,
                ChatId = chatId,
                Text = message,
                TimeStamp = DateTime.Now
            };

            _context.Messages.Add(Message);
            await _context.SaveChangesAsync();

            await _hubContext.Clients.Group(chatId.ToString()).SendAsync("ReceiveMessage", new
            {
                Text = Message.Text,
                Name = Message.Name,
                TimeStamp = Message.TimeStamp.ToString("dd/MM/yyy hh:mm:ss")
            });
            return Ok();

            
        }
    }
}