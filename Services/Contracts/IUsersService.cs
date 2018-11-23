using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Models;

namespace Services.Contracts
{
    public interface IUsersService
    {
        IQueryable<User> AllUsers(string currentUser);

        IQueryable<User> SearchByName(string search, string currentUser);

        bool RestrictUser(string userId);

        bool UnRestrictUser(string userId);
    }
}
