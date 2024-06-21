using Microsoft.AspNetCore.Mvc;
using NTT_DMS.Data;
using NTT_DMS.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using System.Drawing.Printing;

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
        public async Task<IActionResult> Index(int page = 1)
        {
            var users = _userService.GetAll();
            int pageSize = 7;
            var getList = await PaginatedList<User>.CreateAsyncList(users, page, pageSize);
            return View(getList);
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
            var email = HttpContext.Session.GetString("UserEmail");
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
        [HttpPost, ActionName("Create")]
        public async Task<IActionResult> Create(User user)
        {
            var email = HttpContext.Session.GetString("UserEmail");
            var status =  await _userService.Create(user, email);
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
        [HttpPost, ActionName("Delete")]
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
