using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Areas.Shopping.Models;
using Application.Infrastructure.Mapping;
using Models;

namespace Application.Areas.Profile.Models
{
    public class WishlistViewModel: IMapFrom<Product>, ICustomMapping
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public string Image { get; set; }

        public ICollection<SizeViewModel> Sizes { get; set; }

        public void ConfigureMapping(AutoMapper.Profile profile)
            => profile
                .CreateMap<Product, WishlistViewModel>()
                .ForMember(p => p.Image, cfg => cfg.MapFrom(p => p.Images.First()))
                .ForMember(p => p.Sizes, cfg => cfg.MapFrom(p => p.Sizes.Select(s => s.Map<ProductSize, SizeViewModel>())));
    }
}
