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
            var model = new AddCustomer();

            return View("Register", model);
        }

        public ActionResult AddUser(AddCustomer model)
        {
            if (!ModelState.IsValid)
            {
                return View("Register", model);
            }

            model.Profile.Login = model.Profile.Email;//so the user doenst need to enter it also

            //var sessionResponse = APIHelper.AddNewCustomer(model);

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