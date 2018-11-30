using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Data;
using Data.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;
using Models;
using Services.Contracts;

namespace Services
{
    public class ReportsService: IReportsService
    {
        private readonly IRepository<Report> reportRepository;

        public ReportsService(IRepository<Report> reportRepository)
        {
            this.reportRepository = reportRepository;
        }

        public ICollection<Report> GetReports()
        {
            return this.reportRepository.All()
                .Include(r => r.Reporter)
                .Include(r => r.ReportedUser)
                .OrderByDescending(o => o.DateOfCreation)
                .ToList();
        }
    }
}
