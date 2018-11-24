﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Application.Areas.Admin.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Models;
using Services.Contracts;

namespace Application.Areas.Admin.Controllers
{
    public class UsersController: AdminAreaController
    {
        private readonly IUsersService service;

        public UsersController(IUsersService service)
        {
            this.service = service;
        }

        public IActionResult All(string search)
        {
            var users = search != null ? this.service.SearchByName(search, this.User.Identity.Name) : this.service.AllUsers(this.User.Identity.Name);
            var model = users
                .ProjectTo<UserViewModel>()
                .ToList();


            return View(model);
        }

        public IActionResult Restrict(string userId)
        {
            var check = this.service.RestrictUser(userId);
            if (!check)
            {
                this.TempData["Error"] = "User does not exists.";
                return RedirectToAction(nameof(All));
            }

            this.TempData["Success"] = "User restricted.";
            return RedirectToAction(nameof(All));
        }

        public IActionResult UnRestrict(string userId)
        {
            var check = this.service.UnRestrictUser(userId);
            if (!check)
            {
                this.TempData["Error"] = "User does not exists.";
                return RedirectToAction(nameof(All));
            }

            this.TempData["Success"] = "User unrestricted.";
            return RedirectToAction(nameof(All));
        }
    }
}