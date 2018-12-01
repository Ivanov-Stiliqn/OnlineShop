using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using Data.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Enums;
using Services.Contracts;

namespace Services
{
    public class ProductsService: IProductsService
    {
        private readonly IRepository<Product> productsRepository;
        private readonly IRepository<User> usersRepository;

        public ProductsService(
            IRepository<Product> productsRepository, 
            IRepository<User> usersRepository)
        {
            this.productsRepository = productsRepository;
            this.usersRepository = usersRepository;
        }

        public async Task CreateProduct(Product product, string username)
        {
            var user = this.usersRepository.All().FirstOrDefault(u => u.UserName == username);
            product.CreatorId = user.Id;

            await this.productsRepository.AddAsync(product);
            await this.productsRepository.SaveChangesAsync();
        }

        public ICollection<Product> GetLatestProducts()
        {
            return this.productsRepository.All().OrderByDescending(p => p.DateOfCreation).Take(8).ToList();
        }

        public ICollection<Product> GetMostViewedProducts()
        {
            return this.productsRepository.All().OrderByDescending(p => p.Views).Take(8).ToList();
        }

        public ICollection<Product> GetMostOrderedProducts()
        {
            return this.productsRepository.All().Include(p => p.Orders).OrderByDescending(p => p.Orders.Count).Take(12).ToList();
        }

        public Product GetTopProduct()
        {
            return this.productsRepository.All().Include(p => p.Orders).OrderByDescending(p => p.Orders.Count).FirstOrDefault();
        }

        public async Task<Product> GetProduct(Guid id)
        {
            var product = this.productsRepository.All()
                .Include(p => p.Orders)
                .Include(p => p.Reviews)
                    .ThenInclude(r => r.User)
                .Include(p => p.Sizes)
                    .ThenInclude(p => p.Size)
                .FirstOrDefault(p => p.Id == id);

            if (product != null)
            {
                product.Views += 1;
                await this.productsRepository.SaveChangesAsync();
            }  

            return product;
        }
    }
}
