using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Application.Infrastructure.Mapping;
using Models;

namespace Application.Areas.Shopping.Models
{
    public class ProductDetailsPageViewModel: IMapTo<Review>
    {
        public ProductDetailsViewModel Product { get; set; }

        public ICollection<ProductViewModel> MostOrderedProducts { get; set; }

        [Required(ErrorMessage = "You cannot submit empty review.")]
        public string Comment { get; set; }

        [Range(1, 5, ErrorMessage = "Invalid stars.")]
        public int Stars { get; set; }
    }
}
