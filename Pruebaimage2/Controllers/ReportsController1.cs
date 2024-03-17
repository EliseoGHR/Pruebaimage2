using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Pruebaimage2.Controllers
{
    public class ReportsController1 : Controller
    {
        [Authorize(Roles = "Administrador")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
