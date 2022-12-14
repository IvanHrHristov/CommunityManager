﻿using CommunityManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using static CommunityManager.Infrastructure.Data.Constants.RoleConstants;

namespace CommunityManager.Areas.Administration.Controllers
{
    /// <summary>
    /// Controller to manage communities
    /// </summary>
    [Authorize(Roles = Administrator)]
    [Area(AdminArea)]
    public class HomeController : Controller
    {
        /// <summary>
        /// Shows the home page
        /// </summary>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Shows the 404 error page
        /// </summary>
        public IActionResult Error404()
        {
            return View();
        }

        /// <summary>
        /// Shows the error page for unchaught exceptions
        /// </summary>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}