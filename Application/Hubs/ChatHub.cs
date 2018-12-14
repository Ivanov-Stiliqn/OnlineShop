using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Models;

namespace Application.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly UserManager<User> userManager;

        public ChatHub(UserManager<User> userManager)
        {
            this.userManager = userManager;
        }

        public async Task Send(string message, string username)
        {
            var user = await userManager.FindByNameAsync(username);
            var currentUser = await userManager.FindByNameAsync(this.Context.User.Identity.Name);
            await this.Clients.Users(user.Id, currentUser.Id).SendAsync("NewMessage", new Message
            {
                User = this.Context.User.Identity.Name,
                Text = message,
            });
        }
    }
}
