using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Areas.Profile.Models;
using Application.Areas.Shopping.Controllers;
using Application.Infrastructure.Helpers;
using Application.Infrastructure.Mapping;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using Services.Contracts;

namespace Application.Areas.Profile.Controllers
{
    public class CartController: ProfileAreaController
    {
        private readonly IProductsService productsService;
        private readonly IUsersService usersService;
        private readonly IOrdersService ordersService;

        public CartController(
            IProductsService productsService, 
            IUsersService usersService, 
            IOrdersService ordersService)
        {
            this.productsService = productsService;
            this.usersService = usersService;
            this.ordersService = ordersService;
        }

        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartViewModel>>(this.User.Identity.Name) ?? new List<CartViewModel>();
            return View(cart);
        }

        public IActionResult Add(string productId, string size, int quantity)
        {
            var product = this.productsService.GetProductForCart(productId);
            if (product == null)
            {
                this.TempData["Error"] = "Product does not exists.";
                return RedirectToAction(nameof(ProductsController.Index), "Products", new {area = "Shopping"});
            }

            var cartModel = product.Map<Product, CartViewModel>();
            cartModel.Quantity = quantity;
            cartModel.Size = size;

            var cart = HttpContext.Session.GetObjectFromJson<List<CartViewModel>>(this.User.Identity.Name) ?? new List<CartViewModel>();

            cart.Add(cartModel);

            HttpContext.Session.SetObjectAsJson(this.User.Identity.Name, cart);

            this.TempData["Success"] = $"Product {cartModel.Name} added to your cart.";
            return RedirectToAction(nameof(ProductsController.Details), "Products",
                new {area = "Shopping", id = cartModel.Id});
        }

        public IActionResult Remove(string productId, string size)
        {
            var check = Guid.TryParse(productId, out Guid parsedProductId);
            if(!check)
            {
                this.TempData["Error"] = "Product does not exist in cart.";
                return RedirectToAction(nameof(Index));
            }

            var cart = HttpContext.Session.GetObjectFromJson<List<CartViewModel>>(this.User.Identity.Name) ?? new List<CartViewModel>();
            var product = cart.FirstOrDefault(p => p.Id == parsedProductId && p.Size == size);
            cart.Remove(product);

            HttpContext.Session.SetObjectAsJson(this.User.Identity.Name, cart);

            this.TempData["Success"] = "Product removed from cart.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Checkout()
        {
            var userInfo = this.usersService.GetUserInfo(this.User.Identity.Name);
            var cart = HttpContext.Session.GetObjectFromJson<List<CartViewModel>>(this.User.Identity.Name) ?? new List<CartViewModel>();

            if(cart.Count == 0)
            {
                this.TempData["Error"] = "You dont have any products in cart.";
                return RedirectToAction(nameof(ProductsController.Index), "Products", new { area = "Shopping" });
            }

            var model = new CheckoutPageViewModel
            {
                Products = cart,
                UserInfo = userInfo.Map<UserInfo, UserInfoViewModel>(),
                UserInfoId = userInfo?.Id.ToString()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutPageViewModel model)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartViewModel>>(this.User.Identity.Name) ?? new List<CartViewModel>();
            if (!cart.Any())
            {
                this.TempData["Error"] = "Invalid operation.";
                return RedirectToAction(nameof(ProductsController.Index), "Products", new {area = "Shopping"});
            }

            if (!ModelState.IsValid)
            {
                model.Products = cart;

                return View(model);
            }

            if (!model.IsTermsAccepted)
            {
                ModelState.AddModelError("All", "Terms should be accepted.");
                model.Products = cart;

                return View(model);
            }

            var userInfo = model.UserInfo.Map<UserInfoViewModel, UserInfo>();
            if(!string.IsNullOrEmpty(model.UserInfoId))
            {
                var check = Guid.TryParse(model.UserInfoId, out Guid parsedId);
                if(!check)
                {
                    this.TempData["Error"] = "Invalid operation, please try again.";
                    return RedirectToAction(nameof(Checkout));
                }

                userInfo.Id = parsedId;
            }

            await this.usersService.UpdateUserInfo(userInfo, this.User.Identity.Name);

            var orders = new List<Order>();
            foreach (var product in cart)
            {
                orders.Add(new Order
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    ProductImage = product.Image,
                    Size = product.Size,
                    Quantity = product.Quantity,
                    ProductPrice = product.Price
                });
            }

            try
            {
                await this.ordersService.CreateOrders(orders, this.User.Identity.Name);
                HttpContext.Session.SetObjectAsJson(this.User.Identity.Name, new List<CartViewModel>());
                this.TempData["Success"] = "Orders submitted, please wait for confirmation.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException e)
            {
                this.TempData["Error"] = e.Message;
                return RedirectToAction(nameof(Checkout));
            }
        }
    }
}
