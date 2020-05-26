using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ChatApplication.Database;
using ChatApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore;

namespace ChatApplication.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var chat = _context
                        .Chats.Include(x => x.Users)
                        .Where(x => !x.Users.Any(y => y.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value))
                        .ToList();
            return View(chat);
        }

        public IActionResult FindUser(string user)
        {
            var Users = _context.Users
                        .Where(x => x.Id != User.FindFirst(ClaimTypes.NameIdentifier).Value)
                        .ToList();

            return View(Users);
        }

        public IActionResult Private()
        {
            var chats = _context.Chats
                .Include(x => x.Users)
                    .ThenInclude(x => x.User)
                .Where(x => x.Type == ChatType.Private && x.Users.Any(y => y.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value))
                .ToList();

            return View(chats);
        }

        //[HttpPost]
        public async Task<IActionResult> SendPrivateMessage(string userId)
        {
            var chat = new Chat
            {
                Type = ChatType.Private
            };

            //User the private message is being be created for
            chat.Users.Add(new ChatUser 
            {
                UserId = userId
            });

            //The user initiating the private message
            chat.Users.Add(new ChatUser
            {
                UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value
            });

            _context.Chats.Add(chat);
            await _context.SaveChangesAsync();

            return Redirect($"/Chat/{chat.Id}");
        }

        [HttpGet]
        [Route("/Chat/{id}")]
        public IActionResult Chat(int Id)
        {
            var Chat = _context.Chats
                .Include(x => x.Messages)
                .FirstOrDefault(x => x.Id == Id);

             return View(Chat);
        }
        

        [HttpPost]
        public async Task<IActionResult> CreateMessage(int chatId, string message)
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

            return RedirectToAction("Chat", new { id = chatId });
        }


        [HttpPost]
        public async Task<IActionResult> CreateRoom(string name)
        {
            var chat = new Chat
            {
                Name = name,
                Type = ChatType.Room
            };

            try
            {
                chat.Users.Add(new ChatUser
                {
                    UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value,
                    Role = UserRole.Admin
                });

                _context.Chats.Add(chat);


                await _context.SaveChangesAsync();
            }
            catch(Exception e)
            {

            }
           
           

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> JoinRoom(int Id)
        {
            var chatUSer = new ChatUser
            {
                UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value,
                ChatId = Id,
                Role = UserRole.Member
            };


            _context.ChatUsers.Add(chatUSer);


            await _context.SaveChangesAsync();

            return Redirect($"/Chat/{Id}");
        }
    }
}