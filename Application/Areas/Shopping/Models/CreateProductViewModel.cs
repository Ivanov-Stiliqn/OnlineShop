using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using Microsoft.AspNetCore.Http;
using Models;
using Models.Enums;

namespace Application.Areas.Shopping.Models
{
    public class CreateProductViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public Sex Sex { get; set; }

        [Required]
        [Display(Name = "Category")]
        public string CategoryId { get; set; }

        [Required]
        public string Color { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Details { get; set; }

        [Range(1.0, double.MaxValue, ErrorMessage = "Price should be a positive number.")]
        public decimal Price { get; set; }

        public ICollection<CategoryListItemViewModel> AllCategories { get; set; }

        [Required(ErrorMessage = "Please upload at least one image.")]
        public ICollection<IFormFile> Images { get; set; }
    }
}
