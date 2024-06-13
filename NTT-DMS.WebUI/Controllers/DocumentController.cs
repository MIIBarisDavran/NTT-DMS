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
                using (var memoryStream = new MemoryStream())
                {
                    using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                    {
                        foreach (var documentId in documentIds)
                        {
                                string filePath = _documentService.GetPath((int)userId, documentId);
                                string fileName = _documentService.GetName((int)userId, documentId);
                                var path = Path.Combine(_appEnvironment.WebRootPath, "Documents", fileName);

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
            } else
            {
                TempData["Error"] = "An error occurred while deleting the documents.";
            }
            return RedirectToAction("Index", "Document");
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
