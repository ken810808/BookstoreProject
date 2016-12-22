using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookStore_Web_.Models
{
    public class ChatModel
    {
        public List<ChatUser> Users;

        public List<ChatMessage> ChatHistory;

        public ChatModel()
        {
            Users = new List<ChatUser>();
            ChatHistory = new List<ChatMessage>();

            ChatHistory.Add(new ChatMessage()
            {
                Message = "The chat server started at " + DateTime.Now
            });
        }

        public class ChatUser
        {
            public string NickName;
            public DateTime LoggedOnTime;
            public DateTime LastPing;
        }

        public class ChatMessage
        {
            public ChatUser ByUser;

            public DateTime When = DateTime.Now;

            public string Message = "";

        }
    }
}