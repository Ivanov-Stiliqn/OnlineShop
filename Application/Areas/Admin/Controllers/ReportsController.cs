using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Areas.Admin.Models;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
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
            var reports = this.service.GetReports().ProjectTo<ReportViewModel>().ToList();
            return View(reports);
        }
    }
}
