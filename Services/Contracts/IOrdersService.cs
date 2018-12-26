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

        Task<ICollection<Order>> GetSellOrders(string username);

        Task<ICollection<Order>> GetPurchaseOrders(string username);

        Task<bool> ReceiveOrder(string id, string username);

        Task<bool> AcceptOrder(string id, string username);

        Order GetOrderDetails(string id);

        ICollection<Order> GetAllOrders();

        bool IsThereUnSeenPurchaseOrders(string username);

        bool IsThereUnSeenSellOrders(string username);
    }
}
