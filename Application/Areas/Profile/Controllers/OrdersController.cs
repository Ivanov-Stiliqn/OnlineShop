using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;

namespace Application.Areas.Profile.Controllers
{
    public class OrdersController: ProfileAreaController
    {
        private readonly IOrdersService ordersService;

        public OrdersController(IOrdersService ordersService)
        {
            this.ordersService = ordersService;
        }

        public IActionResult Pending()
        {
            var orders = this.ordersService.GetPendingOrders(this.User.Identity.Name);
            return null;
        }
    }
}
