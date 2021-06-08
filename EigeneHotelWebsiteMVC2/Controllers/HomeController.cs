using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EigeneHotelWebsiteMVC2.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            string name = HttpContext.Session.GetString("name");

            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("isLoggedIn");
            HttpContext.Session.Remove("name");

            return View("Index");
        }
    }
}
