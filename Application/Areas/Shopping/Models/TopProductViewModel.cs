using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Infrastructure.Mapping;
using Models;

namespace Application.Areas.Shopping.Models
{
    public class TopProductViewModel: IMapFrom<Product>, ICustomMapping
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Image { get; set; }

        public string DescriptionWords =>
            string.Join(" ", this.Description.Split(" ", StringSplitOptions.RemoveEmptyEntries).Take(20)) + "...";

        public void ConfigureMapping(AutoMapper.Profile profile)
            => profile
                .CreateMap<Product, TopProductViewModel>()
                .ForMember(p => p.Image, cfg => cfg.MapFrom(p => p.Images.First()));
    }
}
