using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Areas.Admin.Models;
using Application.Infrastructure.Mapping;
using Microsoft.AspNetCore.Mvc;
using Models;
using Services.Contracts;

namespace Application.Areas.Admin.Controllers
{
    public class OrdersController: AdminAreaController
    {
        private readonly IOrdersService ordersService;

        public OrdersController(IOrdersService ordersService)
        {
            this.ordersService = ordersService;
        }

        public IActionResult All()
        {
            var model = this.ordersService.GetAllOrders().Select(o => o.Map<Order, AllOrdersViewModel>()).ToList();

            return View(model);
        }
    }
}
