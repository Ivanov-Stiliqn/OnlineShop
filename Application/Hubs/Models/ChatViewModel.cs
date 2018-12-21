using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Infrastructure.Mapping;
using AutoMapper;
using Models;

namespace Application.Hubs.Models
{
    public class ChatViewModel
    {
        public ChatViewModel()
        {
            this.UnReadMessages = new List<MessageViewModel>();
        }

        public string User { get; set; }

        public string Receiver { get; set; }

        public string Text { get; set; } = string.Empty;

        public ICollection<MessageViewModel> UnReadMessages { get; set; }
    }
}
