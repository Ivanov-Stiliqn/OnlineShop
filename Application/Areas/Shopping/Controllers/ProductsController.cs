using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Application.Areas.Shopping.Models;
using Application.Infrastructure.Mapping;
using AutoMapper.QueryableExtensions;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
using Models;
using Services.Contracts;

namespace Application.Areas.Shopping.Controllers
{
    public class ProductsController : ShoppingAreaController
    {
        private readonly Cloudinary cloudinary;
        private readonly ICategoriesService categoriesService;
        private readonly IProductsService productsService;

        public ProductsController(Cloudinary cloudinary, ICategoriesService categoriesService, IProductsService productsService)
        {
            this.cloudinary = cloudinary;
            this.categoriesService = categoriesService;
            this.productsService = productsService;
        }

        public IActionResult Index()
        {
            var model = new HomeViewModel()
            {
                Latest = this.productsService.GetLatestProducts().Select(p => p.Map<Product, ProductViewModel>()).ToList(),
                MostViewed = this.productsService.GetMostViewedProducts().Select(p => p.Map<Product, ProductViewModel>()).ToList(),
                MostOrderedProducts = this.productsService.GetMostOrderedProducts().Select(p => p.Map<Product, ProductViewModel>()).ToList(),
                TopProduct = this.productsService.GetTopProduct().Map<Product, TopProductViewModel>(),
                TopCategories = this.categoriesService.GetTopCategories().Select(c => c.Map<Category, CategoryViewModel>()).ToList()
            };

            return View(model);
        }

        public IActionResult Create()
        {
            var model = new CreateProductViewModel
            {
                AllCategories = this.categoriesService.GetCategories().Select(c => c.Map<Category, CategoryListItemViewModel>()).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AllCategories = this.categoriesService.GetCategories()
                    .Select(c => c.Map<Category, CategoryListItemViewModel>())
                    .ToList();

                return View(model);
            }

            var urls = new List<string>();

            foreach (var formFile in model.ImagesFiles)
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(formFile.FileName, formFile.OpenReadStream())
                };

                var result = await cloudinary.UploadAsync(uploadParams);
                urls.Add(result.Uri.ToString());
            }

            var product = model.Map<CreateProductViewModel, Product>();

            product.ImageUrls = string.Join(", ", urls);

            await this.productsService.CreateProduct(product, User.Identity.Name);
            this.TempData["Success"] = "Product added. Please choose sizes.";

            return RedirectToAction(nameof(SizesController.Create), "Sizes", new {productId = product.Id});
        }

        public async Task<IActionResult> Details(string id)
        {
            var check = Guid.TryParse(id, out Guid parsedId);
            if (!check)
            {
                this.TempData["Error"] = "Product does not exists.";
                return RedirectToAction(nameof(Index));
            }

            var product = await this.productsService.GetProduct(parsedId);
            if (product == null)
            {
                this.TempData["Error"] = "Product does not exists.";
                return RedirectToAction(nameof(Index));
            }

            var details = product.Map<Product, ProductDetailsViewModel>();

            var model = new ProductDetailsPageViewModel
            {
                Product = details,
                MostOrderedProducts = this.productsService.GetMostOrderedProducts().Select(p => p.Map<Product, ProductViewModel>()).ToList()
            };

            return View(model);
        }
    }
}
