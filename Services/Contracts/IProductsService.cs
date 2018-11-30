using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using Models.Enums;

namespace Services.Contracts
{
    public interface IProductsService
    {
        Task CreateProduct(Product product, string username);

        ICollection<Product> GetLatestProducts();

        ICollection<Product> GetMostViewedProducts();

        ICollection<Product> GetMostOrderedProducts();

        Product GetTopProduct();

        Task<Product> GetProduct(Guid id);
    }
}
