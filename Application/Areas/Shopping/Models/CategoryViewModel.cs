using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Infrastructure.Mapping;
using Models;

namespace Application.Areas.Shopping.Models
{
    public class CategoryViewModel: IMapFrom<Category>
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Image { get; set; }
    }
}
