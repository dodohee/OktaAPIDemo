using System.Web.Mvc;
using OktaAPI.Helpers;
using OktaAPIShared.Models;

namespace OktaCustomerUI.Controllers
{
    public class RegistrationController : Controller
    {
        // GET: Registration
        public ActionResult List()
        {
            var model = APIHelper.GetAllCustomers();

            if (TempData["Message"] != null)
            {
                ViewBag.Message = TempData["Message"];
            }

            return View("List", model);
        }

        public ActionResult Register()
        {
            ViewBag.IsNewUser = true;

            var model = new Customer();

            return View("Register", model);
        }

        public ActionResult EditUser(string Id)
        {
            ViewBag.IsNewUser = false;

            var customer = APIHelper.GetCustomerById(Id);

            return View("Register", customer);
        }

        public ActionResult UpdateUser(Customer model)
        {
            if (!ModelState.IsValid)
            {
                return View("Register", model);
            }
            
            var response = APIHelper.UpdateCustomer(model);

            if (response != null && response.status == "ACTIVE")
            {
                var name = "Unknown";
                try
                {
                    name = $"{model.Profile.FirstName} {model.Profile.LastName}";
                }
                catch
                {
                    // ignored
                }

                TempData["Message"] = name + " has been updated";
            }
            
            return RedirectToAction("List");
        }

        public ActionResult AddUser(Customer model)
        {
            if (!ModelState.IsValid)
            {
                return View("Register", model);
            }
            
            var response = APIHelper.AddNewCustomer(model);

            if (response != null && response.status == "ACTIVE")
            {
                var name = "Unknown";
                try
                {
                    name = $"{model.Profile.FirstName} {model.Profile.LastName}";
                }
                catch
                {
                    // ignored
                }

                TempData["Message"] = name + " has been registered";
            }

            return RedirectToAction("List");
        }
    }
}