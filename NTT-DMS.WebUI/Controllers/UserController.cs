using Microsoft.AspNetCore.Mvc;
using NTT_DMS.Data;
using NTT_DMS.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;

namespace NTT_DMS.Controllers
{
    [Authorize(Policy = "Admin")]
    public class UserController : Controller
    {
        private readonly UserService _userService;
        public UserController(DMSContext _context, IConfiguration _config)
        {
            _userService = new UserService(_context, _config);
        }

        /*
         * GET LIST OF USERS
         */
        public IActionResult Index()
        {
            var users = _userService.GetAll();
            return View(users);
        }

        /*
         * NEW USER CREATE FORM
         */
        public IActionResult Create()
        {
            return View();
        }

        /*
         * USER EDIT FORM
         */
        public IActionResult Edit(int userId)
        {
            var user = _userService.GetUser(userId);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        /*
         * EDIT USER
         */
        [HttpPost]
        public IActionResult EditUser(UserViewModel userModel)
        {
            var user = _userService.UpdateUser(userModel);
            return RedirectToAction("Index");
        }

        /*
         * CREATE NEW USER
         */
        [HttpPost]
        public IActionResult Create(User user)
        {
            var status = _userService.Create(user);
            if (status)
            {
                ViewBag.success = "Created successfully";
            }
            else
            {
                ViewBag.error = "Error Occurred";
            }
            return RedirectToAction("Index","User");
        }

        /*
         * DELETE USER BY ID
         */
        [HttpPost]
        public IActionResult Delete(int[] userId)
        {
            var status = _userService.Delete(userId);
            if (status)
            {
                ViewBag.success = "Deleted successfully";
            }
            else
            {
                ViewBag.error = "Error Occurred";
            }
            return RedirectToAction("Index");
        }

    }
}
