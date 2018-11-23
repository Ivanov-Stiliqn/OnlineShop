using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Models;

namespace Services.Contracts
{
    public interface IProductsService
    {
        void CreateProduct(Product product, string username);

        IQueryable<Product> GetLatestProducts();

        IQueryable<Product> GetMostViewedProducts();

        IQueryable<Product> GetMostOrderedProducts();

        IQueryable<Product> GetTopProduct();

        IQueryable<Product> GetProductsByCategory(Guid categoryId, int skip, int take);

        int ProductsCount(Guid categoryId);
    }
}
