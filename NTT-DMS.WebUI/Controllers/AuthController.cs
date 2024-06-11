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
using NTT_DMS.Models;
using NTT_DMS.Data;
using NTT_DMS.Service;

namespace NTT_DMS.Controllers
{
    /*
     * AUTH CONTROLLER MAIN CLASS
     */
    public class AuthController : Controller
    {
        private readonly AuthService _authService;
        private readonly ILogger<AuthController> _logger;
        public AuthController(AuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }
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
        public IActionResult Login(UserLogin user)
        {
            if (ModelState.IsValid)
            {
                var _user = _authService.CheckCredential(user);
                if (_user == null)
                {
                    TempData["error"] = "Credentials do not match.";
                    return RedirectToAction("Index", "Auth");
                }
                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                identity.AddClaim(new Claim(ClaimTypes.Name, _user[0].UserName.ToString()));
                identity.AddClaim(new Claim(ClaimTypes.Role, _user[0].UserRole));
                identity.AddClaim(new Claim(ClaimTypes.Email, _user[0].UserEmail));
                HttpContext.Session.SetString("UserEmail", _user[0].UserEmail);
                HttpContext.Session.SetInt32("UserId", _user[0].UserId);
                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

                if (_user[0].UserRole == "Admin")
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, "Admin"));
                    return RedirectToAction("Index", "Home");
                }
                else if (_user[0].UserRole == "User")
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, "User"));
                    return RedirectToAction("Index", "Home");
                }

            }
            TempData["error"] = "Credentials do not match.";
            return RedirectToAction("Index", "Auth");
        }
        /*
         * LOGOUT
         */
        [HttpGet]
        public async Task<IActionResult> LogOut()
        {
            try
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                HttpContext.Session.Clear();
                _logger.LogInformation("User logged out.");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                TempData["error"] = "An error occurred while logging out.";
                return RedirectToAction("Index");
            }
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
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            _logger.LogError($"Request ID: {requestId}");
            return View(new ErrorViewModel { RequestId = requestId });
        }
    }
}