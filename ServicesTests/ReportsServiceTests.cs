using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Repositories.Contracts;
using Models;
using Moq;
using Services;
using Xunit;

namespace ServicesTests
{
    public class ReportsServiceTests
    {
        [Fact]
        public void GetReportsShouldReturnAllReports()
        {
            var reportRepo = new Mock<IRepository<Report>>();
            reportRepo.Setup(r => r.All()).Returns(new List<Report>
            {
                new Report
                {
                    Reporter = new User(),
                    ReportedUser = new User(),

                },
                new Report
                {
                    Reporter = new User(),
                    ReportedUser = new User(),

                },
                new Report
                {
                    Reporter = new User() { UserName = "test" },
                    ReportedUser = new User(),

                },
            }.AsQueryable());

            var service = new ReportsService(reportRepo.Object, null);
            var reports = service.GetReports();
            Assert.Equal(3, reports.Count);
            reportRepo.Verify(r => r.All(), Times.Once);
        }

        [Fact]
        public void GetReportsShouldReturnAllReportsOrderedDescendingByDate()
        {
            var reportRepo = new Mock<IRepository<Report>>();
            reportRepo.Setup(r => r.All()).Returns(new List<Report>
            {
                new Report
                {
                    Reporter = new User(),
                    ReportedUser = new User(),
                    DateOfCreation = DateTime.Now.AddDays(1),
                    Details = "3"

                },
                new Report
                {
                    Reporter = new User(),
                    ReportedUser = new User(),
                    DateOfCreation = DateTime.Now.AddDays(3),
                    Details = "1"

                },
                new Report
                {
                    Reporter = new User() { UserName = "test" },
                    ReportedUser = new User(),
                    DateOfCreation = DateTime.Now.AddDays(2),
                    Details = "2"

                },
            }.AsQueryable());

            var service = new ReportsService(reportRepo.Object, null);
            var reports = service.GetReports().ToList();
            Assert.Equal(3, reports.Count);
            reportRepo.Verify(r => r.All(), Times.Once);

            Assert.Equal("1", reports.First().Details);
            Assert.Equal("2", reports[1].Details);
            Assert.Equal("3", reports.Last().Details);
        }

        [Fact]
        public async Task SubmitReportShouldBeCalledOnce()
        {
            var reportRepo = new Mock<IRepository<Report>>();
            var usersRepo = new Mock<IRepository<User>>();
            usersRepo.Setup(u => u.All()).Returns(new List<User>
            {
                new User{ UserName = "stamat" },
                new User{ UserName = "gosho" },
            }.AsQueryable());

            var reports = new List<Report>();
            reportRepo.Setup(r => r.All()).Returns(reports.AsQueryable());
            reportRepo.Setup(r => r.AddAsync(It.IsAny<Report>())).Returns<Report>(Task.FromResult)
                .Callback<Report>(r => reports.Add(r));

            var service = new ReportsService(reportRepo.Object, usersRepo.Object);
            var report = new Report
            {
                Id = Guid.NewGuid(),
                Reporter = new User(),
                ReportedUser = new User(),
                DateOfCreation = DateTime.Now.AddDays(3),
                Details = "1"

            };

            await service.SubmitReport(report, "stamat");

            Assert.Equal(1, reports.Count);
            Assert.Contains(reports, r => r.Id == report.Id);
            reportRepo.Verify(r => r.AddAsync(report), Times.Once);
        }

        [Fact]
        public async Task SubmitReportShouldThrowErrorForNonExistingUser()
        {
            var reportRepo = new Mock<IRepository<Report>>();
            var usersRepo = new Mock<IRepository<User>>();
            usersRepo.Setup(u => u.All()).Returns(new List<User>
            {
                new User{ UserName = "stamat" },
                new User{ UserName = "gosho" },
            }.AsQueryable());

            var service = new ReportsService(reportRepo.Object, usersRepo.Object);
            var report = new Report
            {
                Reporter = new User(),
                ReportedUser = new User(),
                DateOfCreation = DateTime.Now.AddDays(3),
                Details = "1"

            };

            await Assert.ThrowsAsync<NullReferenceException>(async () => await service.SubmitReport(report, "pesho"));
            reportRepo.Verify(r => r.AddAsync(report), Times.Never);
        }
    }
}
