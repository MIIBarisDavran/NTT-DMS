using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NTT_DMS.Controllers;
using NTT_DMS.Data;
using NTT_DMS.Service;

namespace NTT_DMS.WebUI.Controllers
{
    [Authorize(Roles = "Admin,User")]
    public class DirectoryController : Controller
    {
        private readonly IWebHostEnvironment _appEnvironment;
        private readonly DirectoryService _directoryService;
        private readonly ILogger<DocumentController> _logger;

        public DirectoryController(
            IWebHostEnvironment appEnvironment,
            DirectoryService directoryService,
            ILogger<DocumentController> logger)
        {
            _appEnvironment = appEnvironment;
            _directoryService = directoryService;
            _logger = logger;
        }

        [HttpGet]
        [Route("api/directory/tree")]
        public IActionResult GetDirectoryTree()
        {
            var directoryTree = _directoryService.GetDirectoryTree();
            return Json(directoryTree);
        }

        public async Task<IActionResult> Index() {
            return View();
        }

    }
}
