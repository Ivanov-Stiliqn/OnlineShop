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
    public class ProductsService: IProductsService
    {
        private readonly ApplicationContext db;

        public ProductsService(ApplicationContext db)
        {
            this.db = db;
        }

        public void CreateProduct(Product product, string username)
        {
            var user = this.db.Users.FirstOrDefault(u => u.UserName == username);
            product.CreatorId = user.Id;

            this.db.Products.Add(product);
            this.db.SaveChanges();
        }

        public IQueryable<Product> GetLatestProducts()
        {
            return this.db.Products.OrderByDescending(p => p.DateOfCreation).Take(8);
        }

        public IQueryable<Product> GetMostViewedProducts()
        {
            return this.db.Products.OrderByDescending(p => p.Views).Take(8);
        }

        public IQueryable<Product> GetMostOrderedProducts()
        {
            return this.db.Products.Include(p => p.Orders).OrderByDescending(p => p.Orders.Count).Take(12);
        }

        public IQueryable<Product> GetTopProduct()
        {
            return this.db.Products.Include(p => p.Orders).OrderByDescending(p => p.Orders.Count).Take(1);
        }
    }
}
