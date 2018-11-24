using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Models;
using Models.Enums;

namespace Services.Contracts
{
    public interface IProductsService
    {
        int SearchedProductsCount { get; }

        void CreateProduct(Product product, string username);

        IQueryable<Product> GetLatestProducts();

        IQueryable<Product> GetMostViewedProducts();

        IQueryable<Product> GetMostOrderedProducts();

        IQueryable<Product> GetTopProduct();

        IQueryable<Product> GetProductsByCategory(Guid categoryId, int skip, int take, decimal minPrice, decimal maxPrice, Guid sizeId, Sex sex);
    }
}
