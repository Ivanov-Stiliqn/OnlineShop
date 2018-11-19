using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;

namespace Application.Views.Shared.Components.Navigation
{
    [ViewComponent(Name = "Navigation")]
    public class NavigationViewComponent: ViewComponent
    {
        private readonly ICategoriesService service;
       
        public NavigationViewComponent(ICategoriesService service)
        {
            this.service = service;
        }

        public IViewComponentResult Invoke()
        {
            var model = this.service
                .GetCategories()
                .ProjectTo<MenuItemViewModel>()
                .ToList();

            return View(model);
        }
    }
}
