using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Areas.Shopping.Models
{
    public class HomeViewModel
    {
        public ICollection<ProductViewModel> Latest { get; set; }

        public ICollection<ProductViewModel> MostViewed { get; set; }

        public ICollection<ProductViewModel> MostOrderedProducts { get; set; }

        public TopProductViewModel TopProduct { get; set; }

        public ICollection<CategoryViewModel> TopCategories { get; set; }
    }
}
