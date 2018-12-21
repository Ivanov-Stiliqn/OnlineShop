using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Hubs.Models;
using Application.Infrastructure.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Models;
using Services.Contracts;

namespace Application.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly UserManager<User> userManager;
        private readonly IMessagesService messagesService;

        public ChatHub(UserManager<User> userManager, IMessagesService messagesService)
        {
            this.userManager = userManager;
            this.messagesService = messagesService;
        }

        public async Task Send(string message, string username)
        {
            var user = await userManager.FindByNameAsync(username);
            var currentUser = await userManager.FindByNameAsync(this.Context.User.Identity.Name);
            var newMessage = new Message
            {
                Text = message,
                ReciverId = user.Id,
                UserId = currentUser.Id,
                IsRead = false
            };

            await this.messagesService.AddMessage(newMessage);
            await this.Clients.Users(user.Id, currentUser.Id).SendAsync("NewMessage", new ChatViewModel
            {
                User = currentUser.UserName,
                Receiver = user.UserName,
                Text = message,
            });
        }

        public async Task NotRead()
        {
            var currentUser = await userManager.FindByNameAsync(this.Context.User.Identity.Name);

            var messages = this.messagesService.GetUnReadMessages(currentUser.UserName);
            var users = messages.Select(m => m.User.UserName).Distinct();

            var unReadMessages = users.Select(u => new ChatViewModel
            {
                User = u,
                Receiver = currentUser.UserName,
                UnReadMessages = messages.Where(m => m.User.UserName == u).Select(m => new MessageViewModel
                {
                    Text = m.Text,
                    DateOfCreation = DateTimeHelper.GetTimeSpan(m.DateOfCreation)
                }).ToList()
            }).ToList();

            foreach (var message in unReadMessages)
            {
                await this.Clients.User(currentUser.Id).SendAsync("NewMessage", message);
            }
        }

        public async Task MarkMessagesAsRead(string sender)
        {
            await this.messagesService.MarkMessagesAsRead(this.Context.User.Identity.Name, sender);
        }
    }
}
