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

        public IActionResult Edit(int[] categoryIds)
        {
            if (categoryIds.Length == 0)
            {
                TempData["error"] = "Please select category to edit!";
                return RedirectToAction("Index");
            }
            if (categoryIds.Length != 1)
            {
                TempData["error"] = "Please select only one category to edit!";
                return RedirectToAction("Index");
            }
            var user = _categoryService.GetCategory(categoryIds[0]);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }
        [HttpPost]
        public async Task<IActionResult> EditCategory(CategoryViewModel categoryModel)
        {
            var email = HttpContext.Session.GetString("UserEmail");
            var status = await _categoryService.UpdateCategory(categoryModel, email);
            if (status)
            {
                TempData["success"] = "Category edited successfully";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["error"] = "Error occurred while editing category";
                return RedirectToAction("Index");
            }
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
                TempData["success"] = "Category Created successfully";
            }
            else
            {
                TempData["error"] = "Something went wrong.";

            }
            return RedirectToAction("Index");
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
                TempData["error"] = "Error Occurred";
            }
            return RedirectToAction("Index");
        }
    }
}
