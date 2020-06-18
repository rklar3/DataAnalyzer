using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DataAnalyzer.Models;
using System.IO;
using Newtonsoft.Json;
using Microsoft.CodeAnalysis.CSharp;

namespace DataAnalyzer.Controllers
{
    public class HomeController : Controller
    {
        private readonly DataContext _datacontext;
        public HomeController(DataContext datacontext)
        {
            _datacontext = datacontext;
        }

        public IActionResult Index()
        {
            var groupedlist = _datacontext.getListOfRiskDetails();

            int RecordsWithRisk = _datacontext.totalRiskRecords();
            ViewData["Message"] = $"{_datacontext.TotalRecords} total records ";
            
            ViewData["Bad"] = RecordsWithRisk;
            ViewData["Good"] = _datacontext.TotalRecords - RecordsWithRisk;

            //List<MostSearchWebsites> websites = _datacontext.getOverallTop5Websites();
            //ViewBag.Websites = websites;

            return View(groupedlist);
        }

        public IActionResult Details(int id)
        {
            var RiskDetails = _datacontext.getRiskDetails(id);
            return View(RiskDetails);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
