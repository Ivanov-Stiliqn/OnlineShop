using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Areas.Shopping.Models;
using Application.Models;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;

namespace Application.Areas.Shopping.Controllers
{
    public class CategoriesController: ShoppingAreaController
    {
        private readonly IProductsService productsService;

        public CategoriesController(IProductsService productsService)
        {
            this.productsService = productsService;
        }

        public IActionResult Products(string categoryId, string price, int page = 1)
        {
            var check = Guid.TryParse(categoryId, out Guid parsedCategoryId);
            if (!check)
            {
                this.TempData["Error"] = "Category does not exists.";
                return RedirectToAction(nameof(ProductsController.Index), "Products");
            }

            if (!string.IsNullOrEmpty(price))
            {
                var priceArgs = price.Split("$", StringSplitOptions.RemoveEmptyEntries);
                var minPrice = decimal.Parse(priceArgs[0].Trim());
                var maxPrice = decimal.Parse(priceArgs[1].Trim());

            }

            var size = 3;

            var skip = (page - 1) * size;
            var take = size;

            var products = this.productsService.GetProductsByCategory(parsedCategoryId, skip, take).ProjectTo<ProductViewModel>()
                .ToList();

            var productsCount = this.productsService.ProductsCount(parsedCategoryId);

            var totalPages = (int)Math.Ceiling(decimal.Divide(productsCount, size));
            var first = 1;
            var last = totalPages;
            var previuous = Math.Max(page - 1, first);
            var next = Math.Min(page + 1, last);

            var model = new CategoryProductsViewModel
            {
                Products = products,
                MostOrderedProducts =
                    this.productsService.GetMostOrderedProducts().ProjectTo<ProductViewModel>().ToList(),
                Pagination = new Pagination
                {
                    First = first,
                    Last = last,
                    Next = next,
                    Previous = previuous,
                    TotalPages = totalPages,
                    Active = page,
                    Category = categoryId,
                    Price = price
                }
            };

            return View(model);
        }
    }
}
