using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using NttDocumentManagement.Models;

namespace DMS.Controllers
{
    /*
     * AUTH CONTROLLER MAIN CLASS
     */
    public class AuthController : Controller
    {
        /*
         * SHOW LOGIN FORM
         */
        public IActionResult Index()
        {
            return View();
        }

        /*
         * USER LOGIN
         */
        [HttpPost]
        public IActionResult Login()
        {
            return RedirectToAction("Index", "Home");
        }
        /*
         * LOGOUT
         */
        [HttpGet]
        public IActionResult LogOut()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Auth");
        }
        public IActionResult Forbidden()
        {
            TempData["error"] = "Permissin Denied!";
            return RedirectToAction("Index", "Auth");
        }

        /*
         * ERROR PAGE
         */
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}