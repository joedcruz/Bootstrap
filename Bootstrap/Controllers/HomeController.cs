using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Bootstrap.Models;

namespace Bootstrap.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Register()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        public IActionResult Index()
        {
            return View("~/Views/Home/Login.cshtml");
        }        

        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult Cards()
        {
            return View();
        }

        public IActionResult Tables()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
