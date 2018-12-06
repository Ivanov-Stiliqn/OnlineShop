using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Areas.Profile.Models;
using Application.Areas.Shopping.Controllers;
using Application.Infrastructure.Mapping;
using Microsoft.AspNetCore.Mvc;
using Models;
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

        public IActionResult SellOrders()
        {
            var orders = this.ordersService.GetSellOrders(this.User.Identity.Name);
            var model = orders.Select(o => o.Map<Order, OrderViewModel>()).ToList();

            return View("Index", model);
        }

        public IActionResult PurchaseOrders()
        {
            var orders = this.ordersService.GetPurchaseOrders(this.User.Identity.Name);
            var model = orders.Select(o => o.Map<Order, OrderViewModel>()).ToList();

            return View("Index", model);
        }

        public async Task<IActionResult> Accept(string id)
        {
            var check = await this.ordersService.AcceptOrder(id, this.User.Identity.Name);
            if (!check)
            {
                this.TempData["Error"] = "Invalid operation.";
                return RedirectToAction(nameof(SellOrders));
            }

            this.TempData["Success"] = "Order accepted successfully, please check the order details to send.";
            return RedirectToAction(nameof(Details), new {id});
        }

        public async Task<IActionResult> Receive(string id, string productId)
        {
            var check = await this.ordersService.ReceiveOrder(id, this.User.Identity.Name);
            if (!check)
            {
                this.TempData["Error"] = "Invalid operation.";
                return RedirectToAction(nameof(PurchaseOrders));
            }

            this.TempData["Success"] = "Thank you for your purchase, please review the product.";
            return RedirectToAction(nameof(ProductsController.Details), "Products", new { area = "Shopping", id = productId });
        }

        public IActionResult Details(string id)
        {
            return View();
        }
    }
}
