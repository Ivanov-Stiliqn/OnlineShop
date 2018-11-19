using System;
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
            var users = search != null ? this.service.SearchByName(search) : this.service.AllUsers();
            var model = users
                .ProjectTo<UserViewModel>()
                .ToList();


            return View(model);
        }
    }
}
