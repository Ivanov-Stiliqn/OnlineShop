using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Models;

namespace Services.Contracts
{
    public interface IReportsService
    {
        IQueryable<Report> GetReports();
    }
}
