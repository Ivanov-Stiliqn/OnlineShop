using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Infrastructure.Mapping;
using Models;

namespace Application.Areas.Admin.Models
{
    public class AllOrdersViewModel: IMapFrom<Order>
    {
        public Guid Id { get; set; }

        public Guid ProductId { get; set; }

        public string Buyer { get; set; }

        public string Seller { get; set; }

        public string ProductName { get; set; }

        public string ProductImage { get; set; }

        public bool IsAccepted { get; set; }

        public bool IsDelivered { get; set; }

        public int Quantity { get; set; }

        public DateTime DateOfCreation { get; set; }
    }
}
