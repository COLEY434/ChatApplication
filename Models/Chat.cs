using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApplication.Models
{
    public class Chat
    {
        public Chat()
        {
            Messages = new Collection<Message>();
            Users = new Collection<ChatUser>();
        }
        public int Id { get; set; }

        public string Name { get; set; }
        public ChatType Type { get; set; }
       
        public ICollection<Message> Messages { get; set; }
        public ICollection<ChatUser> Users { get; set; }
       
    }
}
