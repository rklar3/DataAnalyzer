using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DataAnalyzerML.Model;
using Microsoft.AspNetCore.Mvc;

namespace DataAnalyzer.Controllers
{
    public class ValidatorController : Controller
    {
        [HttpGet]
        public IActionResult Predict()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Predict(ModelInput input)
        {
            Uri tempURI;

            if (Uri.TryCreate(input.URL, UriKind.RelativeOrAbsolute, out tempURI))
            {
                input.URL = tempURI.Host;
                input.LocalPath = tempURI.LocalPath;
            }
            else
            {
                input.LocalPath = "/";
                input.URL = input.URL;
            }

            var prediction = ConsumeModel.Predict(input);
            ViewBag.Result = prediction;
            return View();
        }

    }
}
