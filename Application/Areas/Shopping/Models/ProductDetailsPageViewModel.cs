using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Areas.Shopping.Models
{
    public class ProductDetailsPageViewModel
    {
        public ProductDetailsViewModel Product { get; set; }

        public ICollection<ProductViewModel> MostOrderedProducts { get; set; }
    }
}
