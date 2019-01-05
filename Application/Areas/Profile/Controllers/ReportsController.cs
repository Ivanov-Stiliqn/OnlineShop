using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Application.Areas.Profile.Models;
using Application.Areas.Shopping.Controllers;
using Application.Infrastructure.Mapping;
using Microsoft.AspNetCore.Mvc;
using Models;
using Services.Contracts;

namespace Application.Areas.Profile.Controllers
{
    public class ReportsController: ProfileAreaController
    {
        private readonly IReportsService reportsService;

        public ReportsController(IReportsService reportsService)
        {
            this.reportsService = reportsService;
        }

        public IActionResult Create(string userId)
        {
            var model = new CreateReportViewModel
            {
                ReportedUserId = userId
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateReportViewModel model, string userId)
        {
            model.ReportedUserId = userId;
            var report = model.Map<CreateReportViewModel, Report>();

            await this.reportsService.SubmitReport(report, this.User.Identity.Name);

            this.TempData["Success"] = "Thank you for your report, our admins will consider it soon.";
            return RedirectToAction(nameof(ProductsController.Index), "Products", new {area = "Shopping"});
        }
    }
}
