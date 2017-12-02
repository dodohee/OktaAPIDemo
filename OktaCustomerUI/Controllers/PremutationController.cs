using System.Web.Mvc;
using OktaCustomerUI.Helpers;
using OktaAPIShared.Models;

namespace OktaCustomerUI.Controllers
{
    public class PermutationController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Permutation Demo";
            var m = new PermutationModel();

            return View(m);
        }

        public ActionResult Permutate(PermutationModel m)
        {
            ViewBag.Message = "Permutation Demo";

            PermutateHelper.GetPermutations(m);
            
            return View("Index", m);
        }
    }
}