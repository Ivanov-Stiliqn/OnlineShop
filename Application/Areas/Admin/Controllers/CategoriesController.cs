using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Application.Areas.Admin.Models;
using Application.Controllers;
using Application.Infrastructure.Mapping;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Services.Contracts;

namespace Application.Areas.Admin.Controllers
{
    public class CategoriesController : AdminAreaController
    {
        private readonly ICategoriesService service;
        private readonly Cloudinary cloudinary;

        public CategoriesController(ICategoriesService service, Cloudinary cloudinary)
        {
            this.service = service;
            this.cloudinary = cloudinary;
        }

        public IActionResult Index()
        {
            var model = this.service.GetCategories().Select(c => c.Map<Category, CategoryViewModel>()).ToList();
            return View(model);
        }

        public IActionResult Create()
        {
            var model = new CreateCategoryViewModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var file = model.ImageFile;

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, file.OpenReadStream()),
                Transformation = new Transformation().Width(600).Height(600).Crop("scale")
            };

            var result = await cloudinary.UploadAsync(uploadParams);
            var imageUrl = result.Uri;

            var category = model.Map<CreateCategoryViewModel, Category>();

            category.Image = imageUrl.ToString();

            var check = await this.service.Create(category);
            if (!check)
            {
                ModelState.AddModelError("Name", "Such category already exists.");
                return View(model);
            }

            this.TempData["Success"] = $"Category {category.Name} created !";
            return RedirectToAction(nameof(HomeController.Index), "Home", new { area = "" });
        }

        public IActionResult Edit(string id)
        {
            var category = this.service.GetCategory(id);
            if (category == null)
            {
                this.TempData["Error"] = "Category does not exist.";
                return RedirectToAction(nameof(Index));
            }

            var model = new EditCategoryPageViewModel
            {
                Category = category.Map<Category, EditCategoryViewModel>()
            };

            this.TempData["CurrentCategoryId"] = id;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditCategoryPageViewModel model)
        {
            var id = this.TempData["CurrentCategoryId"].ToString();
            if (id != model.Category.Id.ToString())
            {
                this.TempData["Error"] = "Invalid operation";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                this.TempData["CurrentCategoryId"] = model.Category.Id;
                return View(model);
            }

            var category = model.Category.Map<EditCategoryViewModel, Category>();

            if (model.ImageFile != null)
            {
                var file = model.ImageFile;

                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, file.OpenReadStream()),
                    Transformation = new Transformation().Width(600).Height(600).Crop("scale")
                };

                var result = await cloudinary.UploadAsync(uploadParams);
                var imageUrl = result.Uri;
                category.Image = imageUrl.ToString();
            }

            await this.service.EditCategory(category);

            this.TempData["Success"] = $"Category edited successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(string id)
        {
            var check = await this.service.DeleteCategory(id);
            if (!check)
            {
                this.TempData["Error"] = "Category does not exist.";
                return RedirectToAction(nameof(Index));
            }

            this.TempData["Success"] = $"Category deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}