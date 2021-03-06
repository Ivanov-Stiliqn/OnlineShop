﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace Services.Contracts
{
    public interface IUsersService
    {
        ICollection<User> AllUsers(string currentUser);

        ICollection<User> SearchByName(string search, string currentUser);

        Task<bool> RestrictUser(string userId, string currentUser);

        Task<bool> UnRestrictUser(string userId, string currentUser);

        UserInfo GetUserInfo(string username);

        Task UpdateUserInfo(UserInfo userInfo, string username);

        Task AddProductToWhishlist(string productId, string username);

        ICollection<Product> GetWishList(string username);

        Task RemoveProductFromWishlist(string productId, string username);

        ICollection<Product> GetUserProducts(string username);

        bool IsUserRestricted(string username);
    }
}
