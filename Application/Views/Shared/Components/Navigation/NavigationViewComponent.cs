using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Infrastructure.Mapping;
using Application.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Models;
using Services.Contracts;

namespace Application.Views.Shared.Components.Navigation
{
    [ViewComponent(Name = "Navigation")]
    public class NavigationViewComponent: ViewComponent
    {
        private readonly ICategoriesService service;
        private readonly IOrdersService ordersService;
       
        public NavigationViewComponent(ICategoriesService service, IOrdersService ordersService)
        {
            this.service = service;
            this.ordersService = ordersService;
        }

        public IViewComponentResult Invoke()
        {
            var model = new NavigationViewModel
            {
                Categories = this.service
                    .GetCategories()
                    .Select(c => c.Map<Category, MenuItemViewModel>())
                    .ToList(),
                UnSeenPurchaseOrders = this.User.Identity.IsAuthenticated &&
                    this.ordersService.IsThereUnSeenPurchaseOrders(this.User.Identity.Name),
                UnSeenSellOrders = this.User.Identity.IsAuthenticated &&
                    this.ordersService.IsThereUnSeenSellOrders(this.User.Identity.Name)
            };

            return View(model);
        }
    }
}
