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
        private readonly IRepository<UserInfo> userInfoRepository;
        private readonly IRepository<Product> productsRepository;

        public UsersService(IRepository<User> usersRepository, IRepository<UserInfo> userInfoRepository, IRepository<Product> productsRepository)
        {
            this.usersRepository = usersRepository;
            this.userInfoRepository = userInfoRepository;
            this.productsRepository = productsRepository;
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

        public UserInfo GetUserInfo(string username)
        {
            return this.usersRepository.All().Include(u => u.UserInfo).Where(u => u.UserName == username)
                .Select(u => u.UserInfo).FirstOrDefault();
        }

        public async Task UpdateUserInfo(UserInfo userInfo, string username)
        {
            if (userInfo.Id != Guid.Empty)
            {
                this.userInfoRepository.Update(userInfo);
                await this.userInfoRepository.SaveChangesAsync();
            }

            var user = this.usersRepository.All().FirstOrDefault(u => u.UserName == username);
            user.UserInfo = userInfo;

            await this.usersRepository.SaveChangesAsync();
           
        }

        public async Task AddProductToWhishlist(string productId, string username)
        {
            var user = this.usersRepository.All().FirstOrDefault(u => u.UserName == username);
            if (string.IsNullOrEmpty(user.Whishlist))
            {
                user.Whishlist = string.Empty;
            }

            var whishlist = user.Whishlist.Split(new []{", "}, StringSplitOptions.RemoveEmptyEntries).ToList();
            whishlist.Add(productId);

            user.Whishlist = string.Join(", ", whishlist);

            await this.usersRepository.SaveChangesAsync();
        }

        public ICollection<Product> GetWishList(string username)
        {
            var user = this.usersRepository.All().FirstOrDefault(u => u.UserName == username);
            var wishlist = user.Whishlist.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();

            return this.productsRepository
                .All()
                .Include(p => p.Sizes)
                .ThenInclude(p => p.Size)
                .Where(p => wishlist.Contains(p.Id.ToString()))
                .ToList();
        }

        public async Task RemoveProductFromWishlist(string productId, string username)
        {
            var user = this.usersRepository.All().FirstOrDefault(u => u.UserName == username);
            var wishlist = user.Whishlist.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();

            user.Whishlist = string.Join(", ", wishlist.Where(p => p != productId));

            await this.usersRepository.SaveChangesAsync();
        }

        public ICollection<Product> GetUserProducts(string username)
        {
            return this.usersRepository.All().Include(u => u.MyProducts).Where(u => u.UserName == username)
                .Select(u => u.MyProducts).FirstOrDefault();
        }
    }
}
