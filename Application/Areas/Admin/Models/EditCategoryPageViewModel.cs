using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Application.Areas.Admin.Models
{
    public class EditCategoryPageViewModel
    {
        public IFormFile ImageFile { get; set; }

        public EditCategoryViewModel Category { get; set; }
    }
}
