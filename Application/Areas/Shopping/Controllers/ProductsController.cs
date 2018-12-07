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
        private readonly IReviewService reviewService;
        private readonly IUsersService usersService;

        public ProductsController(
            Cloudinary cloudinary, 
            ICategoriesService categoriesService, 
            IProductsService productsService,
            IReviewService reviewService,
            IUsersService usersService)
        {
            this.cloudinary = cloudinary;
            this.categoriesService = categoriesService;
            this.productsService = productsService;
            this.reviewService = reviewService;
            this.usersService = usersService;
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
                    File = new FileDescription(formFile.FileName, formFile.OpenReadStream()),
                    Transformation = new Transformation().Width(262).Height(262).Crop("scale")
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

        [HttpPost]
        public async Task<IActionResult> Details(ProductDetailsPageViewModel model, string id)
        {
            var check = Guid.TryParse(id, out Guid parsedId);
            if (!check)
            {
                this.TempData["Error"] = "Product does not exists.";
                return RedirectToAction(nameof(Index));
            }

            if (!this.ModelState.IsValid)
            {
                var product = await this.productsService.GetProduct(parsedId);
                if (product == null)
                {
                    this.TempData["Error"] = "Product does not exists.";
                    return RedirectToAction(nameof(Index));
                }

                var details = product.Map<Product, ProductDetailsViewModel>();

                model.Product = details;
                model.MostOrderedProducts = this.productsService.GetMostOrderedProducts()
                    .Select(p => p.Map<Product, ProductViewModel>()).ToList();
                

                return View(model);
            }

            var review = model.Map<ProductDetailsPageViewModel, Review>();
            review.ProductId = parsedId;

            await this.reviewService.Create(review, this.User.Identity.Name);

            this.TempData["Success"] = "Review added.";
            return RedirectToAction(nameof(Details), new {id});
        }

        public IActionResult MyProducts()
        {
            var userProducts = this.usersService.GetUserProducts(this.User.Identity.Name);
            var model = userProducts.Select(p => p.Map<Product, ProductViewModel>()).ToList();

            return View(model);
        }

        public IActionResult Edit(string id)
        {
            var product = this.productsService.GetProductForCart(id);
            if (product == null)
            {
                this.TempData["Error"] = "Product does not exits.";
                return RedirectToAction(nameof(Index));
            }

            var model = new EditProductPageViewModel
            {
                Product = product.Map<Product, EditProductViewModel>(),
                AllCategories = this.categoriesService.GetCategories()
                    .Select(c => c.Map<Category, CategoryListItemViewModel>()).ToList()
            };

            this.TempData["EditedProductId"] = id;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditProductPageViewModel model, string id, string submit)
        {
            var check = Guid.TryParse(id, out Guid parsedId);
            if (!check)
            {
                this.TempData["Error"] = "Product does not exits.";
                return RedirectToAction(nameof(Index));
            }

            var idCheck = this.TempData["EditedProductId"].ToString();
            if (idCheck != id)
            {
                this.TempData["Error"] = "Invalid operation.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                model.AllCategories = this.categoriesService.GetCategories()
                    .Select(c => c.Map<Category, CategoryListItemViewModel>())
                    .ToList();

                this.TempData["EditedProductId"] = id;
                return View(model);
            }

            var urls = new List<string>();

            if (model.ImagesFiles != null && model.ImagesFiles.Any())
            {
                foreach (var formFile in model.ImagesFiles)
                {
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(formFile.FileName, formFile.OpenReadStream()),
                        Transformation = new Transformation().Width(262).Height(262).Crop("scale")
                    };

                    var result = await cloudinary.UploadAsync(uploadParams);
                    urls.Add(result.Uri.ToString());
                }
            }

            var product = model.Product.Map<EditProductViewModel, Product>();
            product.Id = parsedId;

            var isSuccess = await this.productsService.EditProduct(product, this.User.Identity.Name, urls);
            if (!isSuccess)
            {
                this.TempData["Error"] = "Invalid operation.";
                return RedirectToAction(nameof(Index));
            }

            this.TempData["Success"] = $"Product {product.Name} edited successfully.";
            if (!string.IsNullOrEmpty(submit))
            {
                return RedirectToAction(nameof(MyProducts));
            }

            return RedirectToAction(nameof(SizesController.Create), "Sizes", new {productId = product.Id});
        }

        public async Task<IActionResult> Delete(string id, bool isHome)
        {
            await this.productsService.DeleteProduct(id, this.User.Identity.Name);

            this.TempData["Success"] = "Product removed successfully.";
            return RedirectToAction(isHome ? nameof(Index) : nameof(MyProducts));
        }
    }
}
