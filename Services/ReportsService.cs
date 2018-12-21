using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private readonly IRepository<User> usersRepository;

        public ReportsService(IRepository<Report> reportRepository, IRepository<User> usersRepository)
        {
            this.reportRepository = reportRepository;
            this.usersRepository = usersRepository;
        }

        public ICollection<Report> GetReports()
        {
            return this.reportRepository.All()
                .Include(r => r.Reporter)
                .Include(r => r.ReportedUser)
                .OrderByDescending(o => o.DateOfCreation)
                .ToList();
        }

        public async Task SubmitReport(Report report, string reporterName)
        {
            var reporter = this.usersRepository.All().FirstOrDefault(u => u.UserName == reporterName);
            report.ReporterId = reporter.Id;

            await this.reportRepository.AddAsync(report);
            await this.reportRepository.SaveChangesAsync();
        }
    }
}
