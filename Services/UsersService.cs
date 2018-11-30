using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using Data.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;
using Models;
using Services.Contracts;

namespace Services
{
    public class UsersService: IUsersService
    {
        private readonly IRepository<User> usersRepository;

        public UsersService(IRepository<User> usersRepository)
        {
            this.usersRepository = usersRepository;
        }

        public ICollection<User> AllUsers(string currentUser)
        {
            return this.usersRepository.All()
                .Include(u => u.Orders)
                .Include(u => u.Reports)
                .Include(u => u.MyProducts)
                .Where(u => u.UserName != currentUser)
                .ToList();
        }

        public ICollection<User> SearchByName(string search, string currentUser)
        {
            return this.usersRepository.All()
                .Include(u => u.Orders)
                .Include(u => u.Reports)
                .Include(u => u.MyProducts)
                .Where(u => u.UserName == search.ToLower() &&
                            u.UserName != currentUser
                ).ToList();
        }

        public async Task<bool> RestrictUser(string userId)
        {
            var user = this.usersRepository.All().FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return false;
            }

            user.IsRestricted = true;
            await this.usersRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UnRestrictUser(string userId)
        {
            var user = this.usersRepository.All().FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return false;
            }

            user.IsRestricted = false;
            await this.usersRepository.SaveChangesAsync();
            return true;
        }
    }
}
