using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Data;
using Microsoft.EntityFrameworkCore;
using Models;
using Services.Contracts;

namespace Services
{
    public class UsersService: IUsersService
    {
        private readonly ApplicationContext db;

        public UsersService(ApplicationContext db)
        {
            this.db = db;
        }

        public IQueryable<User> AllUsers(string currentUser)
        {
            return this.db.Users.Where(u => u.UserName != currentUser);
        }

        public IQueryable<User> SearchByName(string search, string currentUser)
        {
            return this.db.Users
                .Include(u => u.Orders)
                .Include(u => u.Reports)
                .Include(u => u.MyProducts)
                .Where(u => u.UserName == search.ToLower() &&
                            u.UserName != currentUser
                );
        }

        public bool RestrictUser(string userId)
        {
            var user = this.db.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return false;
            }

            user.IsRestricted = true;
            this.db.SaveChanges();
            return true;
        }

        public bool UnRestrictUser(string userId)
        {
            var user = this.db.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return false;
            }

            user.IsRestricted = false;
            this.db.SaveChanges();
            return true;
        }
    }
}
