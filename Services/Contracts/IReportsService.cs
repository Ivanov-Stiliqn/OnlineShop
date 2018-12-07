using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace Services.Contracts
{
    public interface IReportsService
    {
        ICollection<Report> GetReports();

        Task SubmitReport(Report report, string reporterName);
    }
}
