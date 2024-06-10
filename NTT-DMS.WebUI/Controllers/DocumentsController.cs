using Microsoft.AspNetCore.Mvc;

namespace NttDocumentManagement.Controllers
{
    public class DocumentsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
