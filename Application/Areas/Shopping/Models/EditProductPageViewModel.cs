using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Application.Areas.Shopping.Models
{
    public class EditProductPageViewModel
    {
        public ICollection<CategoryListItemViewModel> AllCategories { get; set; }

        public ICollection<IFormFile> ImagesFiles { get; set; }

        public EditProductViewModel Product { get; set; }
    }
}
