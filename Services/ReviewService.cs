using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Repositories.Contracts;
using Models;
using Services.Contracts;

namespace Services
{
    public class ReviewService: IReviewService
    {
        private readonly IRepository<Review> reviewRepository;
        private readonly IRepository<User> usersRepository;

        public ReviewService(IRepository<Review> reviewRepository, IRepository<User> usersRepository)
        {
            this.reviewRepository = reviewRepository;
            this.usersRepository = usersRepository;
        }

        public async Task Create(Review review, string username)
        {
            var user = this.usersRepository.All().FirstOrDefault(u => u.UserName == username);

            review.UserId = user.Id;
            await this.reviewRepository.AddAsync(review);
            await this.reviewRepository.SaveChangesAsync();
        }
    }
}
