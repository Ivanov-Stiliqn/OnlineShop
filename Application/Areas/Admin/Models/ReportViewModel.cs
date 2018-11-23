using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Infrastructure.Mapping;
using Models;

namespace Application.Areas.Admin.Models
{
    public class ReportViewModel: IMapFrom<Report>, ICustomMapping
    {
        public string Details { get; set; }

        public string Reporter { get; set; }

        public string Reported { get; set; }

        public string Date { get; set; }

        public void ConfigureMapping(AutoMapper.Profile profile)
            => profile.CreateMap<Report, ReportViewModel>()
                .ForMember(r => r.Date, cfg => cfg.MapFrom(r => r.DateOfCreation.ToShortDateString()))
                .ForMember(r => r.Reporter, cfg => cfg.MapFrom(r => r.Reporter.UserName))
                .ForMember(r => r.Reported, cfg => cfg.MapFrom(r => r.ReportedUser.UserName));
    }
}
