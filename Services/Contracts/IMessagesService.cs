using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace Services.Contracts
{
    public interface IMessagesService
    {
        Task AddMessage(Message message);

        ICollection<Message> GetUnReadMessages(string username);

        Task MarkMessagesAsRead(string receiver, string sender);
    }
}
