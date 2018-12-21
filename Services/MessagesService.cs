using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;
using Models;
using Services.Contracts;

namespace Services
{
    public class MessagesService: IMessagesService
    {
        private readonly IRepository<Message> messagesRepository;

        public MessagesService(IRepository<Message> messagesRepository)
        {
            this.messagesRepository = messagesRepository;
        }

        public async Task AddMessage(Message message)
        {
            await this.messagesRepository.AddAsync(message);
            await this.messagesRepository.SaveChangesAsync();
        }

        public ICollection<Message> GetUnReadMessages(string username)
        {
            return this.messagesRepository.All()
                .Include(m => m.Receiver)
                .Include(m => m.User)
                .Where(m => m.Receiver.UserName == username && !m.IsRead).ToList();
        }

        public async Task MarkMessagesAsRead(string receiver, string sender)
        {
            var messages = this.messagesRepository.All().Include(m => m.User).Include(m => m.Receiver)
                .Where(m => m.Receiver.UserName == receiver && m.User.UserName == sender).ToList();

            messages.ForEach(m => m.IsRead = true);
            await this.messagesRepository.SaveChangesAsync();
        }
    }
}
