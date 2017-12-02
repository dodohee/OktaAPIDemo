using System.Web.Mvc;

namespace OktaCustomerUI.Controllers
{
    public class StockController : Controller
    {
        // GET: Stock
        public ActionResult Index()
        {
            ViewBag.Message = "Historic Stock Prices,";

            return View();
        }
    }
}