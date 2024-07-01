using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NTT_DMS.Controllers;
using NTT_DMS.Data;
using NTT_DMS.Service;

namespace NTT_DMS.WebUI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DirectoryApi : ControllerBase
    {
        private readonly IWebHostEnvironment _appEnvironment;
        private readonly DirectoryService _directoryService;
        private readonly CategoryService _categoryService;
        private readonly ILogger<DocumentController> _logger;

        public DirectoryApi(
            IWebHostEnvironment appEnvironment,
            DirectoryService directoryService,
            CategoryService categoryService,
            ILogger<DocumentController> logger)
        {
            _appEnvironment = appEnvironment;
            _directoryService = directoryService;
            _categoryService = categoryService;
            _logger = logger;
        }

        [HttpGet("tree")]
        public IActionResult GetDirectoryTree()
        {
            var directoryTree = _directoryService.GetDirectoryTree();
            return Ok(directoryTree);
        }
    }
}
