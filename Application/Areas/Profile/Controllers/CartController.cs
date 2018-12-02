using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Areas.Profile.Models;
using Application.Areas.Shopping.Controllers;
using Application.Infrastructure.Helpers;
using Application.Infrastructure.Mapping;
using Microsoft.AspNetCore.Mvc;
using Models;
using Services.Contracts;

namespace Application.Areas.Profile.Controllers
{
    public class CartController: ProfileAreaController
    {
        private readonly IProductsService productsService;
        private readonly IUsersService usersService;

        public CartController(IProductsService productsService, IUsersService usersService)
        {
            this.productsService = productsService;
            this.usersService = usersService;
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

        public IActionResult Remove(string productId)
        {
            var check = Guid.TryParse(productId, out Guid parsedProductId);
            if(!check)
            {
                this.TempData["Error"] = "Product does not exist in cart.";
                return RedirectToAction(nameof(Index));
            }

            var cart = HttpContext.Session.GetObjectFromJson<List<CartViewModel>>(this.User.Identity.Name) ?? new List<CartViewModel>();
            cart = cart.Where(p => p.Id != parsedProductId).ToList();

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
        public async Task<IActionResult> Checkout(CheckoutPageViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var cart = HttpContext.Session.GetObjectFromJson<List<CartViewModel>>(this.User.Identity.Name) ?? new List<CartViewModel>();
                model.Products = cart;

                return View(model);
            }

            if (!model.IsTermsAccepted)
            {
                ModelState.AddModelError("All", "Terms should be accepted.");
                var cart = HttpContext.Session.GetObjectFromJson<List<CartViewModel>>(this.User.Identity.Name) ?? new List<CartViewModel>();
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

            return RedirectToAction(nameof(Checkout));

            //submit the order
        }
    }
}
