using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace Services.Contracts
{
    public interface IOrdersService
    {
        Task CreateOrders(ICollection<Order> orders, string username);

        ICollection<Order> GetPendingOrders(string username);
    }
}
