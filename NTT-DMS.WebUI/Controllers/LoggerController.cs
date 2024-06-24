using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NTT_DMS.Data;
using NTT_DMS.Service;

namespace NTT_DMS.WebUI.Controllers
{
    [Authorize(Policy = "Admin")]
    public class LoggerController : Controller
    {
        private readonly LoggerService _loggerService;
        public LoggerController(DMSContext _context)
        {
            _loggerService = new LoggerService(_context);
        }


        public async Task<IActionResult> Index(string str, int page = 1)
        {
            var logs = _loggerService.GetAll(str);
            int pageSize = 7;
            var getList = await PaginatedList<Log>.CreateSyncList(logs, page, pageSize);
            return View(getList);
        }
    }
}
