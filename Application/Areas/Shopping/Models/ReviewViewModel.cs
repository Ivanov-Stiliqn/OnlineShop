using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Infrastructure.Mapping;
using Models;

namespace Application.Areas.Shopping.Models
{
    public class ReviewViewModel: IMapFrom<Review>, ICustomMapping
    {
        public string User { get; set; }

        public string Comment { get; set; }

        public int Stars { get; set; }

        public string Image { get; } = "https://upload.wikimedia.org/wikipedia/commons/thumb/1/12/User_icon_2.svg/768px-User_icon_2.svg.png";

        public void ConfigureMapping(AutoMapper.Profile profile)
            => profile.CreateMap<Review, ReviewViewModel>()
                .ForMember(r => r.User, cfg => cfg.MapFrom(r => r.User.UserName));
    }
}
