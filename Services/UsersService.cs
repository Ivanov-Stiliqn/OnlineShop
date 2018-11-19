using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Data;
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

        public IQueryable<User> AllUsers()
        {
            return this.db.Users;
        }

        public IQueryable<User> SearchByName(string username)
        {
            return this.db.Users.Where(u => u.UserName.ToLower().Contains(username.ToLower()));
        }
    }
}
