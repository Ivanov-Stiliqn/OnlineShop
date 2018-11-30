using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Areas.Shopping.Models;
using Application.Controllers;
using Application.Infrastructure.Mapping;
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
                AllSizes = service.GetSizes().Select(s => s.Map<Size, SizeListItemViewModel>()).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateSizeViewModel model, string submit, string productId)
        {
            if (!ModelState.IsValid)
            {
                model.AllSizes = service.GetSizes().Select(s => s.Map<Size, SizeListItemViewModel>()).ToList();
                return View(model);
            }

            var productCheck = Guid.TryParse(productId, out Guid parsedProductId);
            if (!productCheck)
            {
                this.TempData["Error"] = "Product does not exists.";
                return RedirectToAction(nameof(ProductsController.Index), "Products");
            }

            var size = model.Map<CreateSizeViewModel, ProductSize>();

            size.ProductId = parsedProductId;

            await this.service.Create(size);
            this.TempData["Success"] = "Size added.";

            if (!string.IsNullOrEmpty(submit))
            {
                return RedirectToAction(nameof(ProductsController.Index), "Products");
            }

            return RedirectToAction(nameof(Create), new { productId = parsedProductId });
        }
    }
}
