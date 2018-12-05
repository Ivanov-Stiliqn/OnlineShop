﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Application.Infrastructure.Mapping;
using Models;

namespace Application.Areas.Admin.Models
{
    public class EditCategoryViewModel: IMapTo<Category>, IMapFrom<Category>
    {
        public Guid Id { get; set; }

        [Required]
        [Display(Name = "Category name")]
        public string Name { get; set; }

        public string Image { get; set; }
    }
}