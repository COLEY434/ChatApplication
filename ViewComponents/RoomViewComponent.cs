using ChatApplication.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ChatApplication.ViewComponents
{
    public class RoomViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;

        public RoomViewComponent(AppDbContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            var UserId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var Chats = _context.ChatUsers
                         .Include(x => x.Chat)
                         .Where(x => x.UserId == UserId && x.Chat.Type == Models.ChatType.Room)
                         .Select(x => x.Chat)
                         .ToList();

            //var Chats = _context.Chats


            //         .ToList();
            return View(Chats);
        }
    }
}
