﻿using Microsoft.AspNetCore.Mvc;

namespace ListingApi.Controllers
{
    public class ListingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
