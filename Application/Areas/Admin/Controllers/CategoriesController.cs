using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Application.Areas.Admin.Models;
using Application.Controllers;
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

            var file = model.Image;

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, file.OpenReadStream())
            };

            var result = await cloudinary.UploadAsync(uploadParams);
            var imageUrl = result.Uri;

            var category = new Category()
            {
                Name = model.Name,
                Image = imageUrl.ToString()
            };

            var check = this.service.Create(category);
            if (!check)
            {
                ModelState.AddModelError("Name", "Such category already exists.");
                return View(model);
            }

            this.TempData["Success"] = $"Category {category.Name} created !";
            return RedirectToAction(nameof(HomeController.Index), "Home", new { area = "" });
        }
    }
}