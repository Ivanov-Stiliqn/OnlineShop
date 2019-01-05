using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Application.Infrastructure.Mapping;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Models;
using Models.Enums;

namespace Application.Areas.Admin.Models
{
    public class CreateCategoryViewModel: IMapTo<Category>
    {
        [Required, 
            MinLength(3, ErrorMessage = "Name should be at least 3 symbols long"), 
            MaxLength(20, ErrorMessage = "Name should be no more than 20 symbols long")]
        [Display(Name = "Category name")]
        public string Name { get; set; }

        [Required]
        public CategoryType Type { get; set; }

        [Required(ErrorMessage = "Please upload a image.")]
        public IFormFile ImageFile { get; set; }
    }
}
