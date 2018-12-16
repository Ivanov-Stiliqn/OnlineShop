using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Application.Areas.Shopping.Models;
using Application.Infrastructure.Helpers;
using Application.Infrastructure.Mapping;
using AutoMapper.QueryableExtensions;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Models;
using Services.Contracts;

namespace Application.Areas.Shopping.Controllers
{
    public class ProductsController : ShoppingAreaController
    {
        private readonly Cloudinary cloudinary;
        private readonly IMemoryCache memoryCache;
        private readonly ICategoriesService categoriesService;
        private readonly IProductsService productsService;
        private readonly IReviewService reviewService;
        private readonly IUsersService usersService;

        public ProductsController(
            Cloudinary cloudinary,
            IMemoryCache memoryCache,
            ICategoriesService categoriesService, 
            IProductsService productsService,
            IReviewService reviewService,
            IUsersService usersService)
        {
            this.cloudinary = cloudinary;
            this.memoryCache = memoryCache;
            this.categoriesService = categoriesService;
            this.productsService = productsService;
            this.reviewService = reviewService;
            this.usersService = usersService;
        }

        public IActionResult Index()
        {
            ICollection<ProductViewModel> mostOrderedProductsCache;
            if (!this.memoryCache.TryGetValue("MostOrderedProducts", out mostOrderedProductsCache))
            {
                mostOrderedProductsCache = this.productsService.GetMostOrderedProducts()
                    .Select(p => p.Map<Product, ProductViewModel>()).ToList();

                MemoryCacheEntryOptions memoryCacheOptions = new MemoryCacheEntryOptions();
                memoryCacheOptions.AbsoluteExpiration = DateTime.UtcNow.AddMinutes(5);
                this.memoryCache.Set<ICollection<ProductViewModel>>("MostOrderedProducts", mostOrderedProductsCache, memoryCacheOptions);
            }

            var model = new HomeViewModel()
            {
                Latest = this.productsService.GetLatestProducts().Select(p => p.Map<Product, ProductViewModel>()).ToList(),
                MostViewed = this.productsService.GetMostViewedProducts().Select(p => p.Map<Product, ProductViewModel>()).ToList(),
                MostOrderedProducts = mostOrderedProductsCache,
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
            var categories = this.categoriesService.GetCategories()
                .Select(c => c.Map<Category, CategoryListItemViewModel>())
                .ToList();

            if (!ModelState.IsValid)
            {
                model.AllCategories = categories;

                return View(model);
            }

            var urls = new List<string>();

            foreach (var formFile in model.ImagesFiles)
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(formFile.FileName, formFile.OpenReadStream()),
                    Transformation = new Transformation().Width(600).Height(600).Crop("scale")
                };

                var result = await cloudinary.UploadAsync(uploadParams);
                urls.Add(result.Uri.ToString());
            }

            var type = categories.Where(c => c.Id == model.CategoryId.ToString()).Select(c => c.Type).FirstOrDefault();

            var product = model.Map<CreateProductViewModel, Product>();
            product.ImageUrls = string.Join(", ", urls);

            await this.productsService.CreateProduct(product, User.Identity.Name);
            this.TempData["Success"] = "Product added. Please choose sizes.";

            return RedirectToAction(nameof(SizesController.Create), "Sizes", new {productId = product.Id, type, sex = model.Sex});
        }

        public async Task<IActionResult> Details(string id)
        {
            ICollection<ProductViewModel> mostOrderedProductsCache;
            if (!this.memoryCache.TryGetValue("MostOrderedProducts", out mostOrderedProductsCache))
            {
                mostOrderedProductsCache = this.productsService.GetMostOrderedProducts()
                    .Select(p => p.Map<Product, ProductViewModel>()).ToList();

                MemoryCacheEntryOptions memoryCacheOptions = new MemoryCacheEntryOptions();
                memoryCacheOptions.AbsoluteExpiration = DateTime.UtcNow.AddMinutes(5);
                this.memoryCache.Set<ICollection<ProductViewModel>>("MostOrderedProducts", mostOrderedProductsCache, memoryCacheOptions);
            }

            var product = await this.productsService.GetProduct(id, true);
            if (product == null)
            {
                this.TempData["Error"] = "Product does not exists.";
                return RedirectToAction(nameof(Index));
            }

            var details = product.Map<Product, ProductDetailsViewModel>();

            var model = new ProductDetailsPageViewModel
            {
                Product = details,
                MostOrderedProducts = mostOrderedProductsCache
            };

            this.TempData["CurrentProductId"] = id;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Details(ProductDetailsPageViewModel model, string id)
        {
            if (this.TempData["CurrentProductId"].ToString() != id)
            {
                this.TempData["Error"] = "Invalid operation.";
                return RedirectToAction(nameof(Index));
            }

            if (!this.ModelState.IsValid)
            {
                ICollection<ProductViewModel> mostOrderedProductsCache;
                if (!this.memoryCache.TryGetValue("MostOrderedProducts", out mostOrderedProductsCache))
                {
                    mostOrderedProductsCache = this.productsService.GetMostOrderedProducts()
                        .Select(p => p.Map<Product, ProductViewModel>()).ToList();

                    MemoryCacheEntryOptions memoryCacheOptions = new MemoryCacheEntryOptions();
                    memoryCacheOptions.AbsoluteExpiration = DateTime.UtcNow.AddMinutes(5);
                    this.memoryCache.Set<ICollection<ProductViewModel>>("MostOrderedProducts", mostOrderedProductsCache, memoryCacheOptions);
                }

                var product = await this.productsService.GetProduct(id, false);
                if (product == null)
                {
                    this.TempData["Error"] = "Product does not exists.";
                    return RedirectToAction(nameof(Index));
                }

                var details = product.Map<Product, ProductDetailsViewModel>();

                model.Product = details;
                model.MostOrderedProducts = mostOrderedProductsCache;

                this.TempData["CurrentProductId"] = id;
                return View(model);
            }

            var review = model.Map<ProductDetailsPageViewModel, Review>();
            review.ProductId = Guid.Parse(id);

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

            var categories = this.categoriesService.GetCategories()
                .Select(c => c.Map<Category, CategoryListItemViewModel>())
                .ToList();

            if (!ModelState.IsValid)
            {
                model.AllCategories = categories;

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
                        Transformation = new Transformation().Width(600).Height(600).Crop("scale")
                    };

                    var result = await cloudinary.UploadAsync(uploadParams);
                    urls.Add(result.Uri.ToString());
                }
            }

            var type = categories.Where(c => c.Id == model.Product.CategoryId.ToString()).Select(c => c.Type)
                .FirstOrDefault();

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

            return RedirectToAction(nameof(SizesController.Create), "Sizes", new {productId = product.Id, type, sex = model.Product.Sex});
        }

        public async Task<IActionResult> Delete(string id, bool isHome)
        {
            await this.productsService.DeleteProduct(id, this.User.Identity.Name);

            this.TempData["Success"] = "Product removed successfully.";
            return RedirectToAction(isHome ? nameof(Index) : nameof(MyProducts));
        }

        [Authorize]
        public IActionResult Message(string username)
        {
            var chats = this.HttpContext.Session.GetObjectFromJson<int>("ChatsCount");
            int position = 10;

            chats = chats == 4 ? 0 : chats; 
            if (chats > 0)
            {
                position += 440 * chats;
            }

            var model = new ChatViewModel()
            {
                Id = "chat_window_" + chats + 1,
                Position = position + "px",
                Username = username
            };

            this.HttpContext.Session.SetObjectAsJson("ChatsCount", chats + 1);

            return PartialView("_Chat", model);
        }

        [Authorize]
        public void RemoveChat()
        {
            var chatsCount = this.HttpContext.Session.GetObjectFromJson<int>("ChatsCount");
            chatsCount -= 1;
            this.HttpContext.Session.SetObjectAsJson("ChatsCount", chatsCount);
        }
    }
}
