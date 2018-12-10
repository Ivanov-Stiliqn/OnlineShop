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
    public class ReviewServiceTests
    {
        [Fact]
        public async Task SubmitReportShouldBeCalledOnce()
        {
            var reviewRepo = new Mock<IRepository<Review>>();
            var usersRepo = new Mock<IRepository<User>>();
            usersRepo.Setup(u => u.All()).Returns(new List<User>
            {
                new User{ UserName = "stamat" },
                new User{ UserName = "gosho" },
            }.AsQueryable());

            var service = new ReviewService(reviewRepo.Object, usersRepo.Object);
            var review = new Review();

            await service.Create(review, "stamat");
            reviewRepo.Verify(r => r.AddAsync(review), Times.Once);
        }

        [Fact]
        public async Task SubmitReportShouldThowErrorForNonExistingUser()
        {
            var reviewRepo = new Mock<IRepository<Review>>();
            var usersRepo = new Mock<IRepository<User>>();
            usersRepo.Setup(u => u.All()).Returns(new List<User>
            {
                new User{ UserName = "stamat" },
                new User{ UserName = "gosho" },
            }.AsQueryable());

            var service = new ReviewService(reviewRepo.Object, usersRepo.Object);
            var review = new Review();

            await Assert.ThrowsAsync<NullReferenceException>(async () => await service.Create(review, "pesho"));
            reviewRepo.Verify(r => r.AddAsync(review), Times.Never);
        }
    }
}
