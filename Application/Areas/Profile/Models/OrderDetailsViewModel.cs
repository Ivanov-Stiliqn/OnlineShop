using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Infrastructure.Mapping;
using Models;

namespace Application.Areas.Profile.Models
{
    public class OrderDetailsViewModel: IMapFrom<Order>, ICustomMapping
    {
        public UserInfoViewModel UserInfo { get; set; }

        public Guid Id { get; set; }

        public string ProductName { get; set; }

        public decimal ProductPrice { get; set; }

        public DateTime DateOfCreation { get; set; }

        public string Size { get; set; }

        public int Quantity { get; set; }

        public string Buyer { get; set; }

        public string Seller { get; set; }

        public string BuyerId { get; set; }

        public string SellerId { get; set; }

        public void ConfigureMapping(AutoMapper.Profile profile)
            => profile.CreateMap<Order, OrderDetailsViewModel>()
                .ForMember(o => o.UserInfo,
                    cfg => cfg.MapFrom(o => o.Buyer.UserInfo.Map<UserInfo, UserInfoViewModel>()));
    }
}
