using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Infrastructure.Mapping;
using Models;

namespace Application.Areas.Profile.Models
{
    public class OrderViewModel: IMapFrom<Order>, ICustomMapping
    {
        public Guid Id { get; set; }

        public Guid ProductId { get; set; }

        public string ProductName { get; set; }

        public string ProductImage { get; set; }

        public string Buyer { get; set; }

        public string Seller { get; set; }

        public DateTime DateOfCreation { get; set; }

        public string Size { get; set; }

        public int Quantity { get; set; }

        public bool IsAccepted { get; set; }

        public bool IsDelivered { get; set; }

        public void ConfigureMapping(AutoMapper.Profile profile)
            => profile.CreateMap<Order, OrderViewModel>()
                .ForMember(o => o.Buyer, cfg => cfg.MapFrom(o => o.Buyer.UserName))
                .ForMember(o => o.Seller, cfg => cfg.MapFrom(o => o.Seller.UserName));
    }
}