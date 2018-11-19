using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Infrastructure.Mapping;
using Models;

namespace Application.Areas.Admin.Models
{
    public class UserViewModel: IMapFrom<User>
    {
        public string UserName { get; set; }

        public string Email { get; set; }

        public string Id { get; set; }
    }
}
