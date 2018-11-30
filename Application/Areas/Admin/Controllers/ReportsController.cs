using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Areas.Admin.Models;
using Application.Infrastructure.Mapping;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Models;
using Services.Contracts;

namespace Application.Areas.Admin.Controllers
{
    public class ReportsController: AdminAreaController
    {
        private readonly IReportsService service;

        public ReportsController(IReportsService service)
        {
            this.service = service;
        }

        public IActionResult All()
        {
            var reports = this.service.GetReports().Select(r => r.Map<Report, ReportViewModel>()).ToList();
            return View(reports);
        }
    }
}
