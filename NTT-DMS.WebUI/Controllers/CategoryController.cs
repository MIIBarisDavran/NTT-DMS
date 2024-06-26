using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NTT_DMS.Data;
using NTT_DMS.Service;
using System.Drawing.Printing;

namespace NTT_DMS.Controllers
{
    [Authorize(Roles = "Admin,User")]
    public class CategoryController : Controller
    {
        private CategoryService _categoryService;
        public CategoryController(DMSContext context)
        {
            _categoryService = new CategoryService(context);
        }

        /*
         * CATEGORY LIST
         */
        public async Task<IActionResult> Index(string str, int page = 1)
        {
            var email = HttpContext.Session.GetString("UserEmail");
            var categories = _categoryService.GetAllFiltered(email, str);
            int pageSize = 7;
            var getList = await PaginatedList<Category>.CreateSyncList(categories, page, pageSize);
            return View(getList);
        }

        /*
         * SHOW CATEGORY CREATE FORM
         */
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        /*
         * NEW CATEGORY CREATE
         */
        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            var email = HttpContext.Session.GetString("UserEmail");
            var status = await _categoryService.CreateCategory(category, email);
            if (status)
            {
                ViewBag.success = "Created successfully";
            }
            else
            {
                ViewBag.error = "Something went wrong.";
            }
            return View();
        }

        /*
         * CATEGORY DELETE BY ID
         */

        public async Task<IActionResult> Delete(int[] categoryIds)
        {
            var email = HttpContext.Session.GetString("UserEmail");
            var status = await _categoryService.DeleteCategory(categoryIds, email);
            if (status)
            {
                TempData["success"] = "Deleted successfully";
            }
            else
            {
                TempData["Error"] = "Error Occurred";
            }
            return RedirectToAction("Index");
        }
    }
}
