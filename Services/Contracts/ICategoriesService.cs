using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Models;

namespace Services.Contracts
{
    public interface ICategoriesService
    {
        bool Create(Category category);

        IQueryable<Category> GetCategories();

        IQueryable<Category> GetTopCategories();
    }
}
