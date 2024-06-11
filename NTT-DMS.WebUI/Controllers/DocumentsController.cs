﻿using Microsoft.AspNetCore.Mvc;
using NTT_DMS.Data;
using NTT_DMS.Service;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NTT_DMS.Controllers
{
    [Authorize(Roles = "Admin,User")]
    public class DocumentsController : Controller
    {
        private readonly IWebHostEnvironment _appEnvironment;
        private readonly DocumentService _documentService;
        private readonly CategoryService _categoryService;
        private readonly ILogger<DocumentsController> _logger;

        public DocumentsController(
            IWebHostEnvironment appEnvironment,
            DocumentService documentService,
            CategoryService categoryService,
            ILogger<DocumentsController> logger)
        {
            _appEnvironment = appEnvironment;
            _documentService = documentService;
            _categoryService = categoryService;
            _logger = logger;
        }

        /*
         * GET OWN DOCUMENTS 
         */
        public async Task<IActionResult> Index(string str, int page = 1)
        {
            try
            {
                var email = HttpContext.Session.GetString("UserEmail");
                var documentList = await _documentService.GetList(email, str);
                int pageSize = 10;
                return View(await PaginatedList<DocumentViewModel>.CreateAsync(documentList.AsNoTracking(), page, pageSize));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting documents for user");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        /*
         * SHOW DOCUMENT UPLOAD FORM
         */
        public IActionResult Create()
        {
            var email = HttpContext.Session.GetString("UserEmail");
            ViewBag.categories = _categoryService.GetAll(email);
            return View();
        }

        /*
         * UPLOAD NEW DOCUMENT
         */
        [HttpPost]
        public async Task<IActionResult> Create(IFormFile file, Document document)
        {
            try
            {
                var email = HttpContext.Session.GetString("UserEmail");
                ViewBag.categories = _categoryService.GetAll(email);
                string pathRoot = _appEnvironment.WebRootPath;

                var documentUploadResponse = _documentService.Upload(file, pathRoot, document, email);
                if (documentUploadResponse.ContainsKey("error"))
                {
                    ViewBag.error = documentUploadResponse["error"];
                }
                else
                {
                    ViewBag.success = documentUploadResponse["success"];
                }
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading document");
                ViewBag.error = "An error occurred while uploading the document.";
                return View();
            }
        }

        /*
         * DOWNLOAD DOCUMENT
         */
        public async Task<IActionResult> DownloadAsync(int id)
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                if (userId == null)
                {
                    TempData["Error"] = "User not logged in";
                    return RedirectToAction("Index");
                }

                var status = _documentService.DocumentPermissionRule((int)userId, id);
                if (status)
                {
                    string filePath = _documentService.GetPath((int)userId, id);
                    string fileName = _documentService.GetName((int)userId, id);
                    return await ReturnDocumentFileAsync(filePath, fileName);
                }
                else
                {
                    TempData["Error"] = "Document permission failed";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading document");
                TempData["Error"] = "An error occurred while downloading the document.";
                return RedirectToAction("Index");
            }
        }

        /*
         * RETURN FILE
         */
        private async Task<FileResult> ReturnDocumentFileAsync(string filePath, string fileName)
        {
            try
            {
                var path = Path.Combine(_appEnvironment.WebRootPath, "Documents", fileName);

                var memory = new MemoryStream();
                using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;
                return File(memory, GetContentType(path), Path.GetFileName(path));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error returning document file");
                throw;
            }
        }

        /*
         * GET CONTENT TYPES
         */
        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        /*
         * GET MIME TYPES
         */
        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                { ".txt", "text/plain" },
                { ".pdf", "application/pdf" },
                { ".doc", "application/vnd.ms-word" },
                { ".docx", "application/vnd.ms-word" },
                { ".png", "image/png" },
                { ".jpg", "image/jpeg" },
                { ".jpeg", "image/jpeg" },
                { ".gif", "image/gif" },
                { ".csv", "text/csv" }
            };
        }
    }
}
