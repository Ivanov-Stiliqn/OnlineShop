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
    public class WishlistController: ProfileAreaController
    {
        private readonly IUsersService usersService;
        private readonly IProductsService productsService;

        public WishlistController(IUsersService usersService, IProductsService productsService)
        {
            this.usersService = usersService;
            this.productsService = productsService;
        }

        public async Task<bool> Add(string productId)
        {
            var product = this.productsService.GetProductForCart(productId);
            if (product == null)
            {
                return false;
            }

            await this.usersService.AddProductToWhishlist(productId, this.User.Identity.Name);

            return true;
        }

        public IActionResult Index()
        {
            var products = this.usersService.GetWishList(this.User.Identity.Name);
            var model = products.Select(p => p.Map<Product, WishlistViewModel>()).ToList();

            return View(model);
        }

        public async Task<IActionResult> Remove(string productId)
        {
            await this.usersService.RemoveProductFromWishlist(productId, this.User.Identity.Name);

            this.TempData["Success"] = "Product removed from wishlist.";
            return RedirectToAction(nameof(Index));
        }
    }
}
