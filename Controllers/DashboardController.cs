﻿using Microsoft.AspNetCore.Mvc;

namespace caobaModeloFabricacion.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
