using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Data;
using Models;
using Services.Contracts;

namespace Services
{
    public class ReportsService: IReportsService
    {
        private readonly ApplicationContext db;

        public ReportsService(ApplicationContext db)
        {
            this.db = db;
        }

        public IQueryable<Report> GetReports()
        {
            return this.db.Reports.OrderByDescending(o => o.DateOfCreation);
        }
    }
}
