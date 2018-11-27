using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Infrastructure.Mapping;
using Models;
using Models.Enums;

namespace Application.Areas.Shopping.Models
{
    public class ProductDetailsViewModel: IMapFrom<Product>, ICustomMapping
    {
        public string Name { get; set; }

        public decimal Price { get; set; }

        public string Details { get; set; }

        public string Description { get; set; }

        public string Color { get; set; }

        public Sex Sex { get; set; }

        public int Views { get; set; }

        public int Orders { get; set; }

        public ICollection<string> Images { get; set; }

        public ICollection<SizeViewModel> Sizes { get; set; }

        public void ConfigureMapping(AutoMapper.Profile profile)
            => profile.CreateMap<Product, ProductDetailsViewModel>()
                .ForMember(p => p.Sizes, cfg => cfg.MapFrom(p => p.Sizes.Select(s => new SizeViewModel
                {
                    Id = s.Size.Id,
                    Quantity = s.Quantity,
                    Name = s.Size.Name
                })))
            .ForMember(p => p.Orders, cfg => cfg.MapFrom(p => p.Orders.Count));
    }
}
