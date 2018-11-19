using System.Linq;
using Application.Infrastructure.Mapping;
using Models;
using System;

namespace Application.Areas.Shopping.Models
{
    public class ProductViewModel: IMapFrom<Product>, ICustomMapping
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public string Image { get; set; }

        public void ConfigureMapping(AutoMapper.Profile profile)
            => profile
                .CreateMap<Product, ProductViewModel>()
                .ForMember(p => p.Image, cfg => cfg.MapFrom(p => p.Images.First()));
    }
}
