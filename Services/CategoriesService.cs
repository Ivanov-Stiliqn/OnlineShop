using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using Data.Repositories;
using Data.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Enums;
using Services.Contracts;

namespace Services
{
    public class CategoriesService: ICategoriesService
    {
        private readonly IRepository<Category> categoryRepository;

        public CategoriesService(IRepository<Category> categoryRepository)
        {
            this.categoryRepository = categoryRepository;
        }

        public int SearchedProductsCount { get; private set; }

        public async Task<bool> Create(Category category)
        {
            var existingCategory = this.categoryRepository.All().FirstOrDefault(c => c.Name == category.Name);
            if (existingCategory != null)
            {
                return false;
            }

            await this.categoryRepository.AddAsync(category);
            await this.categoryRepository.SaveChangesAsync();
            return true;
        }

        public ICollection<Category> GetCategories()
        {
            return this.categoryRepository.All().ToList();
        }

        public ICollection<Category> GetTopCategories()
        {
            return this.categoryRepository.All().OrderByDescending(c => c.Views).Take(2).ToList();
        }

        public async Task<ICollection<Product>> GetProductsByCategory(Guid categoryId, int skip, int take, decimal minPrice, decimal maxPrice, Guid sizeId, Sex sex)
        {
            var category = this.categoryRepository.All()
                .Include(c => c.Products)
                .ThenInclude(c => c.Sizes)
                .FirstOrDefault(c => c.Id == categoryId);

            category.Views += 1;

            var products = category.Products;
            if (minPrice != 0.0m && maxPrice != 0.0m)
            {
                products = products.Where(p => p.Price >= minPrice && p.Price <= maxPrice).ToList();
            }

            if (sizeId != Guid.Empty)
            {
                products = products.Where(p => p.Sizes.FirstOrDefault(s => s.SizeId == sizeId) != null).ToList();
            }

            if (sex != 0)
            {
                products = products.Where(p => p.Sex == sex).ToList();
            }

            this.SearchedProductsCount = products.Count();

            await this.categoryRepository.SaveChangesAsync();
            return products.Skip(skip).Take(take).ToList();
        }

        public Category GetCategory(string id)
        {
            var check = Guid.TryParse(id, out Guid parsedId);
            if (!check)
            {
                return null;
            }

            return this.categoryRepository.All().FirstOrDefault(c => c.Id == parsedId);
        }

        public async Task EditCategory(Category category)
        {
            var categoryFromDb = this.categoryRepository.All().FirstOrDefault(c => c.Id == category.Id);
            categoryFromDb.Name = category.Name;

            if (!string.IsNullOrEmpty(category.Image))
            {
                categoryFromDb.Image = category.Image;
            }

            this.categoryRepository.Update(categoryFromDb);
            await this.categoryRepository.SaveChangesAsync();
        }

        public async Task<bool> DeleteCategory(string id)
        {
            var check = Guid.TryParse(id, out Guid parsedId);
            if (!check)
            {
                return false;
            }

            var category = this.categoryRepository.All().FirstOrDefault(c => c.Id == parsedId);
            if (category == null)
            {
                return false;
            }

            this.categoryRepository.Delete(category);
            await this.categoryRepository.SaveChangesAsync();

            return true;
        }
    }
}
