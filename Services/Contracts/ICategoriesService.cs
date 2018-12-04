using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using Models.Enums;

namespace Services.Contracts
{
    public interface ICategoriesService
    {
        int SearchedProductsCount { get; }

        Task<bool> Create(Category category);

        ICollection<Category> GetCategories();

        ICollection<Category> GetTopCategories();

        Category GetCategory(string id);

        Task EditCategory(Category category);

        Task<bool> DeleteCategory(string id);

        Task<ICollection<Product>> GetProductsByCategory(
            Guid categoryId, 
            int skip, 
            int take, 
            decimal minPrice, 
            decimal maxPrice, 
            Guid sizeId, 
            Sex sex);
    }
}
