using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Infrastructure.Mapping;
using Models;

namespace Application.Areas.Shopping.Models
{
    public class SizeViewModel: IMapFrom<ProductSize>, ICustomMapping
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int Quantity { get; set; }

        public void ConfigureMapping(AutoMapper.Profile profile)
            => profile.CreateMap<ProductSize, SizeViewModel>()
                .ForMember(s => s.Name, cfg => cfg.MapFrom(p => p.Size.Name))
                .ForMember(s => s.Id, cfg => cfg.MapFrom(p => p.Size.Id));
    }
}
