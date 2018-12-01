using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace Services.Contracts
{
    public interface IReviewService
    {
        Task Create(Review review, string username);
    }
}
