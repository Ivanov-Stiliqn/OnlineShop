using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Infrastructure.Mapping;
using Models;
using Models.Enums;

namespace Application.Areas.Shopping.Models
{
    public class CategoryListItemViewModel: IMapFrom<Category>
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public CategoryType Type { get; set; }
    }
}
