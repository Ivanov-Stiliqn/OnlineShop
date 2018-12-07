using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Repositories;
using Data.Repositories.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models;
using Services.Contracts;

namespace Services
{
    public class OrdersService : IOrdersService
    {
        private readonly IRepository<Order> ordersRepository;
        private readonly IRepository<ProductSize> productSizeRepository;
        private readonly IRepository<User> usersRepository;
        private readonly IRepository<Product> productsRepository;

        public OrdersService(
            IRepository<Order> ordersRepository,
            IRepository<ProductSize> productSizeRepository,
            IRepository<User> usersRepository,
            IRepository<Product> productsRepository)
        {
            this.ordersRepository = ordersRepository;
            this.productSizeRepository = productSizeRepository;
            this.usersRepository = usersRepository;
            this.productsRepository = productsRepository;
        }

        public async Task CreateOrders(ICollection<Order> orders, string username)
        {
            var user = this.usersRepository.All().FirstOrDefault(u => u.UserName == username);

            foreach (var order in orders)
            {
                var product = this.productsRepository
                    .All()
                    .Include(p => p.Sizes)
                    .ThenInclude(ps => ps.Size)
                    .FirstOrDefault(p => p.Id == order.ProductId);

                if (product.CreatorId == user.Id)
                {
                    throw new InvalidOperationException("You cannot order your own product.");
                }

                order.BuyerId = user.Id;
                order.SellerId = product.CreatorId;

                var productSize = product.Sizes.FirstOrDefault(ps => ps.Size.Name == order.Size);
                if (productSize.Quantity < order.Quantity)
                {
                    throw new InvalidOperationException(
                        "Sorry you are too late, current product size is out of stock.");
                }
            }

            await this.ordersRepository.AddRangeAsync(orders);
            await this.ordersRepository.SaveChangesAsync();
        }

        public ICollection<Order> GetSellOrders(string username)
        {
            var user = this.usersRepository
                .All()
                .Include(p => p.SellOrders)
                    .ThenInclude(o => o.Product)
                .Include(p => p.SellOrders)
                    .ThenInclude(o => o.Buyer)
                .FirstOrDefault(u => u.UserName == username);

            return user.SellOrders.ToList();
        }

        public async Task<bool> AcceptOrder(string id, string username)
        {
            var check = Guid.TryParse(id, out Guid parsedid);
            if (!check)
            {
                return false;
            }

            var order = this.ordersRepository.All().Include(o => o.Seller).FirstOrDefault(o => o.Id == parsedid);
            if (order == null)
            {
                return false;
            }

            if (order.Seller.UserName != username)
            {
                return false;
            }

            order.IsAccepted = true;
            await this.ordersRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ReceiveOrder(string id, string username)
        {
            var check = Guid.TryParse(id, out Guid parsedid);
            if (!check)
            {
                return false;
            }

            var order = this.ordersRepository.All().Include(o => o.Buyer).FirstOrDefault(o => o.Id == parsedid);
            if (order == null)
            {
                return false;
            }

            if (order.Buyer.UserName != username || !order.IsAccepted)
            {
                return false;
            }

            order.IsDelivered = true;
            await this.ordersRepository.SaveChangesAsync();
            return true;
        }

        public ICollection<Order> GetPurchaseOrders(string username)
        {
            var user = this.usersRepository
                .All()
                .Include(p => p.PurchaseOrders)
                .ThenInclude(o => o.Product)
                .Include(p => p.PurchaseOrders)
                .ThenInclude(o => o.Seller)
                .FirstOrDefault(u => u.UserName == username);

            return user.PurchaseOrders.Where(o => o.IsAccepted).ToList();
        }

        public Order GetOrderDetails(string id)
        {
            var check = Guid.TryParse(id, out Guid parsedId);
            if (!check)
            {
                return null;
            }

            return this.ordersRepository
                .All()
                .Include(o => o.Seller)
                .Include(o => o.Buyer)
                .ThenInclude(b => b.UserInfo)
                .FirstOrDefault(o => o.Id == parsedId);
        }

        public ICollection<Order> GetAllOrders()
        {
            return this.ordersRepository
                .All()
                .Include(o => o.Buyer)
                .Include(o => o.Seller)
                .ToList();
        }
    }
}