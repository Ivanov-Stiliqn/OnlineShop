using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Areas.Shopping.Models;
using Application.Controllers;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Models;
using Services.Contracts;

namespace Application.Areas.Shopping.Controllers
{
    public class SizesController: ShoppingAreaController
    {
        private readonly ISizesService service;

        public SizesController(ISizesService service)
        {
            this.service = service;
        }

        public IActionResult Create(string productId)
        {
            var model = new CreateSizeViewModel
            {
                AllSizes = service.GetSizes().ProjectTo<SizeListItemViewModel>().ToList()
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Create(CreateSizeViewModel model, string submit, string productId)
        {
            if (!ModelState.IsValid)
            {
                model.AllSizes = service.GetSizes().ProjectTo<SizeListItemViewModel>().ToList();
                return View(model);
            }

            var productCheck = Guid.TryParse(productId, out Guid parsedProductId);
            if (!productCheck)
            {
                this.TempData["Error"] = "Product does not exists.";
                return RedirectToAction(nameof(ProductsController.Index), "Products");
            }

            var sizeCheck = Guid.TryParse(model.SizeId, out Guid parsedSizeId);
            if (!sizeCheck)
            {
                ModelState.AddModelError("SizeId", "Size does not exists.");
                model.AllSizes = service.GetSizes().ProjectTo<SizeListItemViewModel>().ToList();
                return View(model);
            }

            var size = new ProductSize
            {
                ProductId = parsedProductId,
                SizeId = parsedSizeId,
                Quantity = model.Quantity
            };

            this.service.Create(size);
            this.TempData["Success"] = "Size added.";

            if (!string.IsNullOrEmpty(submit))
            {
                return RedirectToAction(nameof(ProductsController.Index), "Products");
            }

            return RedirectToAction(nameof(Create), new { productId = parsedProductId });
        }
    }
}
