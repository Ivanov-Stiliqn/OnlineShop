﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Models;

namespace Application.Areas.Shopping.Models
{
    public class CategoryProductsViewModel
    {
        public ICollection<ProductViewModel> Products { get; set; }

        public ICollection<ProductViewModel> MostOrderedProducts { get; set; }

        public ICollection<CategoryListItemViewModel> Categories { get; set; }

        public CategoryListItemViewModel CurrentCategory { get; set; }

        public ICollection<SizeListItemViewModel> Sizes { get; set; }

        public Pagination Pagination { get; set; }
    }
}
