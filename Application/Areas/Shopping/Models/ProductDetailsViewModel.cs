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
        public Guid Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public string Details { get; set; }

        public string Description { get; set; }

        public string Color { get; set; }

        public Sex Sex { get; set; }

        public int Views { get; set; }

        public int Orders { get; set; }

        public Guid CategoryId { get; set; }

        public string Creator { get; set; }

        public ICollection<ReviewViewModel> Reviews { get; set; }

        public decimal Rating => this.Reviews.Count > 0
            ? Math.Ceiling((decimal) this.Reviews.Select(r => r.Stars).Sum() / this.Reviews.Count)
            : 0.0m;

        public ICollection<string> Images { get; set; }

        public ICollection<SizeViewModel> Sizes { get; set; }

        public void ConfigureMapping(AutoMapper.Profile profile)
            => profile.CreateMap<Product, ProductDetailsViewModel>()
                .ForMember(p => p.Sizes, cfg => cfg.MapFrom(p => p.Sizes.Where(s => s.Quantity > 0).Select(s => s.Map<ProductSize, SizeViewModel>())))
            .ForMember(p => p.Orders, cfg => cfg.MapFrom(p => p.Orders.Count))
            .ForMember(p => p.Reviews, cfg => cfg.MapFrom(p => p.Reviews.Select(s => s.Map<Review, ReviewViewModel>())))
            .ForMember(p => p.Views, cfg => cfg.MapFrom(p => p.ViewsCount));
    }
}
