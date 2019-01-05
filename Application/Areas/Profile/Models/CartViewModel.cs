using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Application.Areas.Shopping.Models;
using Application.Infrastructure.Mapping;
using Models;
using Models.Enums;

namespace Application.Areas.Profile.Models
{
    public class CartViewModel: IMapFrom<Product>, ICustomMapping
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public string Image { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity should be positive number")]
        public int Quantity { get; set; }

        [Required]
        public string Size { get; set; }

        public void ConfigureMapping(AutoMapper.Profile profile)
            => profile
                .CreateMap<Product, CartViewModel>()
                .ForMember(p => p.Image, cfg => cfg.MapFrom(p => p.Images.First()));
    }
}
