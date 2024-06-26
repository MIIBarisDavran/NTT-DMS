using Microsoft.AspNetCore.Mvc;
using NTT_DMS.Data;
using NTT_DMS.Service;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO.Compression;
using Microsoft.CodeAnalysis;
using Microsoft.AspNetCore.StaticFiles;

namespace NTT_DMS.Controllers
{
    [Authorize(Roles = "Admin,User")]
    public class DocumentController : Controller
    {
        private readonly IWebHostEnvironment _appEnvironment;
        private readonly DocumentService _documentService;
        private readonly CategoryService _categoryService;
        private readonly ILogger<DocumentController> _logger;

        public DocumentController(
            IWebHostEnvironment appEnvironment,
            DocumentService documentService,
            CategoryService categoryService,
            ILogger<DocumentController> logger)
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
                int pageSize = 7;
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
        public async Task<IActionResult> Create(IFormFile file, NTT_DMS.Data.Document document)
        {
            try
            {
                var email = HttpContext.Session.GetString("UserEmail");
                ViewBag.categories = _categoryService.GetAll(email);
                string pathRoot = _appEnvironment.WebRootPath;

                var documentUploadResponse = await _documentService.Upload(file, pathRoot, document, email);
                if (documentUploadResponse.ContainsKey("error"))
                {
                    TempData["Error"] = documentUploadResponse["error"];
                }
                else
                {
                    TempData["success"] = documentUploadResponse["success"];
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
        [HttpPost]
        public async Task<IActionResult> DownloadSelected(int[] documentIds)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["Error"] = "User not logged in";
                return RedirectToAction("Index");
            }

            if (documentIds == null || documentIds.Length == 0)
            {
                TempData["Error"] = "No documents selected for download.";
                return RedirectToAction("Index");
            }

            try
            {
                if (documentIds.Length == 1)
                {
                    string fileName = _documentService.GetName((int)userId, documentIds[0]);
                    var path = Path.Combine(_appEnvironment.WebRootPath, "Documents", userId.ToString(), fileName);
                    return await ReturnDocumentFileAsync(path);
                }
                using (var memoryStream = new MemoryStream())
                {
                    using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                    {
                        foreach (var documentId in documentIds)
                        {
                            string fileName = _documentService.GetName((int)userId, documentId);
                            var path = Path.Combine(_appEnvironment.WebRootPath, "Documents", userId.ToString(), fileName);

                            var fileBytes = await System.IO.File.ReadAllBytesAsync(path);
                            var zipEntry = archive.CreateEntry(fileName, CompressionLevel.Fastest);

                            using (var zipStream = zipEntry.Open())
                            {
                                await zipStream.WriteAsync(fileBytes, 0, fileBytes.Length);
                            }
                        }
                    }

                    return File(memoryStream.ToArray(), "application/zip", "SelectedDocuments.zip");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading documents");
                TempData["Error"] = "An error occurred while downloading the documents.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult Delete(int[] documentIds)
        {
            if (documentIds == null || documentIds.Length == 0)
            {
                TempData["Error"] = "No documents selected for deletion";
                return RedirectToAction("Index");
            }
            var status = _documentService.Delete(documentIds);
            if(status)
            {
                ViewBag.success = "Selected documents were successfully deleted.";
                TempData["Error"] = null;
            } else
            {
                TempData["Error"] = "An error occurred while deleting the documents.";
            }
            return RedirectToAction("Index", "Document");
        }

        /*
         * RETURN FILE
         */
        private async Task<FileResult> ReturnDocumentFileAsync(string filePath)
        {
            try
            {
                var memory = new MemoryStream();
                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;
                var contentTypeProvider = new FileExtensionContentTypeProvider();
                if (!contentTypeProvider.TryGetContentType(filePath, out string contentType))
                {
                    contentType = "application/octet-stream";
                }
                return File(memory, contentType, Path.GetFileName(filePath));

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error returning document file");
                throw;
            }
        }
        [HttpGet("GetDocument")]
        public async Task<IActionResult> GetDocument(string filePath)
        {
            try
            {
                var absolutePath = Path.Combine(_appEnvironment.WebRootPath, filePath);
                if (!System.IO.File.Exists(absolutePath))
                {
                    return NotFound();
                }
                var memory = new MemoryStream();
                using (var stream = new FileStream(absolutePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;

                var contentTypeProvider = new FileExtensionContentTypeProvider();
                if (!contentTypeProvider.TryGetContentType(filePath, out string contentType))
                {
                    contentType = "application/octet-stream";
                }

                return File(memory, contentType, Path.GetFileName(filePath));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error returning document file");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
