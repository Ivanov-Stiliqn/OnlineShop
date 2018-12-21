using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using Data.Repositories.Contracts;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<User> manager;

        public ProductsService(
            IRepository<Product> productsRepository, 
            IRepository<User> usersRepository,
            UserManager<User> manager)
        {
            this.productsRepository = productsRepository;
            this.usersRepository = usersRepository;
            this.manager = manager;
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

        public async Task<Product> GetProduct(string id, bool increaseView)
        {
            var check = Guid.TryParse(id, out Guid parsedId);
            if (!check)
            {
                return null;
            }

            var product = this.productsRepository.All()
                .Include(p => p.Creator)
                .Include(p => p.Orders)
                .Include(p => p.Reviews)
                    .ThenInclude(r => r.User)
                .Include(p => p.Sizes)
                    .ThenInclude(p => p.Size)
                .FirstOrDefault(p => p.Id == parsedId);

            if (product != null && increaseView)
            {
                product.Views += 1;
                await this.productsRepository.SaveChangesAsync();
            }  

            return product;
        }

        public Product GetProductForCart(string id)
        {
            var check = Guid.TryParse(id, out Guid parsedId);
            if (!check)
            {
                return null;
            }

            return this.productsRepository.All().FirstOrDefault(p => p.Id == parsedId);
        }

        public async Task<bool> EditProduct(Product model, string username, ICollection<string> images)
        {
            var product = this.productsRepository.All().Include(p => p.Creator).FirstOrDefault(p => p.Id == model.Id);
            if (product.Creator.UserName != username)
            {
                return false;
            }

            product.Name = model.Name;
            product.Price = model.Price;
            product.Color = model.Color;
            product.CategoryId = model.CategoryId;
            product.Description = model.Description;
            product.Details = model.Details;

            var productImages = product.Images.ToList();
            productImages.AddRange(images);
            product.ImageUrls = string.Join(", ", productImages);

            product.Sex = model.Sex;

            await this.productsRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteProduct(string productId, string username)
        {
            var check = Guid.TryParse(productId, out Guid parsedProductId);
            if (!check)
            {
                return false;
            }

            var user = this.usersRepository.All().FirstOrDefault(u => u.UserName == username);
            var isAdmin = await this.manager.IsInRoleAsync(user, "Admin");
            var product = this.productsRepository.All().Include(p => p.Creator).FirstOrDefault(p => p.Id == parsedProductId);
            if (product == null || user.UserName != product.Creator.UserName && !isAdmin)
            {
                return false;
            }

            this.productsRepository.Delete(product);
            await this.productsRepository.SaveChangesAsync();
            return true;
        }
    }
}
