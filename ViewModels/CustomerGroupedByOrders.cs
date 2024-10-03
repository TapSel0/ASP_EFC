using Microsoft.AspNetCore.Mvc;

namespace ASP_EFC.ViewModels
{
    public class CustomerGroupedByOrders : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
