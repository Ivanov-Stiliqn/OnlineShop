using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Areas.Shopping.Models;
using Application.Infrastructure.Mapping;
using Application.Models;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.Enums;
using Services.Contracts;

namespace Application.Areas.Shopping.Controllers
{
    public class CategoriesController: ShoppingAreaController
    {
        private readonly IProductsService productsService;
        private readonly ICategoriesService categoriesService;
        private readonly ISizesService sizesService;

        public CategoriesController(IProductsService productsService, ICategoriesService categoriesService, ISizesService sizesService)
        {
            this.productsService = productsService;
            this.categoriesService = categoriesService;
            this.sizesService = sizesService;
        }

        public async Task<IActionResult> Products(string categoryId, string price, string sizeId, Sex sex, int page = 1)
        {
            var check = Guid.TryParse(categoryId, out Guid parsedCategoryId);
            var categories = this.categoriesService.GetCategories().Select(c => c.Map<Category, MenuItemViewModel>()).ToList();
            var category = categories.FirstOrDefault(c => c.Id == categoryId);
            if (!check || category == null)
            {
                this.TempData["Error"] = "Category does not exists.";
                return RedirectToAction(nameof(ProductsController.Index), "Products");
            }

            var sizeCheck = Guid.TryParse(sizeId, out Guid parseSizeId);
            if (!string.IsNullOrEmpty(sizeId) && !sizeCheck)
            {
                this.TempData["Error"] = "Invalid size filter.";
                return RedirectToAction(nameof(Products), new { categoryId, page });
            }

            decimal minPrice = 0m;
            decimal maxPrice = 0m;

            if (!string.IsNullOrEmpty(price))
            {
                var priceArgs = price.Split("$", StringSplitOptions.RemoveEmptyEntries);
                if (priceArgs.Length < 2)
                {
                    this.TempData["Error"] = "Invalid price range filter.";
                    return RedirectToAction(nameof(Products), new { categoryId, page });
                }

                var minPriceCheck = decimal.TryParse(priceArgs[0].Trim(), out minPrice);
                var maxPriceCheck = decimal.TryParse(priceArgs[1].Trim(), out maxPrice);
                if (!minPriceCheck || !maxPriceCheck || minPrice < 0 || maxPrice < 0)
                {
                    this.TempData["Error"] = "Invalid price range filter.";
                    return RedirectToAction(nameof(Products), new {categoryId, page});
                }
            }

            var size = 3;

            var skip = (page - 1) * size;
            var take = size;

            var categoryProducts = await this.categoriesService.GetProductsByCategory(parsedCategoryId, skip, take,
                minPrice, maxPrice, parseSizeId, sex);

            var products = categoryProducts
                .Select(p => p.Map<Product, ProductViewModel>())
                .ToList();

            var productsCount = this.categoriesService.SearchedProductsCount;

            var totalPages = (int)Math.Ceiling(decimal.Divide(productsCount, size));
            var first = 1;
            var last = totalPages;
            var previuous = Math.Max(page - 1, first);
            var next = Math.Min(page + 1, last);

            var sizes = this.sizesService.GetSizes().Select(s => s.Map<Size, SizeListItemViewModel>()).ToList();
            var currentSize = sizes.Where(s => s.Id == sizeId).Select(s => s.Name).FirstOrDefault();

            var model = new CategoryProductsViewModel
            {
                Products = products,
                MostOrderedProducts =
                    this.productsService.GetMostOrderedProducts().Select(p => p.Map<Product, ProductViewModel>()).ToList(),
                Pagination = new Pagination
                {
                    First = first,
                    Last = last,
                    Next = next,
                    Previous = previuous,
                    TotalPages = totalPages,
                    Active = page,
                    Category = categoryId,
                    Price = price,
                    SizeId = sizeId,
                    Size = currentSize,
                    Sex = (int)sex,
                    Gender = sex.ToString()
                },
                Categories = categories,
                CurrentCategory = category,
                Sizes = sizes
            };

            this.TempData["currentCategory"] = category.Name;
            return View(model);
        }
    }
}
