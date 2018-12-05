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
    public class OrdersService: IOrdersService
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

                order.UserId = user.Id;

                var productSize = product.Sizes.FirstOrDefault(ps => ps.Size.Name == order.Size);
                if (productSize.Quantity < order.Quantity)
                {
                    throw new InvalidOperationException("Sorry you are too late, current product size is out of stock.");
                }
            }

            await this.ordersRepository.AddRangeAsync(orders);
            await this.ordersRepository.SaveChangesAsync();
        }

        public ICollection<Order> GetPendingOrders(string username)
        {
            var user = this.usersRepository
                .All()
                .Include(u => u.MyProducts)
                .ThenInclude(p => p.Orders)
                .FirstOrDefault(u => u.UserName == username);

           return user.MyProducts.SelectMany(p => p.Orders).Where(o => !o.IsAccepted).ToList();
        }
    }
}
