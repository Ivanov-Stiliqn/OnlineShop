using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Application.Areas.Shopping.Models;
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
                Latest = this.productsService.GetLatestProducts().ProjectTo<ProductViewModel>().ToList(),
                MostViewed = this.productsService.GetMostViewedProducts().ProjectTo<ProductViewModel>().ToList(),
                MostOrderedProducts = this.productsService.GetMostOrderedProducts().ProjectTo<ProductViewModel>().ToList(),
                TopProduct = this.productsService.GetTopProduct().ProjectTo<TopProductViewModel>().ToList().First(),
                TopCategories = this.categoriesService.GetTopCategories().ProjectTo<CategoryViewModel>().ToList()
            };

            return View(model);
        }

        public IActionResult Create()
        {
            var model = new CreateProductViewModel
            {
                AllCategories = this.categoriesService.GetCategories().ProjectTo<CategoryListItemViewModel>().ToList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AllCategories = this.categoriesService.GetCategories().ProjectTo<CategoryListItemViewModel>()
                    .ToList();

                return View(model);
            }

            var check = Guid.TryParse(model.CategoryId, out Guid parsedCategoryId);
            if (!check)
            {
                ModelState.AddModelError("CategoryId", "Category does not exists.");
                model.AllCategories = this.categoriesService.GetCategories().ProjectTo<CategoryListItemViewModel>()
                    .ToList();

                return View(model);
            }

            var urls = new List<string>();

            foreach (var formFile in model.Images)
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(formFile.FileName, formFile.OpenReadStream())
                };

                var result = await cloudinary.UploadAsync(uploadParams);
                urls.Add(result.Uri.ToString());
            }

            var product = new Product()
            {
                Name = model.Name,
                CategoryId = parsedCategoryId,
                Description = model.Description,
                Details = model.Details,
                Color = model.Color,
                Price = model.Price,
                Sex = model.Sex,
                ImageUrls = string.Join(", ", urls)
            };

            this.productsService.CreateProduct(product, User.Identity.Name);
            this.TempData["Success"] = "Product added. Please choose sizes.";

            return RedirectToAction(nameof(SizesController.Create), "Sizes", new {productId = product.Id});
        }
    }
}
