using Microsoft.AspNet.Mvc;
using MvcService.Model;

namespace MvcService.Controller
{
    public class HomeController : Microsoft.AspNet.Mvc.Controller
    {
        public IActionResult Index()
        {
            var model = new HomeModel();
            return View(model);
        }
    }
}
