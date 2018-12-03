using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Application.Infrastructure.Mapping;
using Microsoft.AspNetCore.Http;
using Models;
using Models.Enums;

namespace Application.Areas.Shopping.Models
{
    public class EditProductViewModel: IMapTo<Product>, IMapFrom<Product>
    {
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public Sex Sex { get; set; }

        [Required]
        [Display(Name = "Category")]
        public Guid CategoryId { get; set; }

        [Required]
        public string Color { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Details { get; set; }

        [Range(1.0, double.MaxValue, ErrorMessage = "Price should be a positive number.")]
        public decimal Price { get; set; }
    }
}
