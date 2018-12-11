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
        public async Task SubmitReviewShouldBeCalledOnce()
        {
            var reviewRepo = new Mock<IRepository<Review>>();
            var usersRepo = new Mock<IRepository<User>>();
            var users = new List<User>
            {
                new User {UserName = "stamat"},
                new User {UserName = "gosho"},
            };

            usersRepo.Setup(u => u.All()).Returns(users.AsQueryable());

            var reviews = new List<Review>();
            reviewRepo.Setup(r => r.All()).Returns(reviews.AsQueryable());
            reviewRepo.Setup(r => r.AddAsync(It.IsAny<Review>())).Returns<Review>(Task.FromResult).Callback<Review>(r => reviews.Add(r));

            var service = new ReviewService(reviewRepo.Object, usersRepo.Object);
            var review = new Review {Id = Guid.NewGuid()};

            await service.Create(review, "stamat");

            Assert.Equal(1, reviews.Count);
            Assert.Contains(reviews, r => r.Id == review.Id);
            reviewRepo.Verify(r => r.AddAsync(review), Times.Once);
        }

        [Fact]
        public async Task SubmitReviewShouldThrowErrorForNonExistingUser()
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
