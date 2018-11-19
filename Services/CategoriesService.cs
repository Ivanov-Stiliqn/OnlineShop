using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Data;
using Models;
using Services.Contracts;

namespace Services
{
    public class CategoriesService: ICategoriesService
    {
        private readonly ApplicationContext db;

        public CategoriesService(ApplicationContext db)
        {
            this.db = db;
        }

        public bool Create(Category category)
        {
            var existingCategory = this.db.Categories.FirstOrDefault(c => c.Name == category.Name);
            if (existingCategory != null)
            {
                return false;
            }

            this.db.Categories.Add(category);
            this.db.SaveChanges();
            return true;
        }

        public IQueryable<Category> GetCategories()
        {
            return this.db.Categories;
        }

        public IQueryable<Category> GetTopCategories()
        {
            return this.db.Categories.OrderByDescending(c => c.Views).Take(2);
        }
    }
}
