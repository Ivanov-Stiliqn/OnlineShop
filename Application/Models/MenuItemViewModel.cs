using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Infrastructure.Mapping;
using Models;

namespace Application.Models
{
    public class MenuItemViewModel: IMapFrom<Category>
    {
        public string Name { get; set; }

        public string Id { get; set; }
    }
}
