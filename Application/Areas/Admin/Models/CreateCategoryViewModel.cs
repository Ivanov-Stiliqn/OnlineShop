using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Application.Infrastructure.Mapping;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Models;

namespace Application.Areas.Admin.Models
{
    public class CreateCategoryViewModel: IMapTo<Category>
    {
        [Required]
        [Display(Name = "Category name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please upload a image.")]
        public IFormFile ImageFile { get; set; }
    }
}
