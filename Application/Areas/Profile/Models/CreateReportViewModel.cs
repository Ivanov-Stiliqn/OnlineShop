using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Application.Infrastructure.Mapping;
using Models;

namespace Application.Areas.Profile.Models
{
    public class CreateReportViewModel: IMapTo<Report>
    {
        public string ReporterId { get; set; }

        public string ReportedUserId { get; set; }

        [Required(ErrorMessage = "Cannot submit empty report."), 
            MinLength(5, ErrorMessage = "Report should be at least 5 symbols long"), 
            MaxLength(200, ErrorMessage = "Report should be no more than 200 symbols long")]
        public string Details { get; set; }
    }
}
