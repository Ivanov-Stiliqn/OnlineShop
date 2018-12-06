using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Infrastructure.Mapping;
using Models;

namespace Application.Areas.Admin.Models
{
    public class UserViewModel: IMapFrom<User>, ICustomMapping
    {
        public string UserName { get; set; }

        public string Email { get; set; }

        public string Id { get; set; }

        public int Orders { get; set; }

        public int Products { get; set; }

        public int Reports { get; set; }

        public bool IsRestricted { get; set; }

        public void ConfigureMapping(AutoMapper.Profile profile)
            => profile.CreateMap<User, UserViewModel>()
                .ForMember(u => u.Orders, cfg => cfg.MapFrom(u => u.PurchaseOrders.Count))
                .ForMember(u => u.Products, cfg => cfg.MapFrom(u => u.MyProducts.Count))
                .ForMember(u => u.Reports, cfg => cfg.MapFrom(u => u.Reports.Count));
    }
}
